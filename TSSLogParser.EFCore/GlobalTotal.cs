using System;
using System.Collections.Generic;

#nullable disable

namespace TSSLogParser
{
    public partial class GlobalTotal
    {
        public string LogName { get; set; }
        public string ProviderName { get; set; }
        public string TruncatedMessage { get; set; }
        public int? MachineCount { get; set; }
        public string LevelDisplayName { get; set; }
        public int? MessageCount { get; set; }
    }
}
