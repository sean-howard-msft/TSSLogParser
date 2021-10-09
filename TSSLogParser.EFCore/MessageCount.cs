using System;
using System.Collections.Generic;

#nullable disable

namespace TSSLogParser.EFCore
{
    public partial class MessageCount
    {
        public string LogName { get; set; }
        public string ProviderName { get; set; }
        public string TruncatedMessage { get; set; }
        public string MachineName { get; set; }
        public int? MessageCount1 { get; set; }
    }
}
