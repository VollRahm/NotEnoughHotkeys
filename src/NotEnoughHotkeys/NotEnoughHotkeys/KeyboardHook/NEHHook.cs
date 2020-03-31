using NotEnoughHotkeys.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NotEnoughHotkeys.KeyboardHook
{
    public static class NEHHook
    {
        [DllImport(Constants.NEHHookDLL)]
        public static extern bool StartHook(IntPtr hWnd);

        [DllImport(Constants.NEHHookDLL)]
        public static extern bool StopHook();
    }
}
