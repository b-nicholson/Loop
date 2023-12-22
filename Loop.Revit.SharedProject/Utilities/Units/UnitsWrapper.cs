using System.Collections.Generic;
using System.Web.UI.WebControls;
using Autodesk.Revit.DB;

namespace Utilities.Units
{
    public class RevitUnit
    {
        public string Name { get; set; }
        public List<string> Abbreviation { get; set; }
        public ForgeTypeId UnitTypeId { get; set; }

        public ForgeTypeId Type { get; set; }

        public List<double> Accuracy { get; set; }

        public Autodesk.Revit.DB.Units Unit;

        public RevitUnit(ForgeTypeId unitTypeId, ForgeTypeId type, string name, List<string> abbreviation, List<double> accuracy)
        {
            Name = name;
            UnitTypeId = unitTypeId;
            Abbreviation = abbreviation;
            Type = type;
            Accuracy = accuracy;

            var baseUnit = new Autodesk.Revit.DB.Units(UnitSystem.Metric);
            var formatOptions = new FormatOptions();
            formatOptions.UseDefault = false;
            formatOptions.SetUnitTypeId(UnitTypeId);
            baseUnit.SetFormatOptions(Type, formatOptions);

            Unit = baseUnit;
        }

    }


    public static class RevitUnitTypes
    {
        public static readonly RevitUnit DecimalFeet = new RevitUnit(UnitTypeId.Feet, SpecTypeId.Length, "Feet", new List<string> { "'", "ft" }, new List<double>{1,0.1,0.001,0.001,0.0001});
        public static readonly RevitUnit FractionalFeet = new RevitUnit(UnitTypeId.FeetFractionalInches, SpecTypeId.Length, "Feet Fractional Inches", new List<string> { "' \"", "ft in" }, new List<double> { 1.0/12.0, 0.5/ 12.0, 0.25 / 12.0, 0.125 / 12.0, 0.0625 / 12.0, 0.03125 / 12.0, 0.015625 / 12.0, 0.0078125 / 12.0, 0.00390625 / 12.0 });
        public static readonly RevitUnit DecimalInches = new RevitUnit(UnitTypeId.Inches, SpecTypeId.Length, "Inches", new List<string> { "\"", "in" }, new List<double> { 1, 0.1, 0.001, 0.001 });
        public static readonly RevitUnit FractionalInches = new RevitUnit(UnitTypeId.FractionalInches, SpecTypeId.Length, "Fractional Inches", new List<string> { "\"", "in" }, new List<double> { 1, 0.5, 0.25, 0.125, 0.0625, 0.03125, 0.015625, 0.0078125, 0.00390625 });
        public static readonly RevitUnit Meters = new RevitUnit(UnitTypeId.Meters, SpecTypeId.Length, "Meters", new List<string> { "m" }, new List<double> { 1, 0.1, 0.001, 0.001, 0.0001 });
        public static readonly RevitUnit Millimeters = new RevitUnit(UnitTypeId.Millimeters, SpecTypeId.Length, "Millimeters", new List<string> { "mm" },new List<double> { 1, 0.1, 0.001, 0.0001});




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

        public static List<RevitUnit> GetAllUnitTypes()
        {
            var unitsList= new List<RevitUnit>();
            foreach (var field in typeof(RevitUnitTypes).GetFields())
            {
                unitsList.Add((RevitUnit)field.GetValue(null));
            }
            return unitsList;
        }


    }


}
