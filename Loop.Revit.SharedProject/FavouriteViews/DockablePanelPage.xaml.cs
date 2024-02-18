using Autodesk.Revit.UI;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors;

namespace Loop.Revit.FavouriteViews
{
    public sealed partial class DockablePanelPage : IDockablePaneProvider
    {
        public DockablePanelPage()
        {
            // Useless dummy code to force revit to load the libraries https://stackoverflow.com/questions/55594443/how-to-include-materialdesignxamltoolkit-to-wpf-class-library
            ColorZoneAssist.SetMode(new System.Windows.Controls.GroupBox(), ColorZoneMode.Light);
            new Hue("name", System.Windows.Media.Color.FromArgb(1, 2, 3, 4), System.Windows.Media.Color.FromArgb(1, 5, 6, 7));

            // Dummy code to force the loading of Microsoft.Xaml.Behaviors.Wpf assembly
            var dummyType = typeof(Behavior);

            InitializeComponent();
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