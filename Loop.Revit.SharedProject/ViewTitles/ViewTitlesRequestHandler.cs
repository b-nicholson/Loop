using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Loop.Revit.Utilities.ExtensibleStorage;
using CommunityToolkit.Mvvm.Messaging;
using Loop.Revit.Utilities;
using Loop.Revit.Utilities.CommonActions;
using Loop.Revit.ViewTitles.Helpers;

namespace Loop.Revit.ViewTitles
{
    public enum RequestId
    {
        None,
        AdjustViewTitleLengths,
        CreateSchema,
        LoadSchema
    }
    public class ViewTitlesRequestHandler : IExternalEventHandler
    {
        public RequestId Request { get; set; }

        public ViewTitlesModel Model { get; set; }

        public object Arg1 { get; set; }
        public object Arg2 { get; set; }

        private bool Cancel { get; set; }

        public void Execute(UIApplication app)
        {
            try
            {
                switch (Request)
                {
                    case RequestId.None:
                        return;
                    case RequestId.AdjustViewTitleLengths:
                        AdjustViewTitles(app);
                        break;
                    case RequestId.CreateSchema:
                        CreateDataStorage(app);
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

        public void CreateDataStorage(UIApplication app)
        {
            var Doc = app.ActiveUIDocument.Document;
            var schemaName = "ViewTitleLength";
            var schemaDescription = "Function for storing the viewport title line extension length";
            var paramName = "ExtensionDistance";
            var schemaInfo = new List<(string, Type)> { (paramName, typeof(double)) };

            var result = new OperationResult();
            result.OperationType = nameof(CreateDataStorage);

            try
            {
                var t = new Transaction(Doc, "Save Viewport Titles Extension Length");
                t.Start();

                var schema = ExtensibleStorageHelper.CreateSimpleSchema(
                    Model.ExtensibleStorageGuid, schemaName, schemaDescription, schemaInfo, SpecTypeId.Length);
                var paramInfo = new List<(string, dynamic)> { (paramName, Model.ExtensionDistance) };

                // I have no idea why it stores a weird number when looking it up manually in lookup,
                // it comes back out correctly though
                ExtensibleStorageHelper.CreateDataStorage(Doc, schema, Model.ExtensibleStorageGuid,
                    "Viewport Title Length Extension Distance", paramInfo, UnitTypeId.Feet);
                t.Commit();
                result.Success = true;
            }
            catch (Exception e)
            {
                result.Exception = e;
                result.Message = e.Message;
                result.TraceBackMessage = e.StackTrace;
            }

            WeakReferenceMessenger.Default.Send(new OperationResultMessage(result));

        }

        public void AdjustViewTitles(UIApplication app)
        {

            var result = new OperationResult();
            result.OperationType = nameof(AdjustViewTitles);

            if (!(Arg1 is List<SheetWrapper> selected))
            {
                return;
            }

            if (!(Arg2 is double extensionDistance))
            {
                return;
            }

            var ids = selected.SelectMany(sheet => sheet.ViewportIds).ToList();


            var doc = app.ActiveUIDocument.Document;
            var viewports = new FilteredElementCollector(doc, ids)
                .OfCategory(BuiltInCategory.OST_Viewports)
                .Cast<Viewport>();
            var viewportCount = viewports.Count();
            int viewportProcessingProgress = 0;

            //var tg = new TransactionGroup(doc, "test");
            //tg.Start();

            var t = new Transaction(doc, "Change Viewport Label Line Length");
            t.Start();

            WeakReferenceMessenger.Default.Unregister<CancelMessage>(this);
            WeakReferenceMessenger.Default.Register<ViewTitlesRequestHandler, CancelMessage>(this, (r, m) => r.OnCancel(m));
      
            

            WeakReferenceMessenger.Default.Send(new ProgressResultsMessage(viewportProcessingProgress,
                viewportCount));

            var nonEditableViewports = new List<Viewport>();

            foreach (var vp in viewports)
            {
                if (Cancel)
                {
                    //tg.RollBack();
                    t.RollBack();
                    result.Success = true;
                    result.Message = "Successfully Cancelled";

                    Cancel = false;

                    WeakReferenceMessenger.Default.Send(new ProgressResultsMessage(viewportCount, viewportCount));
                    WeakReferenceMessenger.Default.Send(new OperationResultMessage(result));

                    WeakReferenceMessenger.Default.Unregister<CancelMessage>(this);
                    break;
                }

                //var t = new Transaction(doc, "Change Viewport Label Line Length");
                //t.Start();

                var editableResult = WorkSharingCheckoutStatus.Check(doc, vp);
                if (editableResult.IsEditableByUser == false)
                {
                    nonEditableViewports.Add(vp);
                    continue;
                }

                var viewportTypeId = vp.GetTypeId();
                var viewportType = doc.GetElement(viewportTypeId);

          
                // Access the "Show Title" parameter
                var showTitleParam = viewportType.get_Parameter(BuiltInParameter.VIEWPORT_ATTR_SHOW_LABEL);
                var showTitleLineParam = viewportType.get_Parameter(BuiltInParameter.VIEWPORT_ATTR_SHOW_EXTENSION_LINE);

                var showTitle = Convert.ToBoolean(showTitleParam.AsInteger());
                var showLine = Convert.ToBoolean(showTitleLineParam.AsInteger());

                if (showTitle && showLine)
                {
                    vp.LabelLineLength = 0.001;
                    var rotation = vp.Rotation;

                    if (rotation != ViewportRotation.None)
                        vp.Rotation = ViewportRotation.None;

                    var outlines = vp.GetLabelOutline();
                    var outlineMax = outlines.MaximumPoint;
                    var outlineMin = outlines.MinimumPoint;

                    var labelOffset = vp.LabelOffset;
                    var boxMinPoint = vp.GetBoxOutline().MinimumPoint;
                    var newPoint = (boxMinPoint + labelOffset).X;
                    var symbolSize = newPoint - outlineMin.X;

                    var length = Math.Max(outlineMax.X - outlineMin.X, outlineMax.Y - outlineMin.Y) - symbolSize + extensionDistance;

                    vp.LabelLineLength = length;
                    if (rotation != ViewportRotation.None)
                        vp.Rotation = rotation;
                    
                }
                //t.Commit();
                viewportProcessingProgress++;

                WeakReferenceMessenger.Default.Send(new ProgressResultsMessage(viewportProcessingProgress,
                    viewportCount));
            }

            //tg.Assimilate();

            t.Commit();

            if (nonEditableViewports.Count > 0)
            {
                WeakReferenceMessenger.Default.Send(new NonEditableViewportsMessage(nonEditableViewports));
            }

            result.Success = true;
            result.Message = "✅ Successfully Changed " + 
                             (viewportProcessingProgress-nonEditableViewports.Count).ToString() + 
                             "/" +
                             viewportProcessingProgress.ToString() +
                             " Viewports";
            WeakReferenceMessenger.Default.Send(new OperationResultMessage(result));
            WeakReferenceMessenger.Default.Unregister<CancelMessage>(this);
        }



        public string GetName()
        {
            return "View Titles Event Handler";
        }

        private void OnCancel(CancelMessage obj)
        {
            Cancel = obj.Cancel;
        }
    }
}
