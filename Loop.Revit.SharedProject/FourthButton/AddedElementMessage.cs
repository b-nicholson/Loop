using System.Collections.Generic;


namespace Loop.Revit.FourthButton
{
    public class AddedElementMessage
    {
        public List<ElementWrapper> AddedElements { get; set; }

        public AddedElementMessage(List<ElementWrapper> elements)
        {
            AddedElements = elements;
        }
    }
}
