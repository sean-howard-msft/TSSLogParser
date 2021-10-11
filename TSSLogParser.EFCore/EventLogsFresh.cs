﻿using System;
using System.Collections.Generic;

#nullable disable

namespace TSSLogParser
{
    public partial class EventLogsFresh
    {
        public int RecordId { get; set; }
        public string MachineName { get; set; }
        public string LogName { get; set; }
        public DateTime TimeCreated { get; set; }
        public string LevelDisplayName { get; set; }
        public int? Level { get; set; }
        public int? Id { get; set; }
        public string ProviderName { get; set; }
        public string Message { get; set; }
        public string ContainerLog { get; set; }
        public string TruncatedMessage { get; set; }
        public string CountryCode { get; set; }
        public string Region { get; set; }
        public string MachineType { get; set; }
        public string AppCode { get; set; }
        public string InfraCode { get; set; }
        public string InstanceNum { get; set; }
        public string Domain { get; set; }
    }
}
