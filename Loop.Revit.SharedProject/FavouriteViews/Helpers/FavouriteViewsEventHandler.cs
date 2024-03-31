using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using System.Linq;
using Autodesk.Revit.UI.Events;
using Document = Autodesk.Revit.DB.Document;
using Loop.Revit.Utilities.Wpf.DocManager;


namespace Loop.Revit.FavouriteViews.Helpers
{
    public class FavouriteViewsEventHandler : IExternalEventHandler
    {
        public RequestId Request { get; set; }
        public object Arg1 { get; set; }
        public object Arg2 { get; set; }

        public static string Prefix = "LoopTempDocSwitch";

        public void Execute(UIApplication app)
        {
            try
            {
                switch (Request)
                {
                    case RequestId.None:
                        return;
                    case RequestId.ActivateView:
                        ActivateView(app);
                        break;
                    case RequestId.SwitchViewAndClose:
                        SwitchViewAndClose(app);
                        break;
                    case RequestId.CloseOpenViews:
                        CloseOpenUiViews(app);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception)
            {
                // ignore
            }
        }

        public void RefreshViews(UIApplication app)
        {
            var docWrapperList = ActiveDocumentList.Docs;

            foreach (var docWrapper in docWrapperList)
            {

                var viewWrapperList = docWrapper.ViewCollection;
                var newViewWrapperList = new ObservableCollection<ViewWrapper>();
                foreach (var viewWrapper in viewWrapperList)
                {
                    var viewId = viewWrapper.ElementId;
                    var doc = viewWrapper.Document;

                    var updatedView = new FilteredElementCollector(doc, viewId).ToElements();
                    if (updatedView == null) continue;

                    var newView = (View)updatedView[0];
                    var icon = IconMapper.GetIcon(newView);
                    var newWrapper = new ViewWrapper(doc, newView, icon);
                    newViewWrapperList.Add(newWrapper);
                }
                docWrapper.ViewCollection = newViewWrapperList;
            }

        }

        public void ActivateView(UIApplication app)
        {
            var uiDoc = app.ActiveUIDocument;
            var viewWrapper = (ViewWrapper)Arg1;
            var doc = viewWrapper.Document;
            var newUiDoc = new UIDocument(doc);

            var view = (View)newUiDoc.Document.GetElement(viewWrapper.ElementId);

            //We have a method for doc equals, but not uiDoc
            if (!Equals(uiDoc.Document, newUiDoc.Document))
            {
                //Event handler to deal with the no good view warning
                app.DialogBoxShowing += UiAppOnDialogBoxShowing;

                //Select an element, and use that to force the active ui application to switch. Can't request a view change if its not the ActiveUiDoc.
                //No native method to switch em. Stupid workaround but this is consistent even if the target doc is empty.

                var newDoc = newUiDoc.Document;

                var tg = new TransactionGroup(newDoc, "Temporary View Switcher Group");
                tg.Start();

                var t1 = new Transaction(newDoc, "Temp View Creation");
                t1.Start();

                FilteredElementCollector collector = new FilteredElementCollector(newDoc);
                collector.OfClass(typeof(ViewFamilyType));
                ViewFamilyType viewFamilyType = collector.Cast<ViewFamilyType>().First(vft => vft.ViewFamily == ViewFamily.Drafting);

                // Create a new ViewDrafting instance
                ViewDrafting tempView = ViewDrafting.Create(newDoc, viewFamilyType.Id);
                var id = new Guid().ToString();
                tempView.Name = Prefix + id;

                t1.Commit();

                var t2 = new Transaction(newDoc, "Create a detail line");
                t2.Start();

                var p1 = new XYZ(0, 0, 0);
                var p2 = new XYZ(0, 1, 0);
                var line = Line.CreateBound(p1, p2);

                var crv = newDoc.Create.NewDetailCurve(tempView, line);
                
                t2.Commit();

                var crvId = crv.Id;
                newUiDoc.ShowElements(crvId);

                newUiDoc.RequestViewChange(view);

                tg.RollBack();

                //Un register to the event, we don't need it anymore.
                app.DialogBoxShowing -= UiAppOnDialogBoxShowing;


            }
            else
            {
                //API supported method if we're in the same document
                newUiDoc.RequestViewChange(view);
            }

            //if (Arg2 != null)
            //{

                
            //    var docToClose = (Document)Arg2;
            //    try
            //    {
            //        docToClose.Close(false);
            //    }
            //    catch (Exception e)
            //    {
            //        //do nothing
            //    }
            //    Arg2 = null;
            //}
        }

        public void SwitchViewAndClose(UIApplication app)
        {
            var docToClose = (Document)Arg2;
            docToClose.Close(false);
        }

        public void CloseOpenUiViews(UIApplication app)
        {
            var newDoc = (Document)Arg2;
            var newUiDoc = new UIDocument(newDoc);


            FilteredElementCollector startingViewSettingsCollector =
                new FilteredElementCollector(newDoc);
            startingViewSettingsCollector.OfClass(typeof(StartingViewSettings));

            ElementId startingViewId = null;

            foreach (StartingViewSettings settings in startingViewSettingsCollector)
            {
                startingViewId = settings.ViewId;
            }


            var uiViews = newUiDoc.GetOpenUIViews();

            foreach (var view in uiViews)
            {
                try
                {
                    if (!Equals(startingViewId, view.ViewId))
                    {
                        view.Close();
                    }
                }
                catch (Exception)
                {
                   //do nothing
                }
            }
        }

        private static void UiAppOnDialogBoxShowing(object sender, DialogBoxShowingEventArgs args)
        {
            switch (args)
            {
                // Dismiss no open view pop-up. We pick a view dependent element so it is fast to find
                case TaskDialogShowingEventArgs args2:
                    if (args2.Message ==
                        "There is no open view that shows any of the highlighted elements.  Searching through the closed views to find a good view could take a long time.  Continue?")
                    {
                        //This is from the windows forms dialog result enum. Direct cast to save a reference
                        args2.OverrideResult(1);
                    }
                    break;
                default:
                    return;
            }
        }


        public string GetName()
        {
            return "Favourite Views Event Handler";
        }
    }
}
