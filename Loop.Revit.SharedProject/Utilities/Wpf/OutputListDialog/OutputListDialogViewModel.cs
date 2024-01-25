using CommunityToolkit.Mvvm.ComponentModel;
using Loop.Revit.Utilities.UserSettings;
using Loop.Revit.Utilities.Wpf.WindowServices;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace Loop.Revit.Utilities.Wpf.OutputListDialog
{
    public class OutputListDialogViewModel: ObservableObject
    {
        private readonly IWindowService _windowService;
        public bool IsDarkMode { get; set; }

        public ICollectionView DataGridElements { get; set; }

        public ObservableCollection<DataGridColumnModel> Columns { get; private set; }

        public OutputListDialogViewModel(IWindowService windowService, ITheme theme = null)
        {

            _windowService = windowService;
            IsDarkMode = GlobalSettings.Settings.IsDarkModeTheme;

            _windowService.ToggleDarkMode(IsDarkMode);
            if (theme != null)
            {
                _windowService.SetMaterialDesignTheme(theme);
            }
            else
            {
                var color = GlobalSettings.Settings.PrimaryThemeColor;
                var globalDarkMode = GlobalSettings.Settings.IsDarkModeTheme;
                var initTheme = _windowService.GetMaterialDesignTheme();
                initTheme.SetPrimaryColor(color);
                _windowService.SetMaterialDesignTheme(initTheme);
            }


            Columns = new ObservableCollection<DataGridColumnModel>();
            Columns.Add(new DataGridColumnModel { Header = "Column 1", BindingPath = "SheetNumber", Width = new DataGridLength(1, DataGridLengthUnitType.Auto) });
            Columns.Add(new DataGridColumnModel { Header = "Column 2", BindingPath = "SheetName", Width = new DataGridLength(100, DataGridLengthUnitType.Star) });


        }

    }
}
