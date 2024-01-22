
using Loop.Revit.Utilities;

namespace Loop.Revit.ViewTitles
{
    public class OperationResultMessage
    {
        public OperationResult Result { get; set; }
        public OperationResultMessage(OperationResult result)
        {
            Result = result;
        }
    }
}
