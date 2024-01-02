using System.Windows.Input;
using System.Windows;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace Loop.Revit.Settings
{
    public sealed partial class SettingsView
    {
        public SettingsView()
        {
            // Useless dummy code to force revit to load the libraries https://stackoverflow.com/questions/55594443/how-to-include-materialdesignxamltoolkit-to-wpf-class-library
            ColorZoneAssist.SetMode(new System.Windows.Controls.GroupBox(), ColorZoneMode.Light);
            Hue hue = new Hue("name", System.Windows.Media.Color.FromArgb(1, 2, 3, 4), System.Windows.Media.Color.FromArgb(1, 5, 6, 7));
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }


        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    var sizingParams = GetVirtualWindowSize();
                    WindowState = WindowState.Maximized;
                    MaxHeight = sizingParams.height;
                    MaxWidth = sizingParams.width;
                    break;
                case WindowState.Maximized:
                    WindowState = WindowState.Normal;
                    break;
                default:
                    break;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private (double height, double width) GetVirtualWindowSize()
        {
            Window virtualWindow = new Window();
            virtualWindow.Show();
            virtualWindow.Opacity = 0;
            virtualWindow.WindowState = WindowState.Maximized;
            double returnHeight = virtualWindow.Height;
            double returnWidth = virtualWindow.Width;
            virtualWindow.Close();
            return (returnHeight, returnWidth);
        }
    }
}
