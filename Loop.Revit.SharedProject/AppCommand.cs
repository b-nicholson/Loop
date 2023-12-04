using System;
using System.Linq;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using GalaSoft.MvvmLight.Messaging;
using Loop.Revit.FavouriteViews;
using Loop.Revit.FirstButton;
using Loop.Revit.SecondButton;
using Loop.Revit.ThirdButton;
using Loop.Revit.Utilities;
using Loop.Revit.ViewTitles;

namespace Loop.Revit
{
    public class AppCommand : IExternalApplication
    {
        public static ThirdButtonRequestHandler ThirdButtonHandler { get; set; }
        public static ExternalEvent ThirdButtonEvent { get; set; }

        public static ViewTitlesRequestHandler ViewTitlesHandler { get; set; }
        public static ExternalEvent ViewTitlesEvent { get; set; }

        public Result OnStartup(UIControlledApplication app)
        {
            string tabName = "Loop";
            try
            {
                app.CreateRibbonTab(tabName);
            }
            catch (Exception e)
            {
                //ignored
            }

            string panelName = "Group 1";

            var ribbonPanel = app.GetRibbonPanels(tabName).FirstOrDefault(x => x.Name == panelName) ??
                               app.CreateRibbonPanel(tabName, panelName);

            FirstButtonCommand.CreateButton(ribbonPanel);
            ribbonPanel.AddSeparator();
            SecondButtonCommand.CreateButton(ribbonPanel);
            ribbonPanel.AddSeparator();
            ThirdButtonCommand.CreateButton(ribbonPanel);
            ribbonPanel.AddSeparator();
            FavouriteViewsCommand.CreateButton(ribbonPanel);
            ribbonPanel.AddSeparator();
            ViewTitlesCommand.CreateButton(ribbonPanel);

            DockablePanelUtilsFv.RegisterDockablePanel(app);


            app.ControlledApplication.DocumentChanged += OnDocumentChanged;

            ThirdButtonHandler =new ThirdButtonRequestHandler();
            ThirdButtonEvent = ExternalEvent.Create(ThirdButtonHandler);

            ViewTitlesHandler = new ViewTitlesRequestHandler();
            ViewTitlesEvent = ExternalEvent.Create(ViewTitlesHandler);

            return Result.Succeeded;
        }

        private void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            //todo things
        }

        public Result OnShutdown(UIControlledApplication app)
        {
            return Result.Succeeded;
        }
    }
}
