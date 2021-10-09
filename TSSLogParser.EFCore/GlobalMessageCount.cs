using System;
using System.Collections.Generic;

#nullable disable

namespace TSSLogParser.EFCore
{
    public partial class GlobalMessageCount
    {
        public string LogName { get; set; }
        public string ProviderName { get; set; }
        public string TruncatedMessage { get; set; }
        public int? MessageCount { get; set; }
    }
}
