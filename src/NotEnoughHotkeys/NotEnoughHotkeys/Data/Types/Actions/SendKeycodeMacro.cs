using System;
using System.Threading.Tasks;
using static NotEnoughHotkeys.Misc.Helper;

namespace NotEnoughHotkeys.Data.Types.Actions
{
    public class SendKeycodeMacro : IMacroAction
    {
        public string Name { get; set; }
        public string TypeName => "Send Single Keycode";
        public int KeyCode { get; set; }

        public SendKeycodeMacro(string name, int KeyCode)
        {
            Name = name;
            this.KeyCode = KeyCode; 
        }

        public async Task PerformAsync()
        {
            await DoKeyPress((byte)KeyCode);
        }

        private async Task DoKeyPress(byte KeyCode)
        {
            NativeMethods.keybd_event(KeyCode, 0, NativeMethods.KEYEVENTF_EXTENDEDKEY, new UIntPtr(0));
            await Task.Delay(10);
            NativeMethods.keybd_event(KeyCode, 0, NativeMethods.KEYEVENTF_KEYUP, new UIntPtr(0));
        }
    }
}
