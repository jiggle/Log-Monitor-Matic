using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogMonitor.Extensions;
using Timer = System.Windows.Forms.Timer;

namespace LogMonitor
{
    public partial class MonitorMain : Form
    {
        readonly ThreadedBindingList<FileSystemWatcher> _watchers = new ThreadedBindingList<FileSystemWatcher>();
        private readonly ThreadedBindingList<LogFileEntry> _logEntries = new ThreadedBindingList<LogFileEntry>();
        ThreadedBindingList<WatchFile> _filesWatching = new ThreadedBindingList<WatchFile>();
        
        private Timer _typingStoppedTimer;

        private readonly string[] _commandLineArgs;
        private bool _isInitializing;
        private bool _stopInitializing;

        private readonly object _lockWithThis = new Object();
        private string _pathsWatching;

        public MonitorMain(string[] args)
        {
            InitializeComponent();
            _commandLineArgs = args;
        }

        private void MonitorMain_Load(object sender, EventArgs e)
        {
            // Allow new parts to be added, but not removed once committed.        
            _filesWatching = new ThreadedBindingList<WatchFile>();

            lstFilesWatching.DisplayMember = "FilePath";
            lstFilesWatching.DataSource = GetFilesWatching();

            dgLogEntries.AutoGenerateColumns = true;
            dgLogEntries.DataSource = GetLogFileEntries();

            _typingStoppedTimer = new Timer {Interval = 500};
            _typingStoppedTimer.Tick += typingStoppedTimer_Tick;


            if (_commandLineArgs.Length > 0)
            {
                txtPathsToWatch.Text = _commandLineArgs[0];
            }
            else
            {
                //TODO maybe also default to current executing folder and below or have "/" as default in the app config
                txtPathsToWatch.Text = ConfigurationManager.AppSettings["PathsToMonitor"];
                txtWildcard.Text = ConfigurationManager.AppSettings["FileWildcard"];
            }
            InitializeWatchers();
        }

        void typingStoppedTimer_Tick(object sender, EventArgs e)
        {
            _typingStoppedTimer.Stop();

            InitializeWatchers();
        }

        #region Setup Path Watchers and Populate the files to monitor
        
        private void InitializeWatchers()
        {
            if (_isInitializing) return;
            Task.Factory.StartNew(PopulateWatchers);
        }

        private void PopulateWatchers()
        {
            lock (_lockWithThis)
            {
                _isInitializing = true;
            }

            _pathsWatching = txtPathsToWatch.Text;
            var pathsToWatch = txtPathsToWatch.Text.Split(';');

            foreach (var path in pathsToWatch.Where(p => !string.IsNullOrEmpty(p)))
            {
                if (Directory.Exists(path))
                {
                    PopulateFilesToWatch(path);

                    if (_watchers.All(w => w.Path != path))
                    {
                        AddFileWatcher(path);
                    }
                }
            }

            StopWatchingFilesNotInPath(pathsToWatch);

            RefreshListOfFiles();

            UpdateStatus("");

            lock (_lockWithThis)
            {
                _isInitializing = false;
            }

        }

        private void AddFileWatcher(string path)
        {
            var filters = txtWildcard.Text.Split(';');
            foreach (var filter in filters)
            {
                var watcher = new FileSystemWatcher(path)
                {
                    NotifyFilter = NotifyFilters.LastWrite,
                    Filter = filter
                };
                watcher.Changed += watcher_Changed;
                watcher.EnableRaisingEvents = true;
                watcher.IncludeSubdirectories = true;
                _watchers.Add(watcher);
            }
            
        }

        private void StopWatchingFilesNotInPath(string[] pathsToWatch)
        {
            var stopWatching = _watchers.Where(w => !pathsToWatch.Contains(w.Path)).ToList();
            foreach (var toStop in stopWatching)
            {
                var stopping = toStop;
                Debug.WriteLine("Stop Watching:{0}", toStop.Path);
                _watchers.Remove(toStop);
                var stopWatchingFiles =
                    _filesWatching.Where(l => Path.GetDirectoryName(l.FilePath).Contains(stopping.Path)).ToList();

                foreach (var watchFile in stopWatchingFiles)
                {
                    Invoke(new Action<WatchFile>((file) => { _filesWatching.Remove(file); }), watchFile);
                }
            }
        }

        private void PopulateFilesToWatch(string path)
        {
            UpdateStatus("Registering files to watch..." + path);

            var resolvedPath = Environment.ExpandEnvironmentVariables(path);
            var currentWatchedFiles = _filesWatching.ToList();
            if (Directory.Exists(resolvedPath))
            {
                var searchPatterns = txtWildcard.Text.Split(';');
                foreach (var pattern in searchPatterns)
                {
                    var filesToWatch = FilesToWatch(resolvedPath, pattern);
                  foreach (var file in filesToWatch.ToList())
                    {
                        if (_stopInitializing)
                        {
                            lock (_lockWithThis)
                            {
                                _isInitializing = false;
                            }
                            return;
                        }

                        var fileSize = new FileInfo(file).Length;

                        var alreadyWatching = currentWatchedFiles.FirstOrDefault(f => f != null && f.FilePath == file);

                        if (alreadyWatching == null)
                        {
                            var filetoWatch = new WatchFile()
                            {
                                FilePath = file,
                                LastSize = fileSize
                            };

                            _filesWatching.Add(filetoWatch);
                        }
                        UpdateStatus(string.Format("Registering files to watch...{0}", file));

                    }
                }

                
                RefreshListOfFiles();
            }
        }

        private static string[] FilesToWatch(string resolvedPath, string pattern)
        {
            return Directory.GetFiles(resolvedPath, pattern, SearchOption.AllDirectories);
        }

        #endregion

        private void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Debug.WriteLine(e.FullPath);
            var file = e.FullPath;
            
