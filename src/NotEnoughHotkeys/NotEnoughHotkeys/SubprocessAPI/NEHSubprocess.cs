using NotEnoughHotkeys.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NotEnoughHotkeys.SubprocessAPI
{
    public class NEHSubprocess
    {
        public delegate void KeyEventRecievedHandler(object sender, NEHKeyPressEventArgs e);
        public event KeyEventRecievedHandler KeyEventRecieved;

        public event EventHandler ProcessEnded;

        private const string ExecutablePath = "bin\\NEHSubprocess.exe";
        private const string ExecutablePathAdmin = "bin\\NEHSubprocessAdmin.exe";
        private ProcessStartInfo psi;
        private bool IsAdmin = false;
        private string KeyboardToBlock;

        private PipeServer Pipe;

        public NEHSubprocess(bool isAdminProcess, string KeyboardToBlock, string pipeName)
        {
            IsAdmin = isAdminProcess;
            this.KeyboardToBlock = KeyboardToBlock;

            Pipe = new PipeServer(pipeName);
            Pipe.OnMessageRecieved += new PipeServer.MessageRecievedHandler(PipeMessageRecieved);
            Pipe.OnPipeClientClosed += PipeClosed;
        }

        private void PipeClosed(object sender, EventArgs e)
        {
            ProcessEnded.Invoke(this, null);
        }

        private void PipeMessageRecieved(string Reply)
        {
            var command = Reply.Split(' ');
            NEHKeyState state = command[0] == "MAKE" ? NEHKeyState.KeyDown : NEHKeyState.KeyUp;
            int KeyCode = int.Parse(command[1]);
            Key key = KeyInterop.KeyFromVirtualKey(KeyCode);
            NEHKeyPressEventArgs eventArgs = new NEHKeyPressEventArgs(state, key, KeyCode);
            KeyEventRecieved.Invoke(this, eventArgs);
        }

        public async Task StartProcess()
        {
            if (!IsAdmin)
                ProcessLauncher.ExecuteProcessUnElevated(ExecutablePath, KeyboardToBlock, Directory.GetCurrentDirectory());
            else
            {
                psi = new ProcessStartInfo(ExecutablePathAdmin, KeyboardToBlock);
                psi.UseShellExecute = true;
                Process.Start(psi);
            }

            await Pipe.StartReadingAsync();
        }

        public async Task StopProcess()
        {
            await Pipe.StopReadingAsync();
            if (IsAdmin)
                Process.GetProcessesByName(ExecutablePathAdmin).ToList().ForEach(x => x.Kill());
            else
                Process.GetProcessesByName(ExecutablePath).ToList().ForEach(x => x.Kill());
        }
    }

    public class NEHKeyPressEventArgs : EventArgs
    {
        public NEHKeyState State { get; }
        public Key Key { get; }
        public int KeyCode { get; }

        public NEHKeyPressEventArgs(NEHKeyState state, Key key, int keyCode)
        {
            State = state;
            Key = key;
            KeyCode = keyCode;
        }
    }

    public enum NEHKeyState
    {
        KeyUp,
        KeyDown
    }
    
}
