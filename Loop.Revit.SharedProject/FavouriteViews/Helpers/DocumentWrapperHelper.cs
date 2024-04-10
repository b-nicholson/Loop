using System;
using System.Windows.Media;
using Autodesk.Revit.DB;
using Loop.Revit.Utilities.Wpf.DocManager;
using Color = System.Windows.Media.Color;

namespace Loop.Revit.FavouriteViews.Helpers
{
    public static class DocumentWrapperHelper
    {
        public static void AddNewDocument(Document doc)
        {
            var docColour = Colors.Transparent;
            foreach (var colourItem in ColourThemeList.Colours)
            {
                if (colourItem == null) continue;
                if (!colourItem.IsTaken)
                {
                    docColour = colourItem.Color;
                    colourItem.IsTaken = true;
                    break;
                }
                // all taken, give it something random
                var rdm = new Random();
                var r = (byte)rdm.Next(0, 255);
                var g = (byte)rdm.Next(0, 255);
                var b = (byte)rdm.Next(0, 255);
                docColour = Color.FromRgb(r, g, b);

            }
            var newWrapper = new DocumentWrapper(doc, docColour);
            ActiveDocumentList.Docs.Add(newWrapper);
        }

        public static void RemoveDocument(Document doc)
        {
            var docList = ActiveDocumentList.Docs;

            var colour = Colors.DarkOliveGreen;
            foreach (var wrapper in docList)
            {
                if (Equals(wrapper.Doc, doc))
                {
                    colour = wrapper.Color;
                }
            }

            foreach (var colourItem in ColourThemeList.Colours)
            {
                if (colourItem.Color == colour)
                {
                    colourItem.IsTaken = false;
                }

            }
            docList.RemoveAll(item => Equals(item.Doc, doc));
        }


    }
}
