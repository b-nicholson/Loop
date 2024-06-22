using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;
using Loop.Revit.Utilities.RevitUi;
using Utilities.Wpf.Services.WindowServices;

namespace Loop.Revit.ShapeEdits
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class ShapeEditsCommand : IExternalCommand
    {
        private Thread _uiThread;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var uiApp = commandData.Application;
                var m = new ShapeEditsModel(uiApp);

                _uiThread = new Thread(() =>
                {
                    //Set the sync context
                    SynchronizationContext.SetSynchronizationContext(
                        new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));


                    var v = new ShapeEditsView();

                    var vm = new ShapeEditsViewModel(m, new WindowService(v));


                    v.DataContext = vm;
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
            var buttonData = new PushButtonData(
                MethodBase.GetCurrentMethod()?.DeclaringType?.Name + "19737CD1-6B7D-439D-84F0-AD108485CA03", //any loaded button cannot be named the same, add GUID for safety
                "Shape Edits",
                assembly.Location,
                MethodBase.GetCurrentMethod()?.DeclaringType?.FullName
            );

            var customButtonData = new CustomRibbonButton(buttonData, "_32x32.viewTitles.png", "_32x32.firstButton.png");
            RibbonButtonRecord.CustomButtons.Add(customButtonData);

            var newButton = (Autodesk.Revit.UI.RibbonButton)panel.AddItem(buttonData);
            newButton.LargeImage = customButtonData.LightBitmapImage;
            newButton.ToolTip = "Shape Edit Projections";
        }
    }
}
