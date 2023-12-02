using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Loop.Revit.Utilities;

namespace Loop.Revit.ViewTitles
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class ViewTitlesCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var uiApp = commandData.Application;
                var m = new ViewTitlesModel(uiApp);
                var vm = new ViewTitlesViewModel(m);
                var v = new ViewTitlesView
                {
                    DataContext = vm
                };

                var unused = new WindowInteropHelper(v)
                {
                    Owner = Process.GetCurrentProcess().MainWindowHandle
                };

                v.Show();

                m.ChangeTitleLength();
                return Result.Succeeded;

            }
            catch (Exception e)
            {
                return Result.Failed;
            }
        }

        public static void CreateButton(RibbonPanel panel)
        {
            var assembly = Assembly.GetExecutingAssembly();
            panel.AddItem(new PushButtonData(
                MethodBase.GetCurrentMethod()?.DeclaringType?.Name + "CAA92DF0-54CC-464C-8C78-0E43FA6727C8", //any loaded button cannot be named the same, add GUID for safety
                "View Titles",
                assembly.Location,
                MethodBase.GetCurrentMethod()?.DeclaringType?.FullName
            )
            {
                ToolTip = "Adjust Viewport Title Line Lengths",
                LargeImage = ImageUtils.LoadImage(assembly, "_32x32.viewTitles.png")
            });
        }
    }
}
