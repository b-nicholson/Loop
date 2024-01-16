using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows.Interop;
using Autodesk.Revit.DB;
using Loop.Revit.Utilities;
using Loop.Revit.Utilities.Wpf.WindowServices;
using System.Windows.Threading;

namespace Loop.Revit.Settings
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class SettingsCommand : IExternalCommand
    {
        private Thread _uiThread;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                //its kinda useless for this to be multi-threaded + modeless but the ColorPicker crashes on going to the far right when it is modal.

                var uiApp = commandData.Application;
                var m = new SettingsModel(uiApp);
                _uiThread = new Thread(() =>
                    {
                        SynchronizationContext.SetSynchronizationContext(
                            new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                        var v = new SettingsView();
                        var vm = new SettingsViewModel(m, new WindowService(v));

                        v.DataContext = vm;

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
                MethodBase.GetCurrentMethod()?.DeclaringType?.Name + "CAA92DF0-54FC-464C-8C88-0E43FA6727C9", //any loaded button cannot be named the same, add GUID for safety
                "Settings",
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
