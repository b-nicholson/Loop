using Autodesk.Revit.UI;

namespace Loop.Revit.FourthButton
{
    public sealed partial class DockablePanelPage : IDockablePaneProvider
    {
        public DockablePanelPage()
        {
            this.InitializeComponent();
        }

        public void SetupDockablePane(DockablePaneProviderData data)
        {
            data.FrameworkElement = this;
            data.InitialState = new DockablePaneState()
            {
                DockPosition = DockPosition.Tabbed,
                TabBehind = DockablePanes.BuiltInDockablePanes.ProjectBrowser
            };
            data.VisibleByDefault = true;
        }
    }
}
