using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Loop.Revit.Utilities.UserSettings;
using Loop.Revit.Utilities.Wpf.WindowServices;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using Autodesk.Revit.UI;
using CommunityToolkit.Mvvm.Input;
using Loop.Revit.Utilities.Wpf.DataGridUtils;
using Autodesk.Revit.DB;
using System.Windows.Controls.Primitives;

namespace Loop.Revit.Utilities.Wpf.OutputListDialog
{
    public class OutputListDialogViewModel: ObservableObject
    {
        private readonly IWindowService _windowService;
        public bool IsDarkMode { get; set; }

        public ICollectionView DataGridElements { get; set; }

        public ObservableCollection<DataGridColumnModel> Columns { get; set; }

        public RelayCommand<object> ButtonCommand { get; set; }

        public UIDocument uiDoc { get; set; }

        public OutputListDialogViewModel(IWindowService windowService, ObservableCollection<DataGridColumnModel> columns, ITheme theme = null)
        {

            _windowService = windowService;
            IsDarkMode = GlobalSettings.Settings.IsDarkModeTheme;
            ButtonCommand = new RelayCommand<object>(OnButtonCommand);

            foreach (var column in columns)
            {
                if (column is DataGridButtonColumnModel buttonColumn)
                {
                    buttonColumn.Command = ButtonCommand;
                }
            }

            Columns = columns;

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

            

        }

        private void OnButtonCommand(object parameter)
        {
            AppCommand.OutputListDialogHandler.Arg1 = parameter;
            AppCommand.OutputListDialogHandler.Request = RequestId.FindElements;
            AppCommand.OutputListDialogEvent.Raise();
        }

    }
}
