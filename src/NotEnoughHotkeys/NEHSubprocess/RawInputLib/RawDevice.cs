using System;
using static NotEnoughHotkeys.RawInputLib.Native;

namespace NotEnoughHotkeys.RawInputLib
{
    public class RawDevice
    {
        public string HWID { get; set; }
        public IntPtr Handle { get; set; }
        public RawInputDeviceType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public string KeyboardLayout { get; set; }
    }
}
