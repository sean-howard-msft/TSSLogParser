﻿using System;
using System.Collections.Generic;

#nullable disable

namespace TSSLogParser.EFCore
{
    public partial class MachineMetadatum
    {
        public string CountryCode { get; set; }
        public string Region { get; set; }
        public string MachineType { get; set; }
        public string AppCode { get; set; }
        public string InfraCode { get; set; }
        public string InstanceNum { get; set; }
        public string Domain { get; set; }
        public string MachineName { get; set; }
    }
}