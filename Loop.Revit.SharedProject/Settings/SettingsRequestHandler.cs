using System;
using Autodesk.Revit.UI;

namespace Loop.Revit.Settings
{
    public class SettingsRequestHandler : IExternalEventHandler
    {
        public bool IsDarkMode { get; set; }
        public void Execute(UIApplication app)
        {
            var theme = UITheme.Light;
            if (IsDarkMode)
            {
                theme = UITheme.Dark;
            }
            UIThemeManager.CurrentTheme = theme;
        }

        public string GetName()
        {
            return "Settings Request Handler";
        }
    }
}
