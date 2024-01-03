using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
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
        private Thread _uiThread;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var uiApp = commandData.Application;
                var m = new ViewTitlesModel(uiApp);

                _uiThread = new Thread(() =>
                {
                    //Set the sync context
                    SynchronizationContext.SetSynchronizationContext(
                        new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));




                    var vm = new ViewTitlesViewModel(m);
                    var v = new ViewTitlesView
                    {
                        DataContext = vm
                    };

                    var unused = new WindowInteropHelper(v)
                    {
                        Owner = Process.GetCurrentProcess().MainWindowHandle
                    };
                    v.Closed += (s, e) => Dispatcher.CurrentDispatcher.InvokeShutdown();

                    v.Show();
                    Dispatcher.Run();







                });

                _uiThread.SetApartmentState(ApartmentState.STA);
                _uiThread.IsBackground = true;
                _uiThread.Start();


                return Result.Succeeded;

            }
            catch (Exception)
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
