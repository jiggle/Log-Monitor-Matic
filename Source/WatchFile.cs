using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogMonitor
{
    public class WatchFile
    {
        public string FilePath { get; set; }
        public long LastSize { get; set; }
        public override string ToString()
        {
            return FilePath;
        }
    }
}
