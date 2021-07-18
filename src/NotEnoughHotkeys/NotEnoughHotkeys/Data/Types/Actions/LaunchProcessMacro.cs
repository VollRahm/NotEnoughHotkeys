using Newtonsoft.Json;
using NotEnoughHotkeys.SubprocessAPI;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;

namespace NotEnoughHotkeys.Data.Types.Actions
{
    public class LaunchProcessMacro : IMacroAction
    {
        public string Name { get; set; }
        public string TypeName { get => "Launch Process"; }
        
        [JsonIgnore]
        public ProcessStartInfo ProcessInfo { get; set; }
        public string ProcessFileName { get; set; }
        public string ProcessArgs { get; set; }
        public string ProcessStartPath { get; set; }
        public bool LaunchAsAdmin { get; set; }
        

        public LaunchProcessMacro(string name, string procPath, string procArgs, string procStartPath, bool asAdmin)
        {
            Name = name;
            ProcessFileName = procPath;
            ProcessArgs = procArgs;
            ProcessStartPath = procStartPath;

            var desiredPath = ProcessStartPath;
            try
            {
                if (string.IsNullOrEmpty(desiredPath))
                {
                    desiredPath = ProcessFileName;
                }
                DirectoryInfo directory = new DirectoryInfo(desiredPath);
                if (Directory.Exists(directory.FullName))
                    ProcessStartPath = directory.FullName;
                else if (File.Exists(directory.FullName))
                    ProcessStartPath = new FileInfo(directory.FullName).DirectoryName;
            }
            catch { }

            LaunchAsAdmin = asAdmin;
        }

        public async Task PerformAsync()
        {
            ProcessInfo = new ProcessStartInfo(ProcessFileName, ProcessArgs) { WorkingDirectory = ProcessStartPath };
            ProcessInfo.UseShellExecute = true;
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
