﻿using System;
using System.Collections.Generic;

#nullable disable

namespace TSSLogParser.EFCore
{
    public partial class MachineCount
    {
        public string LogName { get; set; }
        public string ProviderName { get; set; }
        public string TruncatedMessage { get; set; }
        public int? MachineCount1 { get; set; }
    }
}
