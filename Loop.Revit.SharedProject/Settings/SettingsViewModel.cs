using System;
using System.Collections.Generic;
using System.Text;
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

        public SettingsViewModel(SettingsModel model)
        {
            Model = model;


            var loadedSettings = UserSettingsManager.LoadSettings();


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



        }

        private void OnImportSettings()
        {
            // TODO add success messages and error handling
            var filePath = DialogUtils.SelectSingleFile("JSON files|*.json", "json");

            var settings = UserSettingsManager.LoadSettings(filePath);
            if (settings != null)
            {
                UserSettingsManager.SaveSettings(settings);
            }

        }

        private void OnExportSettings()
        {

        }

        private void OnClearSettings()
        {

        }

        private void OnChangeTheme()
        {
            Model.ChangeTheme();
        }
    }
}
