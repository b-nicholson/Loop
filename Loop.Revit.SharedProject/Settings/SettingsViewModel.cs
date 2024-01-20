using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Loop.Revit.Utilities;
using Loop.Revit.Utilities.UserSettings;
using Loop.Revit.Utilities.Wpf.SmallDialog;
using System.Text.Json;
using MaterialDesignThemes.Wpf;
using System.Windows.Media;
using Loop.Revit.Utilities.Wpf.WindowServices;


namespace Loop.Revit.Settings
{
    //TODO: long snackbar messages (the exception ones) need another interface. maybe use the button option to load a smalldialog. make the smalldialog a method.
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

        public SnackbarMessageQueue MessageQueue { get; } = new SnackbarMessageQueue();

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
            var saveResult = UserSettingsManager.SaveSettings(settingsToSave);
            if (saveResult.Success == false)
            {
                MessageQueue.Enqueue("❎ Save Failed: " + saveResult.Message);
                return;
            }
            MessageQueue.Enqueue("✅ Settings Saved");
        }

        private void OnImportSettings()
        { 
            var win = _windowService.GetWindow();
            var theme = _windowService.GetMaterialDesignTheme();
            var dialogResults = SmallDialog.Create(
                title: "Are You Sure?",
                message: "Importing settings will overwrite the current configuration, do you wish to proceed?",
                button1: new SdButton("No", SmallDialogResults.No),
                button2: new SdButton("Yes", SmallDialogResults.Yes),
                iconKind: PackIconKind.AlertBoxOutline,
                theme: theme,
                owner: win
            );
            if (dialogResults != SmallDialogResults.Yes)
            {
                MessageQueue.Enqueue("❎ Import Cancelled");
                return;
            }
            var filePath = DialogUtils.SelectSingleFile("JSON files|*.json", "json");
            if (string.IsNullOrEmpty(filePath))
            {
                MessageQueue.Enqueue("❎ Import Cancelled");
                return;
            }
            var settingsResult = UserSettingsManager.LoadSettings(filePath);
            if (settingsResult.Success == false)
            {
                MessageQueue.Enqueue("❎ Import Failed: " + settingsResult.Message);
                return;
            }
            var saveResults = UserSettingsManager.SaveSettings(settingsResult.ReturnObject);
            if (saveResults.Message == false)
            {
                MessageQueue.Enqueue("❎ Save Settings Failed: " + saveResults.Message);
                return;
            }
            var dupSettings = DuplicateGlobalSetting(settingsResult.ReturnObject);
            TemporarySettings = dupSettings;
            LoadSettings();
            MessageQueue.Enqueue("✅ Settings Imported");
        }


        private void CheckUserWantsTempSettings(string mainMessage)
        {
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

            var settingsResult = UserSettingsManager.LoadSettings();
            if (settingsResult.Success == false)
            {
                MessageQueue.Enqueue("❎ Failed to Load Settings: " + settingsResult.Message);
                return;
                
            }
            var newPath = DialogUtils.SaveSingleFile("JSON files|*.json", "json");
            if (!string.IsNullOrEmpty(newPath))
            {
                UserSettingsManager.ExportSettings(settingsResult.ReturnObject, newPath);
                MessageQueue.Enqueue("✅ Settings Exported");
            }
            else
            {
                MessageQueue.Enqueue("❎ Export Cancelled");
            }
        }

        private void OnClearSettings()
        {
            var win = _windowService.GetWindow();
            var theme = _windowService.GetMaterialDesignTheme();
            var dialogResults = SmallDialog.Create(
                title: "Are You Sure?",
                message: "Settings will be discarded, do you wish to proceed?",
                button1: new SdButton("No", SmallDialogResults.No),
                button2: new SdButton("Yes", SmallDialogResults.Yes),
                iconKind: PackIconKind.AlertBoxOutline,
                theme: theme,
                owner: win
            );

            if (dialogResults == SmallDialogResults.Yes)
            {
                var deleteResult = UserSettingsManager.DeleteSettings();
                if (deleteResult.Success == false)
                {
                    MessageQueue.Enqueue("❎ Settings Clear Failed: " + deleteResult.Message);
                }
                var loadResult = UserSettingsManager.LoadSettings();
                if (loadResult.Success == false)
                {
                    MessageQueue.Enqueue("❎ Creation of New Settings Failed: " + loadResult.Message);
                }

                TemporarySettings = DuplicateGlobalSetting(GlobalSettings.Settings);
                LoadSettings();
                MessageQueue.Enqueue("✅ Settings Cleared");
            }
            else
            {
                MessageQueue.Enqueue("❎ Settings Reset Cancelled");
            }

          
        }

        private void OnChangeTheme()
        {
            Model.ChangeTheme();
        }
    }
}
