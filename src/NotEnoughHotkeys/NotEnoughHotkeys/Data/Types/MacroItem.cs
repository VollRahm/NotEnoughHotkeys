using NotEnoughHotkeys.Data.Types.Actions;
using System.Windows.Input;

namespace NotEnoughHotkeys.Data.Types
{
    public class MacroItem
    {
        public string Name { get; set; }
        public Key Hotkey { get; set; }
        public IMacroAction Action { get;set; }

        public MacroItem(Key hotkey, IMacroAction action)
        {
            Hotkey = hotkey;
            Action = action;
        }
    }
}
