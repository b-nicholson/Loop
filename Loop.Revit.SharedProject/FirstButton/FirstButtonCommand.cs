﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using GenFusionsRevitCore.Servers3dContext.Graphics;
using Loop.Revit.Utilities.RevitUi;
using RibbonPanel = Autodesk.Revit.UI.RibbonPanel;


namespace Loop.Revit.FirstButton
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class FirstButtonCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {

                var uiApp = commandData.Application;
                var doc = uiApp.ActiveUIDocument.Document;
                var uiDoc = uiApp.ActiveUIDocument;

                //var blah = commandData.Application.Application.Documents;

                //var oi = uiDoc.GetOpenUIViews();



                //var docList = new List<Document>();
                //foreach (var docu in blah)
                //{
                //    var tempDoc = (Document)docu;

                //    if (!Equals(doc, tempDoc))
                //    {
                //        var hi = "";
                //        var newUiDoc = new UIDocument(tempDoc);
                //        var stupid = newUiDoc.GetOpenUIViews();
                //    }
                //    docList.Add(tempDoc);
                //}

               


                //var win= UIFramework.MainWindow.getMainWnd();

                //var children = win.getAllViews();

                //var b = "";
                //foreach (var child in children)
                //{
                //    child.Foreground = new SolidColorBrush(Colors.Aqua);
                //    b = child.Name;
                //    child.FontSize = 22;
        
                //}

                var point = new List<XYZ>();
                    point.Add(new XYZ(0,0,0));

                AppCommand.s_AppInstance.ServerStateMachine.ClearSolidServers();
                
                AppCommand.s_AppInstance.ServerStateMachine.DrawPointCube(doc, new XYZ(0,0,10), 30,SimpleColors.Blue,SimpleColors.Orange;

                AppCommand.s_AppInstance.ServerStateMachine.DrawPointsSphere(doc,point, 3, new ColorWithTransparency(255,165,0,50), new ColorWithTransparency(255,255,255,0));

                






            
                //MessageBox.Show("Hello World", "Loop", MessageBoxButton.OK);
                return Result.Succeeded;
            }
            catch (Exception)
            {
                return Result.Failed;
            }
        }

        public static void CreateButton (RibbonPanel panel)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var buttonData = new PushButtonData(
                MethodBase.GetCurrentMethod()?.DeclaringType?.Name +
                "CAA92DF0-54FC-464C-8C78-0E43FA6727C8", //any loaded button cannot be named the same, add GUID for safety
                "First" + Environment.NewLine + "Button",
                assembly.Location,
                MethodBase.GetCurrentMethod()?.DeclaringType?.FullName
            );

            var customButtonData =
                new CustomRibbonButton(buttonData, "AdskIcons.AreaPlan_light.ico", "_32x32.viewTitles.png");
            RibbonButtonRecord.CustomButtons.Add(customButtonData);

            var hi = RibbonButtonRecord.CustomButtons;

            var newButton = (Autodesk.Revit.UI.RibbonButton)panel.AddItem(buttonData);
            newButton.LargeImage = customButtonData.LightBitmapImage;
            newButton.ToolTip = "hi";
        }
    }
}
