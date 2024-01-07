using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Autodesk.Revit.DB;


namespace Loop.Revit.Utilities.UserSettings
{
    public class UserSettingsManager
    {
        private static string DefaultFilePath { get; set; }

        // Save settings to a JSON file
        public static void SaveSettings(UserSetting settings, string filePath = null)
        {
            if (filePath == null)
            {
                SetDefaultFilePath();
                filePath = DefaultFilePath;
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string jsonString = JsonSerializer.Serialize(settings, options);
            File.WriteAllText(filePath, jsonString);

            GlobalSettings.Settings = settings;
        }


        // Load settings from a JSON file
        public static UserSetting LoadSettings(string filePath = null)
        {
            if (filePath == null)
            {
                SetDefaultFilePath();
                filePath = DefaultFilePath;
            }
            if (!File.Exists(filePath))
            {
                var newSetting = new UserSetting();
                SaveSettings(newSetting, filePath);
                return newSetting; // Return new instance with default settings if file does not exist
            }

            string jsonString = File.ReadAllText(filePath);
            var setting = JsonSerializer.Deserialize<UserSetting>(jsonString);
            return setting;
        }

        public static void ExportSettings(UserSetting settings, string exportFilePath)
        {
            SaveSettings(settings, exportFilePath);
        }

        // Import settings from a specified file path
        public static UserSetting ImportSettings(string importFilePath)
        {
            var setting = LoadSettings(importFilePath);
            return setting;
        }
        public static void DeleteSettings(string filePath = null)
        {
            if (filePath == null)
            {
                SetDefaultFilePath();
                filePath = DefaultFilePath;
            }
            // Check if the file exists before trying to delete
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private static void SetDefaultFilePath()
        {
            DefaultFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Settings.json";
       
        }
    }
}
