namespace LogMonitor
{
    partial class MonitorMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.txtWildcard = new System.Windows.Forms.TextBox();
            this.btnClearSearch = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSearchLogs = new System.Windows.Forms.TextBox();
            this.btnAddFolder = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.txtPathsToWatch = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgLogEntries = new System.Windows.Forms.DataGridView();
            this.lstFilesWatching = new System.Windows.Forms.ListBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtSearchFiles = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgLogEntries)).BeginInit();
            this.panel2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.txtWildcard);
            this.panel1.Controls.Add(this.btnClearSearch);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtSearchLogs);
            this.panel1.Controls.Add(this.btnAddFolder);
            this.panel1.Controls.Add(this.btnClear);
            this.panel1.Controls.Add(this.txtPathsToWatch);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(904, 47);
            this.panel1.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(747, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Wildcard";
            // 
            // txtWildcard
            // 
            this.txtWildcard.Location = new System.Drawing.Point(792, 21);
            this.txtWildcard.Name = "txtWildcard";
            this.txtWildcard.Size = new System.Drawing.Size(100, 20);
            this.txtWildcard.TabIndex = 9;
            // 
            // btnClearSearch
            // 
            this.btnClearSearch.Location = new System.Drawing.Point(702, 22);
            this.btnClearSearch.Name = "btnClearSearch";
            this.btnClearSearch.Size = new System.Drawing.Size(39, 20);
            this.btnClearSearch.TabIndex = 8;
            this.btnClearSearch.Text = "Clear";
            this.btnClearSearch.UseVisualStyleBackColor = true;
            this.btnClearSearch.Click += new System.EventHandler(this.btnClearSearch_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Monitor Paths:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Search Content:";
            // 
            // txtSearchLogs
            // 
            this.txtSearchLogs.Location = new System.Drawing.Point(95, 22);
            this.txtSearchLogs.Name = "txtSearchLogs";
            this.txtSearchLogs.Size = new System.Drawing.Size(601, 20);
            this.txtSearchLogs.TabIndex = 5;
            this.txtSearchLogs.TextChanged += new System.EventHandler(this.txtSearchLogs_TextChanged);
            // 
            // btnAddFolder
            // 
            this.btnAddFolder.Location = new System.Drawing.Point(702, 0);
            this.btnAddFolder.Name = "btnAddFolder";
            this.btnAddFolder.Size = new System.Drawing.Size(39, 20);
            this.btnAddFolder.TabIndex = 4;
            this.btnAddFolder.Text = "Add";
            this.btnAddFolder.UseVisualStyleBackColor = true;
            this.btnAddFolder.Click += new System.EventHandler(this.btnAddFolder_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(747, 0);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 20);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Clear Logs";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // txtPathsToWatch
            // 
            this.txtPathsToWatch.Location = new System.Drawing.Point(95, 0);
            this.txtPathsToWatch.Name = "txtPathsToWatch";
            this.txtPathsToWatch.Size = new System.Drawing.Size(601, 20);
            this.txtPathsToWatch.TabIndex = 2;
            this.txtPathsToWatch.Text = "D:\\Projects\\Qore\\AppsBuilt;";
            this.txtPathsToWatch.TextChanged += new System.EventHandler(this.txtPathsToWatch_TextChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 47);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgLogEntries);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lstFilesWatching);
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Panel2.Controls.Add(this.statusStrip1);
            this.splitContainer1.Size = new System.Drawing.Size(904, 597);
            this.splitContainer1.SplitterDistance = 433;
            this.splitContainer1.TabIndex = 8;
            // 
            // dgLogEntries
            // 
            this.dgLogEntries.AllowUserToAddRows = false;
            this.dgLogEntries.AllowUserToDeleteRows = false;
            this.dgLogEntries.AllowUserToOrderColumns = true;
            this.dgLogEntries.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgLogEntries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgLogEntries.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgLogEntries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgLogEntries.Location = new System.Drawing.Point(0, 0);
            this.dgLogEntries.Name = "dgLogEntries";
            this.dgLogEntries.Size = new System.Drawing.Size(904, 433);
            this.dgLogEntries.TabIndex = 6;
            this.dgLogEntries.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgLogEntries_CellContentClick);
            this.dgLogEntries.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgLogEntries_CellContentDoubleClick);
            this.dgLogEntries.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgLogEntries_RowHeaderMouseClick);
            // 
            // lstFilesWatching
            // 
            this.lstFilesWatching.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstFilesWatching.FormattingEnabled = true;
            this.lstFilesWatching.Location = new System.Drawing.Point(0, 19);
            this.lstFilesWatching.Name = "lstFilesWatching";
            this.lstFilesWatching.Size = new System.Drawing.Size(904, 119);
            this.lstFilesWatching.TabIndex = 6;
            this.lstFilesWatching.DoubleClick += new System.EventHandler(this.lstFilesWatching_DoubleClick);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.txtSearchFiles);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(904, 19);
            this.panel2.TabIndex = 5;
            // 
            // txtSearchFiles
            // 
            this.txtSearchFiles.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtSearchFiles.Location = new System.Drawing.Point(0, 0);
            this.txtSearchFiles.Name = "txtSearchFiles";
            this.txtSearchFiles.Size = new System.Drawing.Size(904, 20);
            this.txtSearchFiles.TabIndex = 5;
            this.txtSearchFiles.TextChanged += new System.EventHandler(this.txtSearchFiles_TextChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 138);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(904, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(84, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatus";
            // 
            // MonitorMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(904, 644);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Name = "MonitorMain";
            this.Text = "Log Monitor";
            this.Load += new System.EventHandler(this.MonitorMain_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgLogEntries)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtPathsToWatch;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgLogEntries;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnAddFolder;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ListBox lstFilesWatching;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtSearchFiles;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSearchLogs;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnClearSearch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtWildcard;


    }
}

