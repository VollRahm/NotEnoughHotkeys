using System;
using System.Runtime.InteropServices;

namespace NotEnoughHotkeys.RawInputLib
{
    public static class Native
    {
        [DllImport("user32.dll")]
        public static extern bool RegisterRawInputDevices([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] RAWINPUTDEVICE[] pRawInputDevices, int uiNumDevices, int cbSize);

        public struct RAWINPUTDEVICE
        {
            /// <summary>Top level collection Usage page for the raw input device.</summary>
            public HIDUsagePage UsagePage;
            /// <summary>Top level collection Usage for the raw input device. </summary>
            public HIDUsage Usage;
            /// <summary>Mode flag that specifies how to interpret the information provided by UsagePage and Usage.</summary>
            public RawInputDeviceFlags Flags;
            /// <summary>Handle to the target device. If NULL, it follows the keyboard focus.</summary>
            public IntPtr WindowHandle;
        }

        public enum HIDUsagePage : ushort
        {
            /// <summary>Generic desktop controls.</summary>
            Generic = 0x01
        }

        public enum HIDUsage : ushort
        {
            Keyboard = 0x06,
        }

        [Flags()]
        public enum RawInputDeviceFlags
        {
            /// <summary>No flags.</summary>
            None = 0,
            /// <summary>If set, this removes the top level collection from the inclusion list. This tells the operating system to stop reading from a device which matches the top level collection.</summary>
            Remove = 0x00000001,
            /// <summary>If set, this specifies the top level collections to exclude when reading a complete usage page. This flag only affects a TLC whose usage page is already specified with PageOnly.</summary>
            Exclude = 0x00000010,
            /// <summary>If set, this specifies all devices whose top level collection is from the specified usUsagePage. Note that Usage must be zero. To exclude a particular top level collection, use Exclude.</summary>
            PageOnly = 0x00000020,
            /// <summary>If set, this prevents any devices specified by UsagePage or Usage from generating legacy messages. This is only for the mouse and keyboard.</summary>
            NoLegacy = 0x00000030,
            /// <summary>If set, this enables the caller to receive the input even when the caller is not in the foreground. Note that WindowHandle must be specified.</summary>
            InputSink = 0x00000100,
            /// <summary>If set, the mouse button click does not activate the other window.</summary>
            CaptureMouse = 0x00000200,
            /// <summary>If set, the application-defined keyboard device hotkeys are not handled. However, the system hotkeys; for example, ALT+TAB and CTRL+ALT+DEL, are still handled. By default, all keyboard hotkeys are handled. NoHotKeys can be specified even if NoLegacy is not specified and WindowHandle is NULL.</summary>
            NoHotKeys = 0x00000200,
            /// <summary>If set, application keys are handled.  NoLegacy must be specified.  Keyboard only.</summary>
            AppKeys = 0x00000400,
            /// <summary>If set, this enables the caller to receive WM_INPUT_DEVICE_CHANGE notifications for device arrival and device removal.</summary>
            DevNotify =   0x00002000
        }

        public enum RawInputDeviceType : uint
        {
            MOUSE = 0,
            KEYBOARD = 1,
            HID = 2
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTDEVICELIST
        {
            public IntPtr hDevice;
            public RawInputDeviceType Type;
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint GetRawInputDeviceList
        (
            [In, Out] RAWINPUTDEVICELIST[] RawInputDeviceList,
            ref uint NumDevices,
            uint Size
        );

        [DllImport("user32.dll")]
        public static extern uint GetRawInputDeviceInfo(IntPtr deviceHandle, uint command, IntPtr data, ref uint dataSize);

        public enum RawInputDeviceInfo
        {
            DeviceName = 0x20000007
        }

        public const uint DEVICE_NOTIFY_WINDOW_HANDLE = 0x0;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, uint Flags);

        public const int DBT_DEVTYP_DEVICEINTERFACE = 0x5;

        public static readonly Guid GUID_DEVINTERFACE_HID = new Guid("4D1E55B2-F16F-11CF-88CB-001111000030");

        public struct _DEV_BROADCAST_HDR
        {
            public int dbch_size;
            public int dbch_devicetype;
            public Guid dbcc_classguid;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterDeviceNotification(IntPtr handle);

        public const int WM_INPUT = 0xFF;
        public const int WM_USB_DEVICECHANGE = 0x219;

        [DllImport("user32.dll")]
        public static extern int GetRawInputData(IntPtr hRawInput, RawInputCommand uiCommand, out RAWINPUT pData, ref int pcbSize, int cbSizeHeader);

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUT
        {
            /// <summary>
            /// Header for the data.
            /// </summary>
            public RAWINPUTHEADER Header;
            public Union Data;
            [StructLayout(LayoutKind.Explicit)]
            public struct Union
            {
                /// <summary>
                /// Mouse raw input data.
                /// </summary>
                [FieldOffset(0)]
                public RAWMOUSE Mouse;
                /// <summary>
                /// Keyboard raw input data.
                /// </summary>
                [FieldOffset(0)]
                public RAWKEYBOARD Keyboard;
                /// <summary>
                /// HID raw input data.
                /// </summary>
                [FieldOffset(0)]
                public RAWHID HID;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct RAWMOUSE
        {
            [FieldOffset(0)]
            public ushort usFlags;
            [FieldOffset(4)]
            public uint ulButtons;
            [FieldOffset(4)]
            public ushort usButtonFlags;
            [FieldOffset(6)]
            public ushort usButtonData;
            [FieldOffset(8)]
            public uint ulRawButtons;
            [FieldOffset(12)]
            public int lLastX;
            [FieldOffset(16)]
            public int lLastY;
            [FieldOffset(20)]
            public uint ulExtraInformation;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWKEYBOARD
        {
            public ushort MakeCode;
            public ushort Flags;
            private readonly ushort Reserved;
            public ushort VKey;
            public uint Message;
            public ulong ExtraInformation;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWHID
        {
            public uint dwSizHid;
            public uint dwCount;
            public byte bRawData;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTHEADER
        {
            public uint dwType;
            public uint dwSize;
            public IntPtr hDevice;
            public IntPtr wParam;
        }

        public enum RawInputCommand
        {
            /// <summary>
            /// Get input data.
            /// </summary>
            Input = 0x10000003,
            /// <summary>
            /// Get header data.
            /// </summary>
            Header = 0x10000005
        }

        public const int KEYBOARD_OVERRUN_MAKE_CODE = 0xFF;
        public const int RI_KEY_BREAK = 0x01;
    }
}
