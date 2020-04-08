using Microsoft.Win32.TaskScheduler;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using static NotEnoughHotkeys.Data.Constants;

namespace NotEnoughHotkeys.Misc
{
    public static class AutoStart
    {
        public static void Toggle(bool Enable, bool AsAdmin)
        {
            var status = Status(out bool _asAdmin);
            if (Enable)
            {
                if (status) RemoveTask();
                AddTask(AsAdmin);
            }
            else
            {
                if (!status) return;
                RemoveTask();
            }
        }

        public static bool Status(out bool asAdmin)
        {
            using(var ts = new TaskService())
            {
                var task = ts.GetTask(StartupTaskName);
                if (task == null)
                {
                    asAdmin = false;
                    return false;
                }
                if(task.Definition.Principal.RunLevel == TaskRunLevel.Highest)
                    asAdmin = true;
                else
                    asAdmin = false;
                return true;
            }
        }

        private static void AddTask(bool AsAdmin)
        {
            try
            {
                using (var ts = new TaskService())
                {
                    var task = ts.GetTask(StartupTaskName);
                    if (task != null)
                    {
                        return;
                    }
                    var newTask = TaskService.Instance.NewTask();
                    if (AsAdmin)
                        newTask.Principal.RunLevel = TaskRunLevel.Highest;
                    else
                        newTask.Principal.RunLevel = TaskRunLevel.LUA;

                    newTask.RegistrationInfo.Author = "VollRahm";
                    newTask.RegistrationInfo.Date = DateTime.Now;
                    newTask.RegistrationInfo.Description = "Starts NotEnoughHotkeys";
                    var trigger = new LogonTrigger { Delay = new TimeSpan(0, 0, 0, 10) };
                    newTask.Triggers.Add(trigger);
                    newTask.Actions.Add(new ExecAction(Assembly.GetExecutingAssembly().Location, "--minimized", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)));
                    newTask.Settings.DisallowStartIfOnBatteries = false;
                    newTask.Settings.StopIfGoingOnBatteries = false;
                    TaskService.Instance.RootFolder.RegisterTaskDefinition(StartupTaskName, newTask);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to register startup task. Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void RemoveTask()
        {
            using (var ts = new TaskService())
            {
                var task = ts.GetTask(StartupTaskName);
                if (task != null)
                {
                    ts.RootFolder.DeleteTask(StartupTaskName, false);
                }
            }
        }
    }
}
