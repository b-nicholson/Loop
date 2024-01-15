using System.Windows;
using Loop.Revit.Utilities.Wpf.WindowServices;
using MaterialDesignThemes.Wpf;

namespace Loop.Revit.Utilities.Wpf.SmallDialog
{

    public class SmallDialog
    {
        public static SmallDialogResults Create(string title, string message, SdButton button1 = null, SdButton button2 = null, SdButton button3 = null, bool modeless = false, PackIconKind iconKind = PackIconKind.None, Window owner = null, ITheme theme =null)
        {
            var smallDialogView = new SmallDialogView();

            var smallDialogViewModel = new SmallDialogViewModel(
                windowService: new WindowService(smallDialogView),
                title:title,
                message:message,
                button1:button1,
                button2:button2,
                button3:button3,
                iconKind: iconKind,
                theme: theme
                );
            smallDialogView.DataContext = smallDialogViewModel;
            smallDialogView.ShowInTaskbar = false;
            if (owner != null)
            {
                smallDialogView.Owner = owner;
            }
            //TODO check if modal/modeless is useful. Not sure if usable in modeless.
            if (modeless) smallDialogView.Show();
            else smallDialogView.ShowDialog();

            return smallDialogViewModel.Results;
        }

    }
}
