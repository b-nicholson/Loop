using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Reflection;
using Loop.Revit.Utilities;
using Loop.Revit.Utilities.RevitUi;

namespace Loop.Revit.FavouriteViews
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]


    class FavouriteViewsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var app = commandData.Application;
                DockablePanelUtilsFv.ShowDockablePanel(app);
            }
            catch (Exception)
            {
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        public static void CreateButton(RibbonPanel panel)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var buttonData = new PushButtonData(
                MethodBase.GetCurrentMethod()?.DeclaringType?.Name + "CAA92DF0-54CC-464C-8C78-0E43FA6727C8", //any loaded button cannot be named the same, add GUID for safety
                "Favourite" + Environment.NewLine + "Views",
                assembly.Location,
                MethodBase.GetCurrentMethod()?.DeclaringType?.FullName
            );

            var customButtonData = new CustomRibbonButton(buttonData, "_32x32.viewTitles.png", "_32x32.firstButton.png");
            RibbonButtonRecord.CustomButtons.Add(customButtonData);

            var newButton = (Autodesk.Revit.UI.RibbonButton)panel.AddItem(buttonData);
            newButton.LargeImage = customButtonData.LightBitmapImage;
            newButton.ToolTip = "Show Favourite Views Panel";
        }
    }
}
