using System;
using System.Collections.Generic;

#nullable disable

namespace TSSLogParser
{
    public partial class GlobalMachineCount
    {
        public string LogName { get; set; }
        public string ProviderName { get; set; }
        public string TruncatedMessage { get; set; }
        public int? MachineCount { get; set; }
    }
}
