# NotEnoughHotkeys
This is an Application to block input from a secondary keyboard and instead run custom defined Macros. Currently it is in its early shoes and barely functional

# Building
Here are the steps to build the project:
1. Set the Target Platform to x64.
2. Build the `NotEnoughHotkeys` Project
3. Build the `NEHSubprocess` twice, one time with `Constants.NEHHOOKDLL` set to `NEHKbdhook.dll` and one time with it set to `NEHKbdhookAdmin.dll`. Rename the Administrator one to `NEHSubprocessAdmin.exe`
4. Place both Subprocess Executables into a `bin` folder which is in the same path as the `NotEnoughHotkeys.exe`.
5. Build the `NEH_KbdHook` Project and place the Dll files twice into the `bin` folder, one as `NEHKbdhook.dll` and the other as `NEHKbdhookAdmin.dll`
You can check the [newest release](https://github.com/VollRahm/NotEnoughHotkeys/releases/latest) to see the folder structure.

### Some notes
Icon made by [Freepik](https://www.flaticon.com/authors/freepik) from www.flaticon.com </br>
Icon composition made by [networkException](https://github.com/networkException), thanx for that

Good article on the topic: https://www.codeproject.com/Articles/716591/Combining-Raw-Input-and-keyboard-Hook-to-selective
Raw Input Library: https://www.codeproject.com/Articles/17123/Using-Raw-Input-from-C-to-handle-multiple-keyboard
