using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;

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

        private string _button1Content;
        public string Button1Content
        {
            get => _button1Content; 
            set=> SetProperty(ref _button1Content, value);
        }
        private string _button2Content;
        public string Button2Content
        {
            get => _button2Content;
            set => SetProperty(ref _button2Content, value);
        }
        private string _button3Content;
        public string Button3Content
        {
            get => _button3Content;
            set => SetProperty(ref _button3Content, value);
        }

        private bool _results;
        public bool Results
        {
            get => _results;
            set => SetProperty(ref _results, value);
        }

        public bool IsDarkMode { get; set; }

        public SmallDialogViewModel(string title, string message, string button1Content = null, string button2Content = null, string button3Content = null, bool darkMode = false)
        {
            Message = message;
            Title = title;
            IsDarkMode = darkMode;

            if (button1Content != null)
            {
                Button1Vis = true;
                Button1Content = button1Content;
            }
            if (button2Content != null)
            {
                Button2Vis = true;
                Button2Content = button2Content;
            }
            if (button3Content != null)
            {
                Button3Vis = true;
                Button3Content = button3Content;
            }

            Button1Command = new RelayCommand<Window>(OnButton1Command);
            Button2Command = new RelayCommand<Window>(OnButton2Command);
            Button3Command = new RelayCommand<Window>(OnButton3Command);
        }

        public void OnButton1Command(Window win)
        {
            Results = true;
            WeakReferenceMessenger.Default.Send(new SmallDialogMessage(Results));
            win.Close();
        }
        public void OnButton2Command(Window win)
        {
            Results = false;
            WeakReferenceMessenger.Default.Send(new SmallDialogMessage(Results));
            win.Close();
        }
        public void OnButton3Command(Window win)
        {
            Results = true;
            WeakReferenceMessenger.Default.Send(new SmallDialogMessage(Results));
            win.Close();
        }

  
    }
}
