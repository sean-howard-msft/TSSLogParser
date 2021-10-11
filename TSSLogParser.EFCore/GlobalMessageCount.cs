using System;
using System.Collections.Generic;

#nullable disable

namespace TSSLogParser
{
    public partial class GlobalMessageCount
    {
        public string LogName { get; set; }
        public string ProviderName { get; set; }
        public string TruncatedMessage { get; set; }
        public string LevelDisplayName { get; set; }
        public int? MessageCount { get; set; }
    }
}
