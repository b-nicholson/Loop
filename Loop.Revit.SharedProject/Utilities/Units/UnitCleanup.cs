using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities.Units;

namespace Loop.Revit.Utilities.Units
{
   public class UnitCleanup
    {
        private string AbbreviationRemover(string value, List<string> abbrevs)
        {
            foreach (var a in abbrevs)
            {
                if (value.Contains(a))
                {
                    value = value.Replace(a, "");
                }
            }

            return value;
        }
        public UnitCleanup(RevitUnit unit, string value)
        {
            var type = unit.Type;
            var abbrev = unit.Abbreviation;
            var name = unit.Name;
            var unitTypeId = unit.UnitTypeId;


            if (unitTypeId == UnitTypeId.Millimeters)
            { 
                var newValue = AbbreviationRemover(value, abbrev);

            }
            if (unitTypeId == UnitTypeId.Feet)
            {

            }
            if (unitTypeId == UnitTypeId.FractionalInches)
            {

            }
            else
            {
                
            }




        }
    }
}
