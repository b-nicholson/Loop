﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.UI.WebControls;
using System.Windows.Media;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Windows;
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
using Loop.Revit.ViewTitles.Helpers;
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
            
            app.ControlledApplication.DocumentChanged += OnDocumentChanged;
            app.ViewActivated += OnViewActivated;


            ThirdButtonHandler =new ThirdButtonRequestHandler();
            ThirdButtonEvent = ExternalEvent.Create(ThirdButtonHandler);

            ViewTitlesHandler = new ViewTitlesRequestHandler();
            ViewTitlesEvent = ExternalEvent.Create(ViewTitlesHandler);

            SettingsRequestHandler = new SettingsRequestHandler();
            SettingsEvent = ExternalEvent.Create(SettingsRequestHandler);

            FavouriteViewsHandler = new FavouriteViewsEventHandler();
            FavouriteViewsEvent = ExternalEvent.Create(FavouriteViewsHandler);


            OutputListDialogHandler = new OutputListDialogEventHandler();
            OutputListDialogEvent = ExternalEvent.Create(OutputListDialogHandler);

            var settingsResult = UserSettingsManager.LoadSettings();
            var settings = new UserSetting();
            if (settingsResult.Success)
            {
                settings = settingsResult.ReturnObject;
            }
            GlobalSettings.Settings = settings;


            app.ControlledApplication.DocumentOpened += OnDocumentOpened;
            app.ControlledApplication.DocumentClosing += OnDocumentClosing;


        //#if Revit2024

#if !(Revit2022 || Revit2023)
            app.ThemeChanged += OnThemeChanged;
            ChangeIcons();
#endif

            return Result.Succeeded;
        }


        private void OnDocumentOpened(object sender, DocumentOpenedEventArgs e)
        {
            var alreadyLoadedDocs = ActiveDocumentList.Docs;

            var docColour = Colors.Transparent;
            foreach (var colourItem in GlobalSettings.Settings.DocumentColors)
            {
                if (colourItem == null) continue;
                if (!colourItem.IsTaken)
                {
                    docColour = colourItem.Color;
                    colourItem.IsTaken = true;
                    break;
                }
                // all taken, give it something random
                var rdm = new Random();
                var r = (byte)rdm.Next(0, 255);
                var g = (byte)rdm.Next(0, 255);
                var b = (byte)rdm.Next(0, 255);
                docColour = Color.FromRgb(r, g, b);

            }
            var newWrapper = new DocumentWrapper(e.Document, docColour);
            ActiveDocumentList.Docs.Add(newWrapper);
        }
        private void OnDocumentClosing(object sender, DocumentClosingEventArgs e)
        {
            var docList = ActiveDocumentList.Docs;

            var colour = Colors.DarkOliveGreen;
            foreach (var wrapper in docList)
            {
                if (Equals(wrapper.Doc, e.Document))
                {
                    colour = wrapper.Color;
                }
            }

            foreach (var colourItem in GlobalSettings.Settings.DocumentColors)
            {
                if (colourItem.Color == colour)
                {
                    colourItem.IsTaken = false;
                }
                
            }
            docList.RemoveAll(item => Equals(item.Doc, e.Document));

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
        }
#endif

        public Result OnShutdown(UIControlledApplication app)
        {
            return Result.Succeeded;
        }
    }
}
