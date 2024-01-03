using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Reflection;
using Loop.Revit.Utilities;

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
            panel.AddItem(new PushButtonData(
                MethodBase.GetCurrentMethod()?.DeclaringType?.Name + "CAA92DF0-54FC-464C-8C78-0E43FC6727T8", //any loaded button cannot be named the same, add GUID for safety
                "Favourite" + Environment.NewLine + "Views",
                assembly.Location,
                MethodBase.GetCurrentMethod()?.DeclaringType?.FullName
            )
            {
                ToolTip = "Show Favourite Views Panel",
                LargeImage = ImageUtils.LoadImage(assembly, "_32x32.firstButton.png")
            });
        }
    }
}
