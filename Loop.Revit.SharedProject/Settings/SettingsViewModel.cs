using System;
using System.Numerics;
using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Loop.Revit.Utilities;
using Loop.Revit.Utilities.UserSettings;
using Loop.Revit.Utilities.Wpf.SmallDialog;
using System.Text.Json;
using System.Windows;
using Loop.Revit.Utilities.Wpf;
using MaterialDesignThemes.Wpf;
using System.Windows.Media;
using Loop.Revit.Utilities.Json;
using MaterialDesignColors;
using Color = System.Windows.Media.Color;
using Loop.Revit.Utilities.Wpf.WindowServices;

namespace Loop.Revit.Settings
{
    public class SettingsViewModel: ObservableObject
    {
        private readonly IWindowService _windowService;
        
        private Color _selectedColor;
        public Color SelectedColor
        {
            get => _selectedColor;
            set
            {
                SetProperty(ref _selectedColor, value);
                ChangePrimaryColour(value);
            }
        }

        private bool _overlayVisibility;

        public bool OverlayVisibility
        {
            get => _overlayVisibility;
            set => SetProperty(ref _overlayVisibility, value);
        }

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

        public RelayCommand CloseCommand { get; set; }

        public RelayCommand SaveSettings { get; set; }

        public SettingsViewModel(SettingsModel model, IWindowService windowService)
        {
            Model = model;
            _windowService = windowService;
            

            TemporarySettings = DuplicateGlobalSetting(GlobalSettings.Settings);
            LoadSettings();


            CloseCommand = new RelayCommand(CloseWindow);
            ChangeTheme = new RelayCommand(OnChangeTheme);
            ToggleThemeCommand = new RelayCommand(ToggleDarkMode);
            ImportSettings = new RelayCommand(OnImportSettings);
            ExportSettings = new RelayCommand(OnExportSettings);
            ClearSettings = new RelayCommand(OnClearSettings);
            SaveSettings = new RelayCommand(OnSaveSettings);
        }

        private void LoadSettings()
        {
            IsDarkMode = TemporarySettings.IsDarkModeTheme;
            _windowService.ToggleDarkMode(IsDarkMode);
            SelectedColor = TemporarySettings.PrimaryThemeColor;
            ChangePrimaryColour(SelectedColor);
        }

        private void ToggleDarkMode()
        {
            IsDarkMode = !IsDarkMode;
            TemporarySettings.IsDarkModeTheme = IsDarkMode;
            _windowService.ToggleDarkMode(IsDarkMode);

        }

        private void ChangePrimaryColour(Color color)
        {
            TemporarySettings.PrimaryThemeColor = color;
            var theme = _windowService.GetMaterialDesignTheme();
            theme.SetPrimaryColor(color);
            _windowService.SetMaterialDesignTheme(theme);

        }

        private void CloseWindow()
        {
            CheckUserWantsTempSettings(
                "Changes to the current settings have not been saved, would you like to save them, or discard them before closing?");
            _windowService.CloseWindow();
     
        }

        private UserSetting DuplicateGlobalSetting(UserSetting setting)
        {
            string jsonString = JsonSerializer.Serialize(setting);
            return JsonSerializer.Deserialize<UserSetting>(jsonString);
        }

        private void OnSaveSettings()
        {
            var settingsToSave = DuplicateGlobalSetting(TemporarySettings);
            UserSettingsManager.SaveSettings(settingsToSave);
            //TemporarySettings = DuplicateGlobalSetting(GlobalSettings.Settings);



        }

        private void OnImportSettings()
        {
            // TODO add success messages and error handling
            var filePath = DialogUtils.SelectSingleFile("JSON files|*.json", "json");
            if (!string.IsNullOrEmpty(filePath))
            {
                var settings = UserSettingsManager.LoadSettings(filePath);
                UserSettingsManager.SaveSettings(settings);

                var dupSettings = DuplicateGlobalSetting(settings);
                TemporarySettings = dupSettings;
                LoadSettings();

            }
        }


        private void CheckUserWantsTempSettings(string mainMessage)
        {
            // TODO add success messages and error handling

            var win = _windowService.GetWindow();
            var theme = _windowService.GetMaterialDesignTheme();

            if (!TemporarySettings.Equals(GlobalSettings.Settings))
            {
                OverlayVisibility = true;
                var dialogResults = SmallDialog.Create(
                    title: "Settings Not Saved!",
                    message: mainMessage,
                    button1: new SdButton("Save", SmallDialogResults.Yes),
                    button2: new SdButton("Discard", SmallDialogResults.No),
                    iconKind: PackIconKind.AlertBoxOutline,
                    theme: theme,
                    owner: win
                );

                if (dialogResults == SmallDialogResults.Yes)
                {
                    UserSettingsManager.SaveSettings(TemporarySettings);
                    TemporarySettings = DuplicateGlobalSetting(GlobalSettings.Settings);
                }
                else
                {
                    SmallDialog.Create(
                        title: "Settings Discarded",
                        message:
                        "Temporary Settings have been reset to the last saved state.",
                        button1: new SdButton("OK", SmallDialogResults.Yes),
                        theme: theme,
                        owner: win
                    );
                    TemporarySettings = DuplicateGlobalSetting(GlobalSettings.Settings);
                }
                OverlayVisibility = false;



            }


        }

        private void OnExportSettings()
        {
            CheckUserWantsTempSettings("Changes to the current settings have not been saved, would you like to save them, or discard them before exporting?");

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
            TemporarySettings = DuplicateGlobalSetting(GlobalSettings.Settings);
            LoadSettings();
        }

        private void OnChangeTheme()
        {
            Model.ChangeTheme();
        }
    }
}
