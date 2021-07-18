# NotEnoughHotkeys
This is an Application to block input from a secondary keyboard and instead run custom defined Macros. Currently its ready for daily use. Its an alternative to LuaMacros. [Newest Release](https://github.com/VollRahm/NotEnoughHotkeys/releases/latest)

# Features
* ✔ &ensp; Distinguishing between keyboards
* ✔ &ensp; Blocking input from certain keyboard
* ✔ &ensp; GUI for creating custom macros to run apps or send other keystrokes
* ✔ &ensp; Admin process support
* ✔ &ensp; Enabling and disabling the hook
* ✔ &ensp; Saving macros  
</br>

* ➕ &ensp; Creating macros for more than one extra keyboard
* ➕ &ensp; Support for modifier keys (Ctrl, Shift, Alt)
* ➕ &ensp; Support for double taps
* ➕ &ensp; Maybe your ideas! Submit them as issues with the idea tag!  
</br>

* ❌&ensp;  Blocking Windows Key, Alt+Tab and State Keys (Caps Lock, Num Lock) because those are sent before RawInput gets anything

✔ &ensp; = Implemented, ➕&ensp;  = Scheduled implementation, ❌&ensp;  = Most Likely not implementable

## Building
Here are the steps to build the project:
1. Set the Target Platform to x64.
2. Build the `NotEnoughHotkeys` Project
3. Build the `NEHSubprocess` twice, one time with `Constants.NEHHOOKDLL` set to `NEHKbdhook.dll` and one time with it set to `NEHKbdhookAdmin.dll`. Rename the Administrator one to `NEHSubprocessAdmin.exe`
4. Place both Subprocess Executables into a `bin` folder which is in the same path as the `NotEnoughHotkeys.exe`.
5. Build the `NEH_KbdHook` Project and place the Dll files twice into the `bin` folder, one as `NEHKbdhook.dll` and the other as `NEHKbdhookAdmin.dll`
You can check the [newest release](https://github.com/VollRahm/NotEnoughHotkeys/releases/latest) to see the folder structure.

## Known Issues
- Keystrokes are not blocked in Windows Search or in the Windows Explorer search textbox only.
- Applications that use Global-Keyboard-Hooks for their Hotkeys still get input. The workaround is to assign Keys like F24 to your hotkey and then send F24 over NotEnoughHotkeys
- Windows Key, Alt+Tab and State Keys (Caps Lock, Num Lock) cannot be blocked.
- Games hook the keyboard on a lower level, so the keys won't be blocked in games.

### Some notes
Icon made by [Freepik](https://www.flaticon.com/authors/freepik) from www.flaticon.com </br>
Icon composition made by [networkException](https://github.com/networkException), thanx for that

Good article on the topic: https://www.codeproject.com/Articles/716591/Combining-Raw-Input-and-keyboard-Hook-to-selective </br>
More references:  
https://www.codeproject.com/Articles/17123/Using-Raw-Input-from-C-to-handle-multiple-keyboard
