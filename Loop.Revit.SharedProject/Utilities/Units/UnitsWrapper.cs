using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Utilities.Units
{
    public class RevitUnit
    {
        public string Name { get; set; }
        public List<string> Abbreviation { get; set; }
        public ForgeTypeId UnitTypeId { get; set; }

        public ForgeTypeId Type { get; set; }

        public RevitUnit(ForgeTypeId unitTypeId, ForgeTypeId type, string name, List<string> abbreviation)
        {
            Name = name;
            UnitTypeId = unitTypeId;
            Abbreviation = abbreviation;
            Type = type;
        }

    }

    public static class RevitUnitTypes
    {
        public static readonly RevitUnit DecimalFeet = new RevitUnit(UnitTypeId.Feet, SpecTypeId.Length, "Feet", new List<string> { "'", "ft" });
        public static readonly RevitUnit FractionalFeet = new RevitUnit(UnitTypeId.Feet, SpecTypeId.Length, "Fractional Feet", new List<string> { "' \"", "ft in" });
        public static readonly RevitUnit DecimalInches = new RevitUnit(UnitTypeId.Inches, SpecTypeId.Length, "Inches", new List<string> { "\"", "in" });
        public static readonly RevitUnit FractionalInches = new RevitUnit(UnitTypeId.FractionalInches, SpecTypeId.Length, "Fractional Inches", new List<string> { "\"", "in" });
        public static readonly RevitUnit Meters = new RevitUnit(UnitTypeId.Meters, SpecTypeId.Length, "Meters", new List<string> { "m" });
        public static readonly RevitUnit Millimeters = new RevitUnit(UnitTypeId.Millimeters, SpecTypeId.Length, "Millimeters", new List<string> { "mm" });




        public static List<RevitUnit> GetUnitsByType(ForgeTypeId type)
        {
            var unitsByType = new List<RevitUnit>();
            foreach (var field in typeof(RevitUnitTypes).GetFields())
            {
                if (field.FieldType != typeof(RevitUnit)) continue;
                var unit = (RevitUnit)field.GetValue(null);
                if (unit.Type.Equals(type))
                {
                    unitsByType.Add(unit);
                }
            }

            return unitsByType;
        }


    }


}
