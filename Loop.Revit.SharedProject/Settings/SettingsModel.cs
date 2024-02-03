using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.DB;

namespace Loop.Revit.Settings
{
    public class SettingsModel
    {
        public UIApplication UiApp { get; }
        public Document Doc { get; }

        public SettingsModel(UIApplication uiApp)
        {
            UiApp = uiApp;
            Doc = uiApp.ActiveUIDocument.Document;
        }

        #region Theme Actions

        public void ChangeTheme(bool isDarkMode)
        {
            AppCommand.SettingsRequestHandler.IsDarkMode = isDarkMode;
            AppCommand.SettingsEvent.Raise();

        }
        

        #endregion

    }
}
