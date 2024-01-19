using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Autodesk.Revit.DB;
using Loop.Revit.Utilities.Json;


namespace Loop.Revit.Utilities.UserSettings
{
    public class UserSettingsManager
    {
        private static string DefaultFilePath { get; set; }

        // Save settings to a JSON file
        public static OperationResult SaveSettings(UserSetting settings, string filePath = null)
        {
            var results = new OperationResult();

            if (filePath == null)
            {
                SetDefaultFilePath();
                filePath = DefaultFilePath;
            }

            var options = new JsonSerializerOptions
            {
                Converters = { new JsonColorConverter() }
            };
            try
            {
                string jsonString = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(filePath, jsonString);
                GlobalSettings.Settings = settings;
                results.Success = true;
            }
            catch (Exception e)
            {
                results.Exception = e;
                results.Message = e.Message;
            }

            return results;
        }


        // Load settings from a JSON file
        public static OperationResult LoadSettings(string filePath = null)
        {
            var results = new OperationResult();

            if (filePath == null)
            {
                SetDefaultFilePath();
                filePath = DefaultFilePath;
            }
            if (!File.Exists(filePath))
            {
                var newSetting = new UserSetting();
                SaveSettings(newSetting, filePath);

                results.Success = true;
                results.Message = "Settings Successfully Created";
                results.ReturnObject = newSetting;

                return results; // Return new instance with default settings if file does not exist
            }

            var options = new JsonSerializerOptions
            {
                Converters = { new JsonColorConverter() }
            };
            try
            {
                string jsonString = File.ReadAllText(filePath);
                var setting = JsonSerializer.Deserialize<UserSetting>(jsonString, options);

                results.Success = true;
                results.Message = "Settings Successfully Loaded";
                results.ReturnObject = setting;

            }
            catch (Exception e)
            {
                results.Exception = e;
                results.Message = e.Message;
            }
            
            return results;
        }

        public static OperationResult ExportSettings(UserSetting settings, string exportFilePath)
        {
            var saveResult = SaveSettings(settings, exportFilePath);
            return saveResult;
        }

        // Import settings from a specified file path
        public static OperationResult ImportSettings(string importFilePath)
        {
            var setting = LoadSettings(importFilePath);
            return setting;
        }
        public static OperationResult DeleteSettings(string filePath = null)
        {
            var results = new OperationResult();

            if (filePath == null)
            {
                SetDefaultFilePath();
                filePath = DefaultFilePath;
            }
            // Check if the file exists before trying to delete
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    results.Success = true;
                    results.Message = "Settings Successfully Deleted";
                }
                catch (Exception e)
                {
                    results.Exception = e;
                    results.Message = e.Message;
                }
            }

            return results;
        }

        private static void SetDefaultFilePath()
        {
            DefaultFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Settings.json";
       
        }
    }
}
