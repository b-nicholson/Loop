using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
            return value.Trim();
        }

        private string RemoveSpaces(string value)
        {
            value = Regex.Replace(value, @"\s+", "");
            return value;
        }

        private double? ConvertToDouble(string value)
        {
            try
            {
                return Convert.ToDouble(value);
            }
            catch
            {
                return null;
            }
        }

        private List<string> SplitStringDelimited(string value)
        {
            bool isNegative = false;
            char[] delimiters = { ' ', '-' };

            if (value.StartsWith("-"))
            {
                value.TrimStart('-');
                isNegative = true;
            }

            var splitValue = value.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            if (splitValue.Length > 1)
            {
                return null;
            }

            var feetString = splitValue[0];

            var inchString = splitValue[1];


            if (isNegative)
            {
                feetString = "-" + feetString;
            }

            var finalstring = new List<string>();
            finalstring.Add(feetString);
            finalstring.Add(inchString);

            return finalstring;
        }

        private double? ConvertFractionStringToDouble(string value)
        {
            value = value.Trim();
            var splitValue = value.Split('/');
            var numerator = ConvertToDouble(splitValue[0]);
            var denominator = ConvertToDouble(splitValue[1]);

            return (numerator / denominator);

        }


        private string CheckFeetInches(string value, List<string> abbrevsList)
        {
            string usedAbbrev = null;
            foreach (var a in abbrevsList)
            {
                if (value.Contains(a))
                {
                    usedAbbrev = a;
                    break;
                }
            }
            return usedAbbrev;
        }

        private string ProcessUnitType(string value)
        {
            var unitsList = RevitUnitTypes.GetAllUnitTypes();
            var foundList = new List<RevitUnit>();
    
            foreach (var unit in unitsList)
            {
                var abbreviation = unit.Abbreviation;
                foreach (var a in abbreviation)
                {
                    var pattern = @"\b" + Regex.Escape(a) + @"\b";
                    if (Regex.Match(value, pattern, RegexOptions.IgnoreCase).Success)
                    {
                        if (!foundList.Contains(unit)) foundList.Add(unit);
                    }
                }
            }

            if (foundList.Count > 1)
            {
                if ((foundList.Contains(RevitUnitTypes.FractionalInches) || foundList.Contains(RevitUnitTypes.DecimalInches))
                    && 
                    (foundList.Contains(RevitUnitTypes.FractionalFeet) || foundList.Contains(RevitUnitTypes.DecimalFeet)))
                {
                    var splitFeetInchString = SplitStringDelimited(value);
                    
                    var var1 = splitFeetInchString[0];
                    var var2 = splitFeetInchString[1];


                    if (var1.Contains("/"))
                    {
                        var idk = ConvertFractionStringToDouble(var1);
                    }
                    if (var2.Contains("/"))
                    {
                        var idk2 = ConvertFractionStringToDouble(var2);
                    }


                }

            }



            return "hi";

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
                newValue = RemoveSpaces(newValue);
                var dblValue = Convert.ToDouble(newValue);


            }

            if (unitTypeId == UnitTypeId.FeetFractionalInches)
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
