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

        private ProcessStartInfo processInfo;

        public LaunchProcessMacro(string name, ProcessStartInfo proc)
        {
            Name = name;
            processInfo = proc;
        }

        public async Task PerformAsync()
        {
            Process.Start(processInfo);
            await Task.Delay(0);
        }

    }
}
