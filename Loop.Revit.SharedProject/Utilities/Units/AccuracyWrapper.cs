
namespace Loop.Revit.Utilities.Units
{
    public class AccuracyWrapper
    {
        public double Value { get; set; }
        public string Name { get; set; }

        public AccuracyWrapper(string name, double value)
        {
            Value = value;
            Name = name;
        }
    }
}
