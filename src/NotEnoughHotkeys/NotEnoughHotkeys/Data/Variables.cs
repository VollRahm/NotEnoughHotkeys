using NotEnoughHotkeys.Data.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotEnoughHotkeys.Data
{
    public static class Variables
    {
        public static Keyboard TargetKeyboard = new Keyboard() { HWID = "" }; //because of nullpointerexception
        public static ObservableCollection<MacroItem> Macros = new ObservableCollection<MacroItem>();
    }
}
