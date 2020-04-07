using NotEnoughHotkeys.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using Newtonsoft.Json.Serialization;

namespace NotEnoughHotkeys.Data
{
    public class Config
    {
        public Keyboard TargetKeyboard = new Keyboard() { HWID = "" }; //because of nullpointerexception
    }


    public static class ConfigManager
    {
        public static void StoreObject(object data, string path)
        {
            try
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects, TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple }));
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error while saving data. Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static T LoadFromFile<T>(string path, object standardValue = null)
        {
            try
            {
                if (!File.Exists(path)) return (T)standardValue;
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(path), new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while loading data. Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return (T)standardValue;
            }
        }

        public static void InitPaths()
        {
            string basePath;
            if (Environment.GetCommandLineArgs().Contains("--portable"))
            {
                basePath = "configs\\";
            }
            else
            {
                basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NotEnoughHotkeys\\configs\\");
            }
            if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);

            Constants.ConfigPath = Path.Combine(basePath, "config.json");
            Constants.MacrosPath = Path.Combine(basePath, "macros.json");
        }
    }
}
