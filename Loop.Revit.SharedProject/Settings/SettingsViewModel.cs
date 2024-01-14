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
using MaterialDesignColors;
using Color = System.Windows.Media.Color;
using Loop.Revit.Utilities.Wpf.WindowServices;

namespace Loop.Revit.Settings
{
    public class SettingsViewModel: ObservableObject
    {
        private readonly IWindowService _windowService;
        public RelayCommand CloseCommand { get; set; }




















        private readonly PaletteHelper _paletteHelper = new PaletteHelper();

        private Color _primaryColor;
        private Color _secondaryColor;
        private Color _primaryForegroundColor;
        private Color _secondaryForegroundColor;


        private ColorScheme _activeScheme;
        public ColorScheme ActiveScheme
        {
            get => _activeScheme;
            set
            {
                if (_activeScheme != value)
                {
                    _activeScheme = value;
                    OnPropertyChanged();
                }
            }
        }

        private Color? _selectedColor;

        public Color? SelectedColor
        {
            get => _selectedColor;
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;
                    OnPropertyChanged();

                    // if we are triggering a change internally its a hue change and the colors will match
                    // so we don't want to trigger a custom color change.
                    var currentSchemeColor = default(Color); // or some other default initialization

                    switch (ActiveScheme)
                    {
                        case ColorScheme.Primary:
                            currentSchemeColor = _primaryColor;
                            break;
                        case ColorScheme.Secondary:
                            currentSchemeColor = _secondaryColor;
                            break;
                        case ColorScheme.PrimaryForeground:
                            currentSchemeColor = _primaryForegroundColor;
                            break;
                        case ColorScheme.SecondaryForeground:
                            currentSchemeColor = _secondaryForegroundColor;
                            break;
                        default:
                            throw new NotSupportedException(
                                $"{ActiveScheme} is not a handled ColorScheme. Ye daft programmer!");
                    }

                    if (_selectedColor != currentSchemeColor && value is Color color)
                    {
                        ChangeCustomColor(color);
                    }
                }
            }
        }


        private void ChangeCustomColor(object obj) {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            var color = (Color)obj;

            if (ActiveScheme == ColorScheme.Primary)
            {
                _paletteHelper.ChangePrimaryColor(color);
                _primaryColor = color;
            }
            else if (ActiveScheme == ColorScheme.Secondary)
            {
                _paletteHelper.ChangeSecondaryColor(color);
                _secondaryColor = color;
            }
            else if (ActiveScheme == ColorScheme.PrimaryForeground)
            {
                SetPrimaryForegroundToSingleColor(color);
                _primaryForegroundColor = color;
            }
            else if (ActiveScheme == ColorScheme.SecondaryForeground)
            {
                SetSecondaryForegroundToSingleColor(color);
                _secondaryForegroundColor = color;
            }
        }

        private void ChangeHue(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            var hue = (Color)obj;

            SelectedColor = hue;
            if (ActiveScheme == ColorScheme.Primary)
            {
                _paletteHelper.ChangePrimaryColor(hue);
                _primaryColor = hue;
                _primaryForegroundColor = _paletteHelper.GetTheme().PrimaryMid.GetForegroundColor();
            }
            else if (ActiveScheme == ColorScheme.Secondary)
            {
                _paletteHelper.ChangeSecondaryColor(hue);
                _secondaryColor = hue;
                _secondaryForegroundColor = _paletteHelper.GetTheme().SecondaryMid.GetForegroundColor();
            }
            else if (ActiveScheme == ColorScheme.PrimaryForeground)
            {
                SetPrimaryForegroundToSingleColor(hue);
                _primaryForegroundColor = hue;
            }
            else if (ActiveScheme == ColorScheme.SecondaryForeground)
            {
                SetSecondaryForegroundToSingleColor(hue);
                _secondaryForegroundColor = hue;
            }
        }

        private void SetPrimaryForegroundToSingleColor(Color color)
        {
            var theme = _paletteHelper.GetTheme();

            theme.PrimaryLight = new ColorPair(theme.PrimaryLight.Color, color);
            theme.PrimaryMid = new ColorPair(theme.PrimaryMid.Color, color);
            theme.PrimaryDark = new ColorPair(theme.PrimaryDark.Color, color);

            _paletteHelper.SetTheme(theme);
        }

        private void SetSecondaryForegroundToSingleColor(Color color)
        {
            var theme = _paletteHelper.GetTheme();

            theme.SecondaryLight = new ColorPair(theme.SecondaryLight.Color, color);
            theme.SecondaryMid = new ColorPair(theme.SecondaryMid.Color, color);
            theme.SecondaryDark = new ColorPair(theme.SecondaryDark.Color, color);

            _paletteHelper.SetTheme(theme);
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
        public RelayCommand<Window> ExportSettings { get; set; }
        public RelayCommand ClearSettings { get; set; }

        #endregion

        public RelayCommand SaveSettings { get; set; }

        public SettingsViewModel(SettingsModel model, IWindowService windowService)
        {
            Model = model;
            _windowService = windowService;
            CloseCommand = new RelayCommand(CloseWindow);

            TemporarySettings = DuplicateGlobalSetting(GlobalSettings.Settings);



            ChangeTheme = new RelayCommand(OnChangeTheme);
            ToggleThemeCommand = new RelayCommand(() => IsDarkMode = !IsDarkMode);

            ImportSettings = new RelayCommand(OnImportSettings);
            ExportSettings = new RelayCommand<Window>(OnExportSettings);
            ClearSettings = new RelayCommand(OnClearSettings);
            SaveSettings = new RelayCommand(OnSaveSettings);
        }

        private void CloseWindow()
        {
            var oi = _windowService.GetMaterialDesignTheme();
            
            oi.SetBaseTheme(Theme.Light);
            oi.SetPrimaryColor(Color.FromRgb(239,239,139));
            oi.SetSecondaryColor(Color.FromRgb(156, 166, 89));



            var mat2 = new CustomColorThemeForMaterialDesign();
            mat2.PrimaryColor = Color.FromRgb(230,239,239);
            mat2.SecondaryColor = Color.FromRgb(156, 166, 89);


            //_windowService.SetMaterialDesignTheme(mat2.CustomTheme);
            _windowService.SetMaterialDesignTheme(oi);
            _windowService.ToggleDarkMode(true);

            var tt = "s";
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
