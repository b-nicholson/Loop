using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using CommunityToolkit.Mvvm.Messaging;
using Loop.Revit.FavouriteViews;
using Loop.Revit.FavouriteViews.Helpers;
using Loop.Revit.FirstButton;
using Loop.Revit.SecondButton;
using Loop.Revit.Settings;
using Loop.Revit.ThirdButton;
using Loop.Revit.Utilities.RevitUi;
using Loop.Revit.Utilities.UserSettings;
using Loop.Revit.Utilities.Wpf.DocManager;
using Loop.Revit.Utilities.Wpf.OutputListDialog;
using Loop.Revit.ViewTitles;
using Color = System.Windows.Media.Color;
using RibbonButton = Autodesk.Revit.UI.RibbonButton;
using RibbonItem = Autodesk.Revit.UI.RibbonItem;
using RibbonPanel = Autodesk.Revit.UI.RibbonPanel;

namespace Loop.Revit
{
    public class AppCommand : IExternalApplication
    {
        public static ThirdButtonRequestHandler ThirdButtonHandler { get; set; }
        public static ExternalEvent ThirdButtonEvent { get; set; }
        public static ViewTitlesRequestHandler ViewTitlesHandler { get; set; }
        public static ExternalEvent ViewTitlesEvent { get; set; }
        public static SettingsRequestHandler SettingsRequestHandler { get; set; }
        public static ExternalEvent SettingsEvent { get; set; }
        public static FavouriteViewsEventHandler FavouriteViewsHandler { get; set; }
        public static ExternalEvent FavouriteViewsEvent { get; set; }
        public static OutputListDialogEventHandler OutputListDialogHandler { get; set; }
        public static ExternalEvent OutputListDialogEvent { get; set; }
        private static List<RibbonPanel> CustomPanels { get; set; } = new List<RibbonPanel>();

        public Result OnStartup(UIControlledApplication app)
        {
            string tabName = "Loop";
            try
            {
                app.CreateRibbonTab(tabName);
            }
            catch (Exception)
            {
                //ignored
            }
            
            string panelName = "Group 1";

            var ribbonPanel = app.GetRibbonPanels(tabName).FirstOrDefault(x => x.Name == panelName) ??
                              app.CreateRibbonPanel(tabName, panelName);

            CustomPanels.Add(ribbonPanel);

            FirstButtonCommand.CreateButton(ribbonPanel);
            ribbonPanel.AddSeparator();
            SecondButtonCommand.CreateButton(ribbonPanel);
            ribbonPanel.AddSeparator();
            ThirdButtonCommand.CreateButton(ribbonPanel);
            ribbonPanel.AddSeparator();
            FavouriteViewsCommand.CreateButton(ribbonPanel);
            ribbonPanel.AddSeparator();
            ViewTitlesCommand.CreateButton(ribbonPanel);
            ribbonPanel.AddSeparator();
            SettingsCommand.CreateButton(ribbonPanel);

            DockablePanelUtilsFv.RegisterDockablePanel(app);

            RegisterEventHandlers(app);
            
            var settingsResult = UserSettingsManager.LoadSettings();
            var settings = new UserSetting();
            if (settingsResult.Success)
            {
                settings = settingsResult.ReturnObject;
            }
            GlobalSettings.Settings = settings;

            foreach (var colour in settings.DocumentColors)
            {
                ColourThemeList.Colours.Add(new ColourTheme(colour));
            }

            return Result.Succeeded;
        }


        private void RegisterEventHandlers(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentChanged += OnDocumentChanged;
            app.ViewActivated += OnViewActivated;

            ThirdButtonHandler = new ThirdButtonRequestHandler();
            ThirdButtonEvent = ExternalEvent.Create(ThirdButtonHandler);

            ViewTitlesHandler = new ViewTitlesRequestHandler();
            ViewTitlesEvent = ExternalEvent.Create(ViewTitlesHandler);

            SettingsRequestHandler = new SettingsRequestHandler();
            SettingsEvent = ExternalEvent.Create(SettingsRequestHandler);

            FavouriteViewsHandler = new FavouriteViewsEventHandler();
            FavouriteViewsEvent = ExternalEvent.Create(FavouriteViewsHandler);

            OutputListDialogHandler = new OutputListDialogEventHandler();
            OutputListDialogEvent = ExternalEvent.Create(OutputListDialogHandler);

            app.ControlledApplication.DocumentOpened += OnDocumentOpened;
            app.ControlledApplication.DocumentClosing += OnDocumentClosing;


            #if !(Revit2022 || Revit2023)
            app.ThemeChanged += OnThemeChanged;
            ChangeIcons();
            #endif
        }

        private void OnDocumentOpened(object sender, DocumentOpenedEventArgs e)
        {
            DocumentWrapperHelper.AddNewDocument(e.Document);
        }
        private void OnDocumentClosing(object sender, DocumentClosingEventArgs e)
        {
            DocumentWrapperHelper.RemoveDocument(e.Document);
        }

        private void OnViewActivated(object sender, ViewActivatedEventArgs e)
        {
            //Favourite Views, send view for processing
            WeakReferenceMessenger.Default.Send(new ViewActivatedMessage(e.CurrentActiveView, e.Document));

        }

        private void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            //todo things
        }

        #if !(Revit2022 || Revit2023)
        private void OnThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            ChangeIcons();
        }


        public static void ChangeIcons()
        {
            var theme = UIThemeManager.CurrentTheme;
            ChangeRibbonButtons.SwapIconTheme(CustomPanels, theme);
            bool isDarkMode = theme == UITheme.Dark;
            GlobalSettings.Settings.IsDarkModeTheme = isDarkMode;

            WeakReferenceMessenger.Default.Send(new ColourChangedMessage());
        }
        #endif

        public Result OnShutdown(UIControlledApplication app)
        {
            return Result.Succeeded;
        }
    }
}
