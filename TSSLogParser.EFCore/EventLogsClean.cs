﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

#nullable disable

namespace TSSLogParser.EFCore
{
    public partial class EventLogsClean
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
        public string MsftDocsSearch { get; set; }
        public string MsftDocsTopResult { get; set; }
        public string WebSearch { get; set; }
        public string WebTopResult { get; set; }
    }
}