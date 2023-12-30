using System.Collections.Generic;
using System.Web.UI.WebControls;
using Autodesk.Revit.DB;
using Loop.Revit.Utilities.Units;

namespace Utilities.Units
{
    public class RevitUnit
    {
        public string Name { get; set; }
        public List<string> Abbreviation { get; set; }
        public ForgeTypeId UnitTypeId { get; set; }

        public ForgeTypeId Type { get; set; }

        public List<AccuracyWrapper> Accuracy { get; set; }

        public Autodesk.Revit.DB.Units Unit;

        public RevitUnit(ForgeTypeId unitTypeId, ForgeTypeId type, string name, List<string> abbreviation, List<AccuracyWrapper> accuracy)
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
        private static readonly List<AccuracyWrapper> AccuracyDecimals = new List<AccuracyWrapper>
        {
            new AccuracyWrapper("1", 1),
            new AccuracyWrapper("0.1", 0.1),
            new AccuracyWrapper("0.01", 0.01),
            new AccuracyWrapper("0.001", 0.001),
            new AccuracyWrapper("0.0001", 0.0001),
            new AccuracyWrapper("0.00001", 0.00001)
        };
        private static readonly List<AccuracyWrapper> AccuracyFeet = new List<AccuracyWrapper>
        {
            new AccuracyWrapper("1\"", 1.0/12.0),
            new AccuracyWrapper("1/2\"", 0.5/ 12.0),
            new AccuracyWrapper("1/4\"", 0.25 / 12.0),
            new AccuracyWrapper("1/8\"", 0.125 / 12.0),
            new AccuracyWrapper("1/16\"", 0.0625 / 12.0),
            new AccuracyWrapper("1/32\"", 0.03125 / 12.0),
            new AccuracyWrapper("1/64\"", 0.015625 / 12.0),
            new AccuracyWrapper("1/128\"", 0.0078125 / 12.0),
            new AccuracyWrapper("1/256\"", 0.00390625 / 12.0)
        };
        private static readonly List<AccuracyWrapper> AccuracyInches = new List<AccuracyWrapper>
        {
            new AccuracyWrapper("1\"", 1.0),
            new AccuracyWrapper("1/2\"", 0.5),
            new AccuracyWrapper("1/4\"", 0.25),
            new AccuracyWrapper("1/8\"", 0.125),
            new AccuracyWrapper("1/16\"", 0.0625),
            new AccuracyWrapper("1/32\"", 0.03125),
            new AccuracyWrapper("1/64\"", 0.015625),
            new AccuracyWrapper("1/128\"", 0.0078125),
            new AccuracyWrapper("1/256\"", 0.00390625)
        };



        public static readonly RevitUnit DecimalFeet = new RevitUnit(UnitTypeId.Feet, SpecTypeId.Length, "Decimal Feet", new List<string> { "'", "ft" }, AccuracyDecimals);
        public static readonly RevitUnit FractionalFeet = new RevitUnit(UnitTypeId.FeetFractionalInches, SpecTypeId.Length, "Feet Fractional Inches", new List<string> { "' \"", "ft in" }, AccuracyFeet);
        public static readonly RevitUnit DecimalInches = new RevitUnit(UnitTypeId.Inches, SpecTypeId.Length, "Decimal Inches", new List<string> { "\"", "in" }, AccuracyDecimals);
        public static readonly RevitUnit FractionalInches = new RevitUnit(UnitTypeId.FractionalInches, SpecTypeId.Length, "Fractional Inches", new List<string> { "\"", "in" }, AccuracyInches);
        public static readonly RevitUnit Meters = new RevitUnit(UnitTypeId.Meters, SpecTypeId.Length, "Meters", new List<string> { "m" }, AccuracyDecimals);
        public static readonly RevitUnit Centimeters = new RevitUnit(UnitTypeId.Centimeters, SpecTypeId.Length, "Centimeters", new List<string> { "cm" }, AccuracyDecimals);
        public static readonly RevitUnit Millimeters = new RevitUnit(UnitTypeId.Millimeters, SpecTypeId.Length, "Millimeters", new List<string> { "mm" }, AccuracyDecimals);




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
