

using System;

namespace Loop.Revit.Utilities
{
    public class OperationResult
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; }
        public dynamic ReturnObject { get; set; }
        public Exception Exception { get; set; }

    }
}
