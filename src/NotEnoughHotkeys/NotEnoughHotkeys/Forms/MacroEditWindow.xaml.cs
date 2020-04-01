using NotEnoughHotkeys.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NotEnoughHotkeys.Forms
{
    /// <summary>
    /// Interaktionslogik für MacroEditWindow.xaml
    /// </summary>
    public partial class MacroEditWindow : Window
    {
        private MacroItem Macro;

        public MacroEditWindow(MacroItem macro, bool newMacro)
        {
            InitializeComponent();
            if (!newMacro)
            {
                this.Title = "Edit Macro";
                this.nameTb.Text = macro.Action.Name;
            }
            MacroEditWindowHandlers.Init(this);
        }
    }
}
