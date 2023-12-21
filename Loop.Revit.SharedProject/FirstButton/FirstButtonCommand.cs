using System;
using System.Collections.Generic;
using System.Linq;
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

                var uiApp = commandData.Application;
                var doc = uiApp.ActiveUIDocument.Document;

                var units = doc.GetUnits();
                

                var variable2 = "=1'+3'- 0.125\"";


                var baseUnit = new Autodesk.Revit.DB.Units(UnitSystem.Metric);
                var formatOptions2 = new FormatOptions();
                formatOptions2.UseDefault = false;
                formatOptions2.SetUnitTypeId(UnitTypeId.Meters);
                baseUnit.SetFormatOptions(SpecTypeId.Length, formatOptions2);





                var formatOptions = new FormatOptions(UnitTypeId.Meters);
                formatOptions.UseDefault = false;
                formatOptions.SetUnitTypeId(UnitTypeId.Meters);
                formatOptions.Accuracy = 0.0625/12;
         

                var valueParsingOpts = new ValueParsingOptions();
                valueParsingOpts.SetFormatOptions(formatOptions);



                double outputDouble;
                string outputMessage = String.Empty;

                //Always Converts to Internal Units, doesn't care what parsing options you give it
                var newunit = UnitFormatUtils.TryParse(units, SpecTypeId.Length, variable2, valueParsingOpts, out outputDouble, out outputMessage);



                var outputFormatOptions = new FormatValueOptions();
                outputFormatOptions.AppendUnitSymbol = true;
                outputFormatOptions.SetFormatOptions(formatOptions);



                var outputUnit = UnitFormatUtils.Format(units, SpecTypeId.Length, outputDouble, true, outputFormatOptions);








                //MessageBox.Show("Hello World", "Loop", MessageBoxButton.OK);
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
