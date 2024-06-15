using System;

namespace Loop.Revit.Utilities
{
    public class OperationResult
    {
        public bool Success { get; set; } = false;
        public string OperationType { get; set; }
        public string Message { get; set; }
        public string TraceBackMessage { get; set; }

        private string _fullMessage;
        public string FullMessage
        {
            get => _fullMessage;
            set => _fullMessage = "Message: \n" + Message + "\n" + "Call Stack: \n" + TraceBackMessage;
        }
        public dynamic ReturnObject { get; set; }
        public Exception Exception { get; set; }

    }
}
