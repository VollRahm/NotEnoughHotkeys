using NotEnoughHotkeys.RawInputLib;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static NotEnoughHotkeys.RawInputLib.Native;

namespace NotEnoughHotkeys.RawInputLib
{
    public class KeyboardHandler
    {
        public object _lockObj = new object();
        public Dictionary<IntPtr, RawDevice> Devices = new Dictionary<IntPtr, RawDevice>();
        private IntPtr notifHandle = IntPtr.Zero;
        private IntPtr Target { get; set; }

        public KeyboardHandler(IntPtr target)
        {
            Target = target;
            var devicesToRegister = CreateDevice(target);
            if (!RegisterRawInputDevices(devicesToRegister, devicesToRegister.Length, Marshal.SizeOf(typeof(RAWINPUTDEVICE))))
                throw new Exception("Could not register RawInputDevice! Error: " + Marshal.GetLastWin32Error());
        }

        public RAWINPUTDEVICE[] CreateDevice(IntPtr target)
        {
            var device = new RAWINPUTDEVICE[1];

            device[0].Usage = HIDUsage.Keyboard;
            device[0].UsagePage = HIDUsagePage.Generic;
            device[0].Flags = RawInputDeviceFlags.InputSink | RawInputDeviceFlags.DevNotify;
            device[0].WindowHandle = target;
            return device;
        }

        public void LoadDevices()
        {
            lock (_lockObj)
            {
                Devices.Clear();
                var currentKeyboard = 1;

                RawDevice nullKeyboard = new RawDevice()
                {
                    Handle = IntPtr.Zero,
                    HWID = "NULL",
                    Id = currentKeyboard,
                    Name = "Not identified keypress",
                    Type = RawInputDeviceType.KEYBOARD
                };

                Devices.Add(nullKeyboard.Handle, nullKeyboard);

                var structSize = Marshal.SizeOf(typeof(RAWINPUTDEVICELIST));

                var bufferCount = 0u;

                if(GetRawInputDeviceList(null, ref bufferCount, (uint)structSize) == 0)
                {
                    RAWINPUTDEVICELIST[] rawDeviceList = new RAWINPUTDEVICELIST[bufferCount];
                    GetRawInputDeviceList(rawDeviceList, ref bufferCount, (uint)structSize);

                    foreach(var device in rawDeviceList)
                    {
                        if(device.Type == RawInputDeviceType.KEYBOARD || device.Type == RawInputDeviceType.HID)
                        {
                            currentKeyboard++;
                            uint size = 0;
                            GetRawInputDeviceInfo(device.hDevice, (uint)RawInputDeviceInfo.DeviceName, IntPtr.Zero, ref size);
                            var data = Marshal.AllocHGlobal((int)size);
                            GetRawInputDeviceInfo(device.hDevice, (uint)RawInputDeviceInfo.DeviceName, data, ref size);

                            var HWID = Marshal.PtrToStringAnsi(data);

                            RawDevice rawDevice = new RawDevice()
                            {
                                HWID = HWID,
                                Handle = device.hDevice,
                                Id = currentKeyboard,
                                Type = device.Type,
                                Name = RawInputHelper.GetDeviceName(HWID)
                            };

                            if (!Devices.ContainsKey(device.hDevice))
                                Devices.Add(device.hDevice, rawDevice);

                            Marshal.FreeHGlobal(data);
                        }
                    }
                    return;
                }
            }
            throw new Exception("Error: " + Marshal.GetLastWin32Error());
        }

        public void RegisterNotifications()
        {
            var deviceInterface = new _DEV_BROADCAST_HDR();
            deviceInterface.dbch_size = Marshal.SizeOf(deviceInterface);
            deviceInterface.dbch_devicetype = DBT_DEVTYP_DEVICEINTERFACE;
            deviceInterface.dbcc_classguid = GUID_DEVINTERFACE_HID;

            var deviceInterfacePtr = IntPtr.Zero;
            try
            {
                deviceInterfacePtr = Marshal.AllocHGlobal(Marshal.SizeOf(deviceInterface));
                Marshal.StructureToPtr(deviceInterface, deviceInterfacePtr, false);
                notifHandle = RegisterDeviceNotification(Target, deviceInterfacePtr, DEVICE_NOTIFY_WINDOW_HANDLE);
            }
            catch
            {
                throw new Exception("Error: " + Marshal.GetLastWin32Error());
            }

            Marshal.FreeHGlobal(deviceInterfacePtr);

            if (notifHandle == IntPtr.Zero) throw new Exception("Failed registering for notifications!");
        }

        public void UnregisterNotifications()
        {
            UnregisterDeviceNotification(Target);
        }
    }
}
