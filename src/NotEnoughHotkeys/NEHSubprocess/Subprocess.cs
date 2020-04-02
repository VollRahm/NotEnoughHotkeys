using NEHSubprocess.Data;
using NEHSubprocess.Misc;
using RawInput_dll;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static NEHSubprocess.Data.Constants;

namespace NEHSubprocess
{
    public partial class Subprocess : Form
    {
        
        private bool isAdminProcess = false;
        private string hwidToBlock;
        private string RecieverPipeName;

        private RawInput rawInput;
        private PipeClient pipeClient;

        public Subprocess()
        {
            InitializeComponent();
        }

        private void LoadProperties()
        {
            isAdminProcess = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            RecieverPipeName = isAdminProcess ? PIPENAME_ADMIN : PIPENAME;

            try
            {
                hwidToBlock = Environment.GetCommandLineArgs()[1];
            }
            catch (IndexOutOfRangeException)
            {
                MessageBox.Show("Something went wrong while starting the Subprocess.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
       
        private void Subprocess_Shown(object sender, EventArgs e)
        {
            LoadProperties();
            pipeClient = new PipeClient(RecieverPipeName);

            rawInput = new RawInput(Handle, false);
            rawInput.KeyPressed += new RawKeyboard.DeviceEventHandler(RawInputHandler);

            NEHHook.StartHook(Handle);           

            if (!NEHHook.StartHook(Handle))
                MessageBox.Show("Something went wrong while starting the Hook.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void RawInputHandler(object sender, RawInputEventArg e)
        {
            var ev = e.KeyPressEvent;
            if (ev.DeviceName == hwidToBlock)
            {
                
                blockNextKeystroke = true;
                _ = HandleMacroAsync(e.KeyPressEvent.KeyPressState, e.KeyPressEvent.VKey);
            }
            else
                blockNextKeystroke = false;
        }

        private bool blockNextKeystroke = false;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WH_HOOK)
            {
                if (blockNextKeystroke)
                {
                    m.Result = new IntPtr(-1);
                    blockNextKeystroke = false;
                }
            }
        }

        private async Task HandleMacroAsync(string Status, int vKey)
        {
            if (isAdminProcess)
                if (!ProcessHelper.IsForegroundProcessAdmin()) return;

            bool success = await pipeClient.Send($"{Status} {vKey}");
            if (!success)
            {
                NEHHook.StopHook();
                Environment.Exit(1);
            }

            await Task.Delay(0);
        }




    }
}
