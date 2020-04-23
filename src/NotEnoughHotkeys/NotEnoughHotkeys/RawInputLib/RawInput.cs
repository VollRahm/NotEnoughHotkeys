using NotEnoughHotkeys.RawInputLib;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static NotEnoughHotkeys.RawInputLib.Native;

namespace NotEnoughHotkeys.RawInputLib
{
    public class RawInput : Form
    {
        private KeyboardHandler KbdHandler;
        public delegate void RawInputHandler(object sender, RawKeyPressEventArgs e);
        public event RawInputHandler RawKeyPressEvent;

        public RawInput()
        {
            KbdHandler = new KeyboardHandler(Handle);
        }

        public void Start()
        {
            KbdHandler.LoadDevices();
            KbdHandler.RegisterNotifications();
        }


        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_INPUT)
            {
                HandleInput(m.LParam);
            }
            else if (m.Msg == WM_USB_DEVICECHANGE)
            {
                KbdHandler.LoadDevices();
            }
            base.WndProc(ref m);
        }

        private void HandleInput(IntPtr hDevice)
        {
            if (KbdHandler.Devices.Count == 0) return;
            RAWINPUT input = new RAWINPUT();
            int outSize = 0;
            int size = Marshal.SizeOf(typeof(RAWINPUT));

            outSize = GetRawInputData(hDevice, RawInputCommand.Input, out input, ref size, Marshal.SizeOf(typeof(RAWINPUTHEADER)));
            if (outSize != -1)
            {
                int vkey = input.Data.Keyboard.VKey;
                int makecode = input.Data.Keyboard.MakeCode;
                int flags = input.Data.Keyboard.Flags;

                if (vkey == KEYBOARD_OVERRUN_MAKE_CODE) return;
                
                RawKeyPressEventArgs rawEvent = new RawKeyPressEventArgs();
                if (KbdHandler.Devices.ContainsKey(input.Header.hDevice))
                {
                    lock (KbdHandler._lockObj)
                    {
                        rawEvent.Keyboard = KbdHandler.Devices[input.Header.hDevice];
                    }
                }
                else return;

                rawEvent.KeyState = ((flags & RI_KEY_BREAK) != 0) ? KeyPressState.Up : KeyPressState.Down;
                rawEvent.VKey = vkey;

                RawKeyPressEvent?.Invoke(this, rawEvent);
            }
        }

        ~RawInput()
        {
            KbdHandler.UnregisterNotifications();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // RawInput
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Name = "RawInput";
            this.ResumeLayout(false);

        }
    }
}
