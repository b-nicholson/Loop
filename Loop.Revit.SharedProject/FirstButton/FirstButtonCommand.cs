using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using Loop.Revit.Utilities;
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
