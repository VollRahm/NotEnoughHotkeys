using NotEnoughHotkeys.Data.Types;
using System.Collections.ObjectModel;

namespace NotEnoughHotkeys.Data
{
    public static class Variables
    {
        public static Config Config = new Config();
        public static ObservableCollection<MacroItem> Macros = new ObservableCollection<MacroItem>();
        public static bool FirstEverStartup = false;
    }
}
