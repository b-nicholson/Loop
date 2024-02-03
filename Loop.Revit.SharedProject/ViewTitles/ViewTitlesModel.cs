using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Autodesk.Revit.DB;
using Utilities.Units;
using Loop.Revit.Utilities.ExtensibleStorage;
using Loop.Revit.ViewTitles.Helpers;
using View = Autodesk.Revit.DB.View;

namespace Loop.Revit.ViewTitles
{
    public class ViewTitlesModel
    {

        public UIApplication UiApp { get; }
        public UIDocument UiDoc { get; }
        public Document Doc { get; }
        public View ActiveView { get; set; }

        public ViewTitlesModel(UIApplication uiApp)
        {
            UiApp = uiApp;
            UiDoc = UiApp.ActiveUIDocument;
            Doc = UiDoc.Document;
            ActiveView = Doc.ActiveView;
        }

        public Guid ExtensibleStorageGuid = new Guid("CA648A85-E51F-4B69-8C25-9CB4A600BE33");

        public double ExtensionDistance { get; set; }

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
                internalUnitsList.Add(UnitUtils.ConvertToInternalUnits(option.Value, selectedUnit.UnitTypeId));
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
            //Always Converts to Internal Units, doesn't care what parsing options you give it
            UnitFormatUtils.TryParse(
                unit, SpecTypeId.Length, inputText, out var outputDouble, out var outputMessage);

            return (outputDouble, outputMessage);
        }

        public void CreateDataStorage(double units)
        {
            AppCommand.ViewTitlesHandler.Arg1 = units;
            AppCommand.ViewTitlesHandler.Model = this;
            AppCommand.ViewTitlesHandler.Request = RequestId.CreateSchema;
            AppCommand.ViewTitlesEvent.Raise();

        }

        public void LoadDataStorage()
        {
            var result = ExtensibleStorageHelper.LoadDataStorage(Doc, ExtensibleStorageGuid, new List<string> { "ExtensionDistance" }, UnitTypeId.Feet);


            if (result.Success == true)
            {
                var dataStorageValue = result.ReturnObject;

                if (dataStorageValue.Count != 0)
                {
                    var elem = (double)dataStorageValue[0];
                    ExtensionDistance = elem;

                }
            }
        }

        public void RefreshActiveView()
        {
            AppCommand.ViewTitlesHandler.Request = RequestId.GetActiveView;
            AppCommand.ViewTitlesEvent.Raise();
        }

        public void ChangeTitleLength(List<SheetWrapper> selected)
        {
            AppCommand.ViewTitlesHandler.Arg1 = selected;
            AppCommand.ViewTitlesHandler.Arg2 = ExtensionDistance;
            AppCommand.ViewTitlesHandler.Request = RequestId.AdjustViewTitleLengths;
            AppCommand.ViewTitlesEvent.Raise();

        }
    }
}
