﻿using System;

namespace RawInput_dll
{
    public class RawInputEventArg : EventArgs
    {
        public RawInputEventArg(KeyPressEvent arg)
        {
            KeyPressEvent = arg;
        }
        
        public KeyPressEvent KeyPressEvent { get; private set; }
    }
}
