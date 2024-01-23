namespace Loop.Revit.ViewTitles.Helpers
{
    public class CancelMessage
    {
        public bool Cancel { get; set; }

        public CancelMessage(bool cancel)
        {
            Cancel = cancel;
        }
    }
}
