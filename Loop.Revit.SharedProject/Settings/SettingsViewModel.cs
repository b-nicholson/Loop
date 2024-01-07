using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Loop.Revit.Utilities;
using Loop.Revit.Utilities.UserSettings;

namespace Loop.Revit.Settings
{
    public class SettingsViewModel: ObservableObject
    {
        #region Common Properties
        public SettingsModel Model { get; set; }

        private UserSetting _tempSetting;

        public UserSetting TemporarySettings
        {
            get => _tempSetting;
            set => SetProperty(ref _tempSetting, value);
        }
        

        #endregion

        #region Theme Settings Properties

        private bool _isDarkMode;

        public bool IsDarkMode
        {
            get => _isDarkMode;
            set => SetProperty(ref _isDarkMode, value);
        }
        
        public RelayCommand ChangeTheme { get; set; }
        public RelayCommand ToggleThemeCommand { get; }

        #endregion

        #region Revit Version Properties
        private int _rvtYear;
        public int RvtYear
        {
            get => _rvtYear;
            set => SetProperty(ref _rvtYear, value);
        }
        #endregion

        #region Import/Export Properties
        public RelayCommand ImportSettings { get; set; }
        public RelayCommand ExportSettings { get; set; }
        public RelayCommand ClearSettings { get; set; }

        #endregion

        public RelayCommand SaveSettings { get; set; }

        public SettingsViewModel(SettingsModel model)
        {
            Model = model;


            TemporarySettings = GlobalSettings.Settings;


            //loadedSettings.Username = "JohnDoe";
            //loadedSettings.Age = 30;
            //loadedSettings.IsSubscribed = true;
            //UserSettingsManager.SaveSettings(loadedSettings);

            //UserSettingsManager.DeleteSettings();






            ChangeTheme = new RelayCommand(OnChangeTheme);
            ToggleThemeCommand = new RelayCommand(() => IsDarkMode = !IsDarkMode);

            ImportSettings = new RelayCommand(OnImportSettings);
            ExportSettings = new RelayCommand(OnExportSettings);
            ClearSettings = new RelayCommand(OnClearSettings);
            SaveSettings = new RelayCommand(OnSaveSettings);
        }

        private void OnSaveSettings()
        {
            GlobalSettings.Settings.Age = 12345;
            UserSettingsManager.SaveSettings(GlobalSettings.Settings);

        }

        private void OnImportSettings()
        {
            // TODO add success messages and error handling
            var filePath = DialogUtils.SelectSingleFile("JSON files|*.json", "json");
            if (!string.IsNullOrEmpty(filePath))
            {
                var settings = UserSettingsManager.LoadSettings(filePath);
                UserSettingsManager.SaveSettings(settings);
             
            }
        }

        private void OnExportSettings()
        {
            // TODO add success messages and error handling
            var settings = UserSettingsManager.LoadSettings();
            var newPath = DialogUtils.SaveSingleFile("JSON files|*.json", "json");
            if (!string.IsNullOrEmpty(newPath))
            {
                UserSettingsManager.ExportSettings(settings, newPath);
            }
        }

        private void OnClearSettings()
        {
            // TODO add success messages and error handling
            UserSettingsManager.DeleteSettings();
            UserSettingsManager.LoadSettings();
        }

        private void OnChangeTheme()
        {
            Model.ChangeTheme();
        }
    }
}
