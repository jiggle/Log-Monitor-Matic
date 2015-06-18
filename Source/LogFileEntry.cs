using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogMonitor
{
    public class LogFileEntry
    {
        public string LogDateTime { get; set; }
        public string LogFilePath { get; set; }
        public string LogFileText { get; set; }
    }
}