            var eventTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            var watchFile = _filesWatching.FirstOrDefault(f => f.FilePath.ToLower() == file.ToLower());

            if (watchFile == null) return;

            var shouldAdd = true;

            if (_logEntries.Any())
            {
                var lastLogged = _logEntries.OrderByDescending(f => f.LogDateTime).FirstOrDefault(f => f.LogFilePath == file);
                if (lastLogged != null)
                {
                    var eventTimeValue = DateTime.Parse(eventTime);
                    var lastLoggedValue = DateTime.Parse(lastLogged.LogDateTime);
                    Debug.WriteLine(eventTimeValue.Subtract(lastLoggedValue).TotalMilliseconds);
                    //Don't the log change if we have just added - work around for the method being called twice
                    shouldAdd = eventTimeValue.Subtract(lastLoggedValue).TotalMilliseconds > 200;
                }
            }

            if (!shouldAdd) return;

            ExtractLatestFileChangesAndAddToLog(file, watchFile, eventTime);
        }

        private void ExtractLatestFileChangesAndAddToLog(string file, WatchFile watchFile, string eventTime)
        {
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                fs.Seek(watchFile.LastSize, SeekOrigin.Begin);
                var bytesToRead = (int)(fs.Length - watchFile.LastSize);
                if (bytesToRead > 0)
                {
                    var bytes = new byte[bytesToRead];
                    fs.Read(bytes, 0, bytesToRead);
                    var logText = System.Text.Encoding.Default.GetString(bytes);
                    watchFile.LastSize = fs.Length;
                    AddLogEntry(file, eventTime, logText);
                }
            }
        }

        private void RefreshListOfFiles()
        {
            lstFilesWatching.SafeInvoke(
                () =>
                {
                    lstFilesWatching.DataSource = null;
                    lstFilesWatching.DisplayMember = "FilePath";
                    lstFilesWatching.DataSource = GetFilesWatching();
                    Application.DoEvents();
                }, false);
        }

        void UpdateStatus(string text)
        {
            statusStrip1.SafeInvoke(
               () =>
               {
                   toolStripStatusLabel1.Text = text;
                   toolStripStatusLabel1.Invalidate();
                   Application.DoEvents();
               }, true);
        }

        private void AddLogEntry(string file, string eventTime, string logText)
        {
            Invoke(new Action<string, string, string>((filePath, txt, t) =>
            {
                _logEntries.Add(new LogFileEntry()
                {
                    LogFilePath = filePath,
                    LogFileText = txt,
                    LogDateTime = t
                });
                dgLogEntries.DataSource = GetLogFileEntries();
            }), file, logText, eventTime);
        }

        private static void OpenFileInNotepad(string filePath)
        {
            var notepad = new Process();
            notepad.StartInfo = new ProcessStartInfo
            {
                FileName = string.Format("notepad.exe"),
                Arguments = filePath
            };
            notepad.Start();
        }

        #region UI

        private void txtPathsToWatch_TextChanged(object sender, EventArgs e)
        {
            _typingStoppedTimer.Start();
        }

        private ThreadedBindingList<WatchFile> GetFilesWatching()
        {
            if (string.IsNullOrEmpty(txtSearchFiles.Text)) return _filesWatching;

            var filteredFiles = new ThreadedBindingList<WatchFile>(_filesWatching.Where(f => f.FilePath.ToLower().Contains(txtSearchFiles.Text.ToLower())).ToList());
            return filteredFiles;
        }

        private ThreadedBindingList<LogFileEntry> GetLogFileEntries()
        {
            if (string.IsNullOrEmpty(txtSearchLogs.Text)) return _logEntries;

            var filteredLogs = new ThreadedBindingList<LogFileEntry>(_logEntries.Where(f => f.LogFileText.ToLower().Contains(txtSearchLogs.Text.ToLower())).ToList());
            return filteredLogs;
        }


        private void btnClear_Click(object sender, EventArgs e)
        {
            _logEntries.Clear();
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                var pathsToWatch = txtPathsToWatch.Text.Split(';');

                if (pathsToWatch.Contains(folderBrowserDialog1.SelectedPath)) return;

                if (txtPathsToWatch.Text.Length > 0 && txtPathsToWatch.Text.Right(1) != ";")
                {
                    txtPathsToWatch.Text = txtPathsToWatch.Text + ";";
                }

                txtPathsToWatch.Text = txtPathsToWatch.Text + folderBrowserDialog1.SelectedPath;
            }
        }

        private void dgLogEntries_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var row = dgLogEntries.Rows[e.RowIndex];
            row.Height = row.GetPreferredHeight(e.RowIndex, DataGridViewAutoSizeRowMode.AllCells, true);
        }

        private void dgLogEntries_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var row = dgLogEntries.Rows[e.RowIndex];
            Debug.WriteLine("Row Height:{0} Min Height:{1}", row.Height, row.MinimumHeight);
            row.Height = 22;
        }

        private void txtSearchFiles_TextChanged(object sender, EventArgs e)
        {
            lstFilesWatching.DataSource = GetFilesWatching();
        }

        private void lstFilesWatching_DoubleClick(object sender, EventArgs e)
        {
            var watchFile = (WatchFile)lstFilesWatching.SelectedItem;

            OpenFileInNotepad(watchFile.FilePath);
        }

        private void txtSearchLogs_TextChanged(object sender, EventArgs e)
        {
            dgLogEntries.DataSource = GetLogFileEntries();
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearchLogs.Text = string.Empty;
        }


        private void dgLogEntries_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var filePath = dgLogEntries.Rows[e.RowIndex];
            var p = (LogFileEntry)filePath.DataBoundItem;

            OpenFileInNotepad(p.LogFilePath);
        }

        #endregion

   

    }
}
