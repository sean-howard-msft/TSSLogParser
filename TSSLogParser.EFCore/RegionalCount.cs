using System;
using System.Collections.Generic;

#nullable disable

namespace TSSLogParser.EFCore
{
    public partial class RegionalCount
    {
        public string LogName { get; set; }
        public string ProviderName { get; set; }
        public string TruncatedMessage { get; set; }
        public string MachineName { get; set; }
        public string CountryCode { get; set; }
        public string Region { get; set; }
        public string MachineType { get; set; }
        public string AppCode { get; set; }
        public string InfraCode { get; set; }
        public string InstanceNum { get; set; }
        public string Domain { get; set; }
        public int? MessageCount { get; set; }
    }
}
