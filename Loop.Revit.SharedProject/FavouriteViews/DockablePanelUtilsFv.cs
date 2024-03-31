using System;
using Autodesk.Revit.UI;
using Utilities.Wpf.Services.PageServices;


namespace Loop.Revit.FavouriteViews
{
    public static class DockablePanelUtilsFv
    {
        public static void RegisterDockablePanel(UIControlledApplication app)
        {
            var v = new DockablePanelPage();
            var vm = new DockablePanelViewModel(new PageService(v));
            v.DataContext = vm;
            var panelId = new DockablePaneId(new Guid("83088243-FE6A-444A-8114-FC5B8520AECD"));

            try
            {
                app.RegisterDockablePane(panelId, "Favourite Views", v);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static void ShowDockablePanel(UIApplication app)
        {
            var panelId = new DockablePaneId(new Guid("83088243-FE6A-444A-8114-FC5B8520AECD"));
            var dp = app.GetDockablePane(panelId);
            if (dp != null && !dp.IsShown())
                dp.Show();
        }
    }
}
