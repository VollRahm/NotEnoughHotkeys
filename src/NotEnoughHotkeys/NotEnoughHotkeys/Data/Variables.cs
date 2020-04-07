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
        public static Config Config = new Config();
        public static ObservableCollection<MacroItem> Macros = new ObservableCollection<MacroItem>();
        public static bool FirstEverStartup = false;
    }
}
