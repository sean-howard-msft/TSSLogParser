using System;
using System.Collections.Generic;

#nullable disable

namespace TSSLogParser
{
    public partial class RegionalTotal
    {
        public string LogName { get; set; }
        public string ProviderName { get; set; }
        public string TruncatedMessage { get; set; }
        public string LevelDisplayName { get; set; }
        public string CountryCode { get; set; }
        public string Region { get; set; }
        public int? MachineCount { get; set; }
        public int? MessageCount { get; set; }
    }
}
