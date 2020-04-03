﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotEnoughHotkeys.Data.Types.Actions
{
    public interface IMacroAction
    {
        string Name { get; set; }
        string TypeName { get; }
        Task PerformAsync();
    }
}