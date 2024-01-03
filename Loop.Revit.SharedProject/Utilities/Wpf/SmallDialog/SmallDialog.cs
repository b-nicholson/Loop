using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Loop.Revit.Utilities.Wpf.SmallDialog
{

    public class SmallDialog
    {
        public static bool Create(string title, string message, string button1Content = null, string button2Content = null, string button3Content = null, bool darkMode = false, bool modeless = false, Window owner = null)
        {
            var smallDialogViewModel = new SmallDialogViewModel(title, message,  button1Content, button2Content, button3Content, darkMode);
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
