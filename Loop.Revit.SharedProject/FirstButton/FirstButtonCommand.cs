using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Loop.Revit.Utilities;

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
                MessageBox.Show("Hello World", "Loop", MessageBoxButton.OK);
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                return Result.Failed;
            }
        }

        public static void CreateButton (RibbonPanel panel)
        {
            var assembly = Assembly.GetExecutingAssembly();
            panel.AddItem(new PushButtonData(
                MethodBase.GetCurrentMethod()?.DeclaringType?.Name + "CAA92DF0-54FC-464C-8C78-0E43FA6727C8", //any loaded button cannot be named the same, add GUID for safety
                "First" + Environment.NewLine + "Button",
                assembly.Location,
                MethodBase.GetCurrentMethod()?.DeclaringType?.FullName
            )
            {
                ToolTip = "Button Things",
                LargeImage = ImageUtils.LoadImage(assembly, "_32x32.firstButton.png")
            });
        }
    }
}
