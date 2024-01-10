using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Loop.Revit.Utilities;
using Loop.Revit.Utilities.UserSettings;
using Loop.Revit.Utilities.Wpf.SmallDialog;
using System.Text.Json;
using System.Windows;
using MaterialDesignThemes.Wpf;

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
        public RelayCommand<Window> ExportSettings { get; set; }
        public RelayCommand ClearSettings { get; set; }

        #endregion

        public RelayCommand SaveSettings { get; set; }

        public SettingsViewModel(SettingsModel model)
        {
            Model = model;

            TemporarySettings = DuplicateGlobalSetting(GlobalSettings.Settings);



            ChangeTheme = new RelayCommand(OnChangeTheme);
            ToggleThemeCommand = new RelayCommand(() => IsDarkMode = !IsDarkMode);

            ImportSettings = new RelayCommand(OnImportSettings);
            ExportSettings = new RelayCommand<Window>(OnExportSettings);
            ClearSettings = new RelayCommand(OnClearSettings);
            SaveSettings = new RelayCommand(OnSaveSettings);
        }

        private UserSetting DuplicateGlobalSetting(UserSetting setting)
        {
            string jsonString = JsonSerializer.Serialize(setting);
            return JsonSerializer.Deserialize<UserSetting>(jsonString);
        }

        private void OnSaveSettings()
        {
            //GlobalSettings.Settings.Age = 12345;
            //UserSettingsManager.SaveSettings(GlobalSettings.Settings);
            TemporarySettings.Age = 451165;

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

        private void OnExportSettings(Window win)
        {
            // TODO add success messages and error handling
            
            if (!TemporarySettings.Equals(GlobalSettings.Settings))
            {
                var dialogResults = SmallDialog.Create(
                    title: "Settings Not Saved!",
                    message:
                    "Changes to the current settings have not been saved, would you like to save them, or discard them before exporting?",
                    button1: new SdButton("Save", SmallDialogResults.Yes),
                    button2: new SdButton("Discard", SmallDialogResults.No),
                    darkMode: IsDarkMode,
                    iconKind: PackIconKind.AlertBoxOutline,
                    owner:win
                    );

                if (dialogResults == SmallDialogResults.Yes)
                {
                    UserSettingsManager.SaveSettings(TemporarySettings);
                }
                else
                {
                    SmallDialog.Create(
                        title: "Settings Discarded",
                        message:
                        "Temporary Settings have been reset to the last saved state.",
                        button1: new SdButton("OK", SmallDialogResults.Yes),
                        darkMode: IsDarkMode,
                        owner: win
                        );
                    TemporarySettings = DuplicateGlobalSetting(GlobalSettings.Settings);
                }

               
            }

            


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
