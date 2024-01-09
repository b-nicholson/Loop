

using System.Windows;
using System.Windows.Input;

namespace Loop.Revit.Utilities.Wpf.SmallDialog
{
    public sealed partial class SmallDialogView
    {
        public SmallDialogView()
        {
            this.InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

    }
}
