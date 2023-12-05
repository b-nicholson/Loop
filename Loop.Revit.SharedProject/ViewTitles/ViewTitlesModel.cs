using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Autodesk.Revit.DB;

namespace Loop.Revit.ViewTitles
{
    public class ViewTitlesModel
    {
        public UIApplication UiApp { get; }
        public Document Doc { get; }
        public ViewTitlesModel(UIApplication uiApp)
        {
            UiApp = uiApp;
            Doc = uiApp.ActiveUIDocument.Document;
        }

        public ObservableCollection<SheetWrapper> CollectSheets()
        {
            var sheets = new FilteredElementCollector(Doc).OfCategory(BuiltInCategory.OST_Sheets).Cast<ViewSheet>().Select(x => new SheetWrapper(x));
            return new ObservableCollection<SheetWrapper>(sheets);
        }

        public ForgeTypeId CollectUnits()
        {
            var units = Doc.GetUnits().GetFormatOptions(SpecTypeId.Length).GetUnitTypeId();
            return units;
        }

        public void ChangeTitleLength(List<SheetWrapper> selected)
        {
            AppCommand.ViewTitlesHandler.Arg1 = selected;
            AppCommand.ViewTitlesHandler.Request = RequestId.AdjustViewTitleLengths;
            AppCommand.ViewTitlesEvent.Raise();

        }
    }
}
