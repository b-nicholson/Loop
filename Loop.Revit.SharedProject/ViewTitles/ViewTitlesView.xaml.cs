
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;


namespace Loop.Revit.ViewTitles
{
    public sealed partial class ViewTitlesView
    {
        public ViewTitlesView()
        {
            // Useless dummy code to force revit to load the libraries https://stackoverflow.com/questions/55594443/how-to-include-materialdesignxamltoolkit-to-wpf-class-library
            ColorZoneAssist.SetMode(new GroupBox(), ColorZoneMode.Light);
            Hue hue = new Hue("name", System.Windows.Media.Color.FromArgb(1, 2, 3, 4), System.Windows.Media.Color.FromArgb(1, 5, 6, 7));
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("Current Directory: " + System.IO.Directory.GetCurrentDirectory());
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
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
