using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NotEnoughHotkeys.Data.Types.Actions
{
    public class SendKeysMacro : IMacroAction
    {
        public string Name { get; set; }
        public string TypeName { get => "Send Keystrokes"; }

        public string Keystrokes { get; set; }

        public SendKeysMacro(string name, string keystrokes)
        {
            Name = name;
            Keystrokes = keystrokes;
        }

        public async Task PerformAsync()
        {
            SendKeys.SendWait(Keystrokes);
            await Task.Delay(0);
        }
    }
}
