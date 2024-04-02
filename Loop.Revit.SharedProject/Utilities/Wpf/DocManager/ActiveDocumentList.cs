using System.Collections.Generic;


namespace Loop.Revit.Utilities.Wpf.DocManager
{
    public static class ActiveDocumentList
    {
        public static List<DocumentWrapper> Docs { get; set; }

        static ActiveDocumentList()
        {
            Docs  = new List<DocumentWrapper>();
        }

    }
}
