using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotEnoughHotkeys.Data
{
    public static class Constants
    {
        public const string NEHHookDLL = "NEHKBDHOOK.dll";

        public const uint WH_HOOK = 32769;

        public const string KEYDOWN = "MAKE";
        public const string KEYUP = "BREAK";
    }
}
