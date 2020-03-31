using NotEnoughHotkeys.Data.Types.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NotEnoughHotkeys.Data.Types
{
    public class MacroItem
    {
        public Key Hotkey { get; set; }
        public IMacroAction Action { get;set; }

        public MacroItem(Key hotkey, IMacroAction action)
        {
            Hotkey = hotkey;
            Action = action;
        }
    }
}
