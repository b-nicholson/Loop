using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.DB;

namespace Loop.Revit.ViewTitles
{
    public class UnitsWrapper
    {
        public string Name { get; set; }


        public UnitsWrapper()
        {
            var unitList = UnitUtils.GetAllUnits();

            foreach (var unit in unitList)
            {
              
            }

        }
    }
}
