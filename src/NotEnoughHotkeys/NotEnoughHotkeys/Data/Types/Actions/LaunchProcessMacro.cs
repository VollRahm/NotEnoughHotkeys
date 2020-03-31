using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotEnoughHotkeys.Data.Types.Actions
{
    public class LaunchProcessMacro : IMacroAction
    {
        public string Name { get; set; }
        public string TypeName { get => "Launch Process"; }

        private Process process;

        public LaunchProcessMacro(string name, Process proc)
        {
            Name = name;
            process = proc;
        }

        public void Perform()
        {
            process.Start();
        }

    }
}
