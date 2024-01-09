using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using MaterialDesignThemes.Wpf;

namespace Loop.Revit.Utilities.Wpf.SmallDialog
{
    public class SmallDialogViewModel : ObservableObject
    {
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

        public RelayCommand<Window> Button1Command { get; set; }
        public RelayCommand<Window> Button2Command { get; set; }
        public RelayCommand<Window> Button3Command { get; set; }

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



        private Enum _results;
        public Enum Results
        {
            get => _results;
            set => SetProperty(ref _results, value);
        }

        public bool IsDarkMode { get; set; }

        public SmallDialogViewModel(string title, string message, SdButton button1 = null, SdButton button2 = null, SdButton button3 = null, bool darkMode = false, PackIconKind iconKind = PackIconKind.None)
        {
            Message = message;
            Title = title;
            IsDarkMode = darkMode;
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

            Button1Command = new RelayCommand<Window>(OnButton1Command);
            Button2Command = new RelayCommand<Window>(OnButton2Command);
            Button3Command = new RelayCommand<Window>(OnButton3Command);
        }

        public void OnButton1Command(Window win)
        {
            Results = Button1.Results;
            WeakReferenceMessenger.Default.Send(new SmallDialogMessage(Results));
            win.Close();
        }
        public void OnButton2Command(Window win)
        {
            Results = Button2.Results;
            WeakReferenceMessenger.Default.Send(new SmallDialogMessage(Results));
            win.Close();
        }
        public void OnButton3Command(Window win)
        {
            Results = Button3.Results;
            WeakReferenceMessenger.Default.Send(new SmallDialogMessage(Results));
            win.Close();
        }

  
    }
}
