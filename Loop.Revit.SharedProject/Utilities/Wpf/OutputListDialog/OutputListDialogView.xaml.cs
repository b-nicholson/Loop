using System.Windows.Input;
using System.Windows;

namespace Loop.Revit.Utilities.Wpf.OutputListDialog
{
    public sealed partial class OutputListDialogView
    {
        public OutputListDialogView()
        {
            this.InitializeComponent();
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
                    var (height, width) = GetVirtualWindowSize();
                    WindowState = WindowState.Maximized;
                    MaxHeight = height;
                    MaxWidth = width;
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
