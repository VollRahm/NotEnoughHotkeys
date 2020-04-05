using NotEnoughHotkeys.SubprocessAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NotEnoughHotkeys.Data.Types.Actions
{
    public class LaunchProcessMacro : IMacroAction
    {
        public string Name { get; set; }
        public string TypeName { get => "Launch Process"; }
        public ProcessStartInfo ProcessInfo { get; set; }
        public bool LaunchAsAdmin { get; set; }
        

        public LaunchProcessMacro(string name, ProcessStartInfo proc, bool asAdmin)
        {
            Name = name;
            ProcessInfo = proc;
            ProcessInfo.UseShellExecute = true;

            var desiredPath = ProcessInfo.WorkingDirectory;
            try
            {
                DirectoryInfo directory = new DirectoryInfo(desiredPath);
                if (Directory.Exists(directory.FullName))
                    ProcessInfo.WorkingDirectory = directory.FullName;
                else if (File.Exists(directory.FullName))
                    ProcessInfo.WorkingDirectory = new FileInfo(directory.FullName).DirectoryName;
            }
            catch { }

            LaunchAsAdmin = asAdmin;
        }

        public async Task PerformAsync()
        {
            try
            {
                bool IsAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
                if (LaunchAsAdmin && IsAdmin)
                {
                    Process.Start(ProcessInfo);
                }
                else
                {
                    if (IsAdmin) ProcessLauncher.ExecuteProcessUnElevated(ProcessInfo.FileName, ProcessInfo.Arguments, ProcessInfo.WorkingDirectory);
                    else Process.Start(ProcessInfo);
                }
            }catch(Exception ex)
            {
                MessageBox.Show($"Could not Launch Process on Macro {Name}. Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            await Task.Delay(0);
        }

    }
}
