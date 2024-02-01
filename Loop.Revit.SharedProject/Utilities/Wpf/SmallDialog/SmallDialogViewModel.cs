using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Loop.Revit.Utilities.UserSettings;
using MaterialDesignThemes.Wpf;
using Loop.Revit.Utilities.Wpf.WindowServices;

namespace Loop.Revit.Utilities.Wpf.SmallDialog
{
    public class SmallDialogViewModel : ObservableObject
    {
        private readonly IWindowService _windowService;

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }
        private PackIconKind _iconKind;

        public PackIconKind IconKind
        {
            get { return _iconKind; }
            set => SetProperty(ref _iconKind, value);
        }

        private bool _iconVis;

        public bool IconVis
        {
            get => _iconVis;
            set => SetProperty(ref _iconVis, value);

        }

        public RelayCommand Button1Command { get; set; }
        public RelayCommand Button2Command { get; set; }
        public RelayCommand Button3Command { get; set; }

        private bool _button1Vis;
        public bool Button1Vis
        {
            get => _button1Vis;
            set => SetProperty(ref _button1Vis, value);
        }
        private bool _button2Vis;
        public bool Button2Vis
        {
            get => _button2Vis;
            set => SetProperty(ref _button2Vis, value);
        }
        private bool _button3Vis;
        public bool Button3Vis
        {
            get => _button3Vis;
            set => SetProperty(ref _button3Vis, value);
        }

        private SdButton _button1;

        public SdButton Button1
        {
            get => _button1;
            set => SetProperty(ref _button1, value);
        }
        private SdButton _button2;

        public SdButton Button2
        {
            get => _button2;
            set => SetProperty(ref _button2, value);
        }

        private SdButton _button3;

        public SdButton Button3
        {
            get => _button3;
            set => SetProperty(ref _button3, value);
        }



        private SmallDialogResults _results;
        public SmallDialogResults Results
        {
            get => _results;
            set => SetProperty(ref _results, value);
        }

        public bool IsDarkMode { get; set; }

        public SmallDialogViewModel(string title, string message, IWindowService windowService, SdButton button1 = null, SdButton button2 = null, SdButton button3 = null, PackIconKind iconKind = PackIconKind.None, ITheme theme = null)
        {
            _windowService = windowService;
            Message = message;
            Title = title;
            IsDarkMode = GlobalSettings.Settings.IsDarkModeTheme;
            IconKind = iconKind;

            Button1 = button1;
            Button2 = button2;
            Button3 = button3;

            if (iconKind != PackIconKind.None)
            {
                IconVis = true;
            }
            if (button1 != null)
            {
                Button1Vis = true;
            }
            if (button2 != null)
            {
                Button2Vis = true;
            }
            if (button3 != null)
            {
                Button3Vis = true;
            }

            Button1Command = new RelayCommand(OnButton1Command);
            Button2Command = new RelayCommand(OnButton2Command);
            Button3Command = new RelayCommand(OnButton3Command);


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

        public void OnButton1Command()
        {
            Results = Button1.Results;
            WeakReferenceMessenger.Default.Send(new SmallDialogMessage(Results));
            _windowService.CloseWindow();
        }
        public void OnButton2Command()
        {
            Results = Button2.Results;
            WeakReferenceMessenger.Default.Send(new SmallDialogMessage(Results));
            _windowService.CloseWindow();
        }
        public void OnButton3Command()
        {
            Results = Button3.Results;
            WeakReferenceMessenger.Default.Send(new SmallDialogMessage(Results));
            _windowService.CloseWindow();
        }

  
    }
}
