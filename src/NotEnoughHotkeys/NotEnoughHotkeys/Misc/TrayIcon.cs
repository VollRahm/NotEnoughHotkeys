using NotEnoughHotkeys.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace NotEnoughHotkeys.Misc
{
    public class TrayIcon
    {
        public NotifyIcon NotifyIcon;

        private MainWindow w;

        public bool IsHookEnabled
        {
            set
            {
                var item = ((ToolStripMenuItem)NotifyIcon.ContextMenuStrip.Items[1]);
                item.CheckedChanged -= new EventHandler(ChangeCheck);
                item.Checked = value;
                item.CheckedChanged += new EventHandler(ChangeCheck);
            }
        }

        public TrayIcon(MainWindow mainWindow)
        {
            NotifyIcon = new NotifyIcon();
            NotifyIcon.Icon = new System.Drawing.Icon(App.GetResourceStream(new Uri("pack://application:,,,/Assets/NEHIcon.ico")).Stream);
            NotifyIcon.Visible = true;
            NotifyIcon.ContextMenuStrip = BuildContextMenu();
            NotifyIcon.MouseClick += new MouseEventHandler(MouseClicked);
            w = mainWindow;
        }

         private void MouseClicked(object sender, MouseEventArgs e)
         {
             if(e.Button == MouseButtons.Left)
             {
                w.Show();
                w.WindowState = System.Windows.WindowState.Normal;
             }
         }

        public async Task ShowNotification(string title, string text, ToolTipIcon ico, int timeout)
        {
            NotifyIcon.ShowBalloonTip(timeout, title, text, ico);
            await Task.Delay(0);
        }

        private ContextMenuStrip BuildContextMenu()
        {
            ContextMenuStrip cm = new ContextMenuStrip();
            cm.Items.AddRange(new ToolStripItem[]
                {
                    new ToolStripMenuItem("Show GUI",null, new EventHandler((o,ev)=>MouseClicked(null, new MouseEventArgs(MouseButtons.Left,0,0,0,0)))),
                    new ToolStripMenuItem("Enabled"){CheckOnClick = true, Checked = true},
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("Quit", null, new EventHandler((o,ev)=>w.QuitAppBtn_Click(o, null)))
                });
            ((ToolStripMenuItem)cm.Items[1]).CheckedChanged += new EventHandler(ChangeCheck);
            return cm;
        }

        private void ChangeCheck(object sender, EventArgs e)
        {
            w.EnabledCb.IsChecked = !w.EnabledCb.IsChecked.Value;
        }
    }
}
