using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace Loop.Revit.Utilities.Selection
{
    public class SelectionFilterMultipleCategories : ISelectionFilter
    {
        private readonly List<string> _categories;

        public SelectionFilterMultipleCategories(List<string> categories)
        {
            _categories = categories;
        }

        public bool AllowElement(Element elem)
        {
            if (elem == null) return false;
            if (elem.Category  == null) return false;

            var elementCategory = elem.Category.Name;
            var status = false;

            foreach (var category in _categories)
            {
                if (category == elementCategory)
                {
                    status = true;
                    break;
                }
            }
            return status;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
