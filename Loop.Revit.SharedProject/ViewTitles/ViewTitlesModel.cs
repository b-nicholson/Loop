using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Autodesk.Revit.DB;
using System.Web.UI.WebControls;
using Utilities.Units;
using System.Windows.Controls;
using View = Autodesk.Revit.DB.View;

namespace Loop.Revit.ViewTitles
{
    public class ViewTitlesModel
    {
        public UIApplication UiApp { get; }
        public Document Doc { get; }

        public View ActiveView { get; }

        public ViewTitlesModel(UIApplication uiApp)
        {
            UiApp = uiApp;
            Doc = uiApp.ActiveUIDocument.Document;
            ActiveView = Doc.ActiveView;
        }

        public ObservableCollection<SheetWrapper> CollectSheets()
        {
            var sheets = new FilteredElementCollector(Doc).OfCategory(BuiltInCategory.OST_Sheets).Cast<ViewSheet>().Select(x => new SheetWrapper(x));
            return new ObservableCollection<SheetWrapper>(sheets);
        }

        public ForgeTypeId CollectUnitsTypeId()
        {
            var units = Doc.GetUnits().GetFormatOptions(SpecTypeId.Length).GetUnitTypeId();
            return units;
        }

        public Units CollectUnits()
        {
            return Doc.GetUnits();
        }

        public int FindClosestUnitAccuracyIndex(RevitUnit selectedUnit)
        {
            var unitList = selectedUnit.Accuracy;
            var internalUnitsList = new List<double>();
            foreach (var option in unitList)
            {
                internalUnitsList.Add(UnitUtils.ConvertToInternalUnits(option, selectedUnit.UnitTypeId));
            }




            var units = Doc.GetUnits();
            var formatOpts = units.GetFormatOptions(SpecTypeId.Length);
            var accuracy = formatOpts.Accuracy;
            var accuracyInternal = UnitUtils.ConvertToInternalUnits(accuracy, formatOpts.GetUnitTypeId());
            var closest = internalUnitsList.OrderBy(item => Math.Abs(accuracyInternal - item)).First();

            var index = internalUnitsList.IndexOf(closest);
            return index;
        }

        public string FormatUnits(double inputDouble, RevitUnit unit, double accuracy)
        {
            var formatOptions = new FormatOptions(UnitTypeId.Meters);
            formatOptions.UseDefault = false;
            formatOptions.SetUnitTypeId(unit.UnitTypeId);
            formatOptions.Accuracy = accuracy;


            var valueParsingOpts = new ValueParsingOptions();
            valueParsingOpts.SetFormatOptions(formatOptions);

            var outputFormatOptions = new FormatValueOptions();
            outputFormatOptions.AppendUnitSymbol = true;
            outputFormatOptions.SetFormatOptions(formatOptions);

            var outputUnit = UnitFormatUtils.Format(unit.Unit, SpecTypeId.Length, inputDouble, true, outputFormatOptions);

            return outputUnit;

        }


        public (double, string) TryParseTextToInternalUnits(string inputText, Units unit)
        {
            double outputDouble;
            string outputMessage = String.Empty;

            //Always Converts to Internal Units, doesn't care what parsing options you give it
            var newunit = UnitFormatUtils.TryParse(unit, SpecTypeId.Length, inputText, out outputDouble, out outputMessage);

            return (outputDouble, outputMessage);
        }

        public void ChangeTitleLength(List<SheetWrapper> selected)
        {
            AppCommand.ViewTitlesHandler.Arg1 = selected;
            AppCommand.ViewTitlesHandler.Request = RequestId.AdjustViewTitleLengths;
            AppCommand.ViewTitlesEvent.Raise();

        }
    }
}
