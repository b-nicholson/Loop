using System;
using Autodesk.Revit.UI;

namespace Loop.Revit.FourthButton
{
    public static class DockablePanelUtils
    {

        public static void RegisterDockablePanel(UIControlledApplication app)
        {
            var vm = new DockablePanelViewModel();
            var v = new DockablePanelPage()
            {
                DataContext = vm
            };
            var panelId = new DockablePaneId(new Guid("97C5B808-41CF-4BE9-AFEE-1338D34C6483"));

            try
            {
                app.RegisterDockablePane(panelId, "Loop", v);
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        public static void ShowDockablePanel(UIApplication app)
        {
            var panelId = new DockablePaneId(new Guid("97C5B808-41CF-4BE9-AFEE-1338D34C6483"));
            var dp = app.GetDockablePane(panelId);
            if (dp != null && !dp.IsShown())
                dp.Show();
        }
    }
}
