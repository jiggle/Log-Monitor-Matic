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
        readonly ThreadedBindingList<FileSystemWatcher> watchers = new ThreadedBindingList<FileSystemWatcher>();
        private readonly ThreadedBindingList<LogFileEntry> logEntries = new ThreadedBindingList<LogFileEntry>();
        ThreadedBindingList<WatchFile> _filesWatching = new ThreadedBindingList<WatchFile>();
        private  Timer _typingStoppedTimer;
        private BindingSource _bs;
        private readonly string[] _commandLineArgs;
        private bool _isInitializing;
        private bool _stopInitializing;

        private object LockWithThis=new Object();
        private string _pathsWatching;
        private CancellationTokenSource _populateFilesCancellationSource;

        public MonitorMain(string[] args)
        {
            InitializeComponent();
            _commandLineArgs = args;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Allow new parts to be added, but not removed once committed.        
            _filesWatching = new ThreadedBindingList<WatchFile>();

            lstFilesWatching.DisplayMember = "FilePath";
            lstFilesWatching.DataSource = GetFilesWatching();

            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = GetLogFileEntries();

            _typingStoppedTimer = new Timer();
            _typingStoppedTimer.Interval = 500;
            _typingStoppedTimer.Tick += typingStoppedTimer_Tick;


            if (_commandLineArgs.Length > 0)
            {
                txtPathsToWatch.Text = _commandLineArgs[0];
            }
            else
            {
                //TODO maybe also default to current executing folder and below or have "/" as default in the app config
                txtPathsToWatch.Text = ConfigurationManager.AppSettings["PathsToMonitor"];
            }
            InitializeWatchers();
        }

        void typingStoppedTimer_Tick(object sender, EventArgs e)
        {
            _typingStoppedTimer.Stop();

            InitializeWatchers();
        }

        private void InitializeWatchers()
        {
            if (_isInitializing) return;
            Task.Factory.StartNew(PopulateWatchers);
        }

        private void PopulateWatchers()
        {
            lock (LockWithThis)
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

                    if (_stopInitializing)
                    {
                        lock (LockWithThis)
                        {
                            _isInitializing = false;
                        }
                        return;
                    }

                    if (watchers.All(w => w.Path != path))
                    {
                        var watcher = new FileSystemWatcher(path)
                        {
                            NotifyFilter = NotifyFilters.LastWrite,
                            Filter = "*.txt"
                        };
                        watcher.Changed += watcher_Changed;
                        watcher.EnableRaisingEvents = true;
                        watcher.IncludeSubdirectories = true;
                        watchers.Add(watcher);
                    }
                }
            }

            var stopWatching = watchers.Where(w => !pathsToWatch.Contains(w.Path)).ToList();
            foreach (var toStop in stopWatching)
            {
                var stopping = toStop;
                Debug.WriteLine("Stop Watching:{0}", toStop.Path);
                watchers.Remove(toStop);
                var stopWatchingFiles = _filesWatching.Where(l => Path.GetDirectoryName(l.FilePath).Contains(stopping.Path)).ToList();

                foreach (var watchFile in stopWatchingFiles)
                {
                    Invoke(new Action<WatchFile>((file) =>
                    {
                        _filesWatching.Remove(file);
                    }), watchFile);
                }
            
            }
            lstFilesWatching.SafeInvoke(
                new Action(() =>
                {
                    lstFilesWatching.DataSource = null;
                    lstFilesWatching.DisplayMember = "FilePath";
                    lstFilesWatching.DataSource = GetFilesWatching();
                    Application.DoEvents();
                }), false);
            UpdateStatus("");

            lock (LockWithThis)
            {
                _isInitializing = false;
            }

        }
        private void PopulateFilesToWatch(string path)
        {
            UpdateStatus("Registering files to watch..." + path);

            var resolvedPath = Environment.ExpandEnvironmentVariables(path);
            var currentWatchedFiles = _filesWatching.ToList();
            if (Directory.Exists(resolvedPath)) { 
                var filesToWatch = Directory.GetFiles(resolvedPath, "*.txt", SearchOption.AllDirectories);
                foreach (var file in filesToWatch.ToList())
                {
                    if (_stopInitializing)
                    {
                        lock (LockWithThis)
                        {
                            _isInitializing = false;
                        }
                        return;
                    }

                    var fileSize = new FileInfo(file).Length;

                    var alreadyWatching = currentWatchedFiles.FirstOrDefault(f => f!=null && f.FilePath == file);

                    if (alreadyWatching == null) { 
                        var filetoWatch = new WatchFile()
                        {
                            FilePath = file,
                            LastSize = fileSize
                        };

                        _filesWatching.Add(filetoWatch);
                    }
                    UpdateStatus(string.Format("Registering files to watch...{0}",file));
                    
                }
                lstFilesWatching.SafeInvoke(
                       new Action(() =>
                       {
                           lstFilesWatching.DataSource = null;
                           lstFilesWatching.DisplayMember = "FilePath";
                           lstFilesWatching.DataSource = GetFilesWatching();
                           Application.DoEvents();
                       }), false);
            }
        }

        void UpdateStatus(string text)
        {
            statusStrip1.SafeInvoke(
               new Action(() =>
                {
                    toolStripStatusLabel1.Text = text;
                    toolStripStatusLabel1.Invalidate();
                    Application.DoEvents();
                }),true);
        }


        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Debug.WriteLine(e.FullPath);
            var file = e.FullPath;
            var eventTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
                                            CultureInfo.InvariantCulture);
            var watchFile = _filesWatching.FirstOrDefault(f => f.FilePath.ToLower() == file.ToLower());

            if (watchFile == null) return;

            var shouldAdd = true;

            if (logEntries.Any())
            {
                var lastLogged = logEntries.OrderByDescending(f => f.LogDateTime).FirstOrDefault(f => f.LogFilePath == file);
                if (lastLogged != null)
                {
                    var eventTimeValue = DateTime.Parse(eventTime);
                    var lastLoggedValue = DateTime.Parse(lastLogged.LogDateTime);
                    Debug.WriteLine(eventTimeValue.Subtract(lastLoggedValue).TotalMilliseconds);
                    shouldAdd = eventTimeValue.Subtract(lastLoggedValue).TotalMilliseconds > 200;
                }
            }

            if (!shouldAdd) return;

            using (var fs = new FileStream(file, FileMode.Open,FileAccess.Read, FileShare.ReadWrite))
            {
                fs.Seek(watchFile.LastSize, SeekOrigin.Begin);
                var bytesToRead = (int) (fs.Length - watchFile.LastSize);
                if (bytesToRead > 0) { 
                    var bytes = new byte[bytesToRead];
                    fs.Read(bytes, 0, bytesToRead); 
                    var logText = System.Text.Encoding.Default.GetString(bytes);
                    watchFile.LastSize = fs.Length;
                    Invoke(new Action<string, string, string>((filePath, txt, t) =>
                    {
                        logEntries.Add(new LogFileEntry()
                        {
                            LogFilePath = filePath,
                            LogFileText = txt,
                            LogDateTime = t
                        });
                        dataGridView1.DataSource = GetLogFileEntries();
                    }), file, logText, eventTime);
                }
            }
        }

        private void txtPathsToWatch_TextChanged(object sender, EventArgs e)
        {
            _typingStoppedTimer.Start();
        }


        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var filePath = dataGridView1.Rows[e.RowIndex];
            var p = (LogFileEntry)filePath.DataBoundItem;

            OpenFileInNotepad(p.LogFilePath);
        }

        private void OpenFileInNotepad(string filePath)
        {
            var notepad = new Process();
            notepad.StartInfo = new ProcessStartInfo
            {
                FileName = string.Format("notepad.exe"),
                Arguments = filePath
            };
            notepad.Start();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            logEntries.Clear();
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           var row= dataGridView1.Rows[e.RowIndex];
           row.Height= row.GetPreferredHeight(e.RowIndex, DataGridViewAutoSizeRowMode.AllCells, true);
        }

        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var row = dataGridView1.Rows[e.RowIndex];
            Debug.WriteLine("Row Height:{0} Min Height:{1}", row.Height,row.MinimumHeight);
            row.Height = 22;
        }

        private void txtSearchFiles_TextChanged(object sender, EventArgs e)
        {
            lstFilesWatching.DataSource = GetFilesWatching();
        }

        private ThreadedBindingList<WatchFile> GetFilesWatching()
        {
            if (string.IsNullOrEmpty(txtSearchFiles.Text)) return _filesWatching;

            var filteredFiles = new ThreadedBindingList<WatchFile>(_filesWatching.Where(f => f.FilePath.ToLower().Contains(txtSearchFiles.Text.ToLower())).ToList());
            return filteredFiles;
        }

        private ThreadedBindingList<LogFileEntry> GetLogFileEntries()
        {
            if (string.IsNullOrEmpty(txtSearchLogs.Text)) return logEntries;

            var filteredLogs = new ThreadedBindingList<LogFileEntry>(logEntries.Where(f => f.LogFileText.ToLower().Contains(txtSearchLogs.Text.ToLower())).ToList());
            return filteredLogs;
        }

        private void lstFilesWatching_DoubleClick(object sender, EventArgs e)
        {
            var watchFile = (WatchFile)lstFilesWatching.SelectedItem;

            OpenFileInNotepad(watchFile.FilePath);
        }

        private void txtSearchLogs_TextChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = GetLogFileEntries();
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearchLogs.Text = string.Empty;
        }


    }
}
