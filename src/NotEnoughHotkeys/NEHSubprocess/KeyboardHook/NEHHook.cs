using NEHSubprocess;
using NEHSubprocess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NEHSubprocess
{
    public static class NEHHook
    {
        [DllImport(Constants.NEHHOOKDLL)]
        public static extern bool StartHook(IntPtr hWnd);

        [DllImport(Constants.NEHHOOKDLL)]
        public static extern bool StopHook();
    }
}
