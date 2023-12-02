using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Loop.Revit.Utilities;

namespace Loop.Revit.SecondButton
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class SecondButtonCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var uiApp = commandData.Application;
                var m = new SecondButtonModel(uiApp);
                var vm = new SecondButtonViewModel(m);
                var v = new SecondButtonView
                {
                    DataContext = vm
                };

                var unused = new WindowInteropHelper(v)
                {
                    Owner = Process.GetCurrentProcess().MainWindowHandle
                };

                v.ShowDialog();

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
                MethodBase.GetCurrentMethod()?.DeclaringType?.Name + "CAA92DF0-54FC-464C-8C78-0E43FA6727C9", //any loaded button cannot be named the same, add GUID for safety
                "Second" + Environment.NewLine + "Button",
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