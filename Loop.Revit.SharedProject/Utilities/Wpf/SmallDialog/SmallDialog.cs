using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using MaterialDesignThemes.Wpf;

namespace Loop.Revit.Utilities.Wpf.SmallDialog
{

    public class SmallDialog
    {
        public static Enum Create(string title, string message, SdButton button1 = null, SdButton button2 = null, SdButton button3 = null, bool darkMode = false, bool modeless = false, PackIconKind iconKind = PackIconKind.None, Window owner = null)
        {
            var smallDialogViewModel = new SmallDialogViewModel(
                title:title,
                message:message,
                button1:button1,
                button2:button2,
                button3:button3,
                darkMode:darkMode,
                iconKind: iconKind
                );
            var smallDialogView = new SmallDialogView { DataContext = smallDialogViewModel };
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
