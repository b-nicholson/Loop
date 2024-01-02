using System.IO;
using System.Reflection;
using System.Text.Json;

namespace Loop.Revit.Utilities.UserSettings
{
    public class UserSettingsManager
    {
        private static string _defaultFilePath { get; set; }

        // Save settings to a JSON file
        public static void SaveSettings(UserSetting settings, string filePath = null)
        {
            if (filePath == null)
            {
                SetDefaultFilePath();
                filePath = _defaultFilePath;
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string jsonString = JsonSerializer.Serialize(settings, options);
            File.WriteAllText(filePath, jsonString);
        }

        // Load settings from a JSON file
        public static UserSetting LoadSettings(string filePath = null)
        {
            if (filePath == null)
            {
                SetDefaultFilePath();
                filePath = _defaultFilePath;
            }
            if (!File.Exists(filePath))
            {
                var newSetting = new UserSetting();
                SaveSettings(newSetting, filePath);
                return newSetting; // Return new instance with default settings if file does not exist
            }

            string jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<UserSetting>(jsonString);
        }

        public static void ExportSettings(UserSetting settings, string exportFilePath)
        {
            SaveSettings(settings, exportFilePath);
        }

        // Import settings from a specified file path
        public static UserSetting ImportSettings(string importFilePath)
        {
            return LoadSettings(importFilePath);
        }
        public static void DeleteSettings(string filePath = null)
        {
            if (filePath == null)
            {
                SetDefaultFilePath();
                filePath = _defaultFilePath;
            }
            // Check if the file exists before trying to delete
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private static void SetDefaultFilePath()
        {
            _defaultFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Settings.json";
       
        }
    }
}
