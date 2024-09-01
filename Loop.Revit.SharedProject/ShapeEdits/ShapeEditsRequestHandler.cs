using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using CommunityToolkit.Mvvm.Messaging;
using Loop.Revit.ShapeEdits.Helpers;
using Loop.Revit.Utilities.Selection;
using Loop.Revit.Utilities.ShapeEdits;
using Loop.Revit.ViewTitles.Helpers;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Loop.Revit.ShapeEdits
{
    public enum RequestId
    {
        None,
        Project,
        Select
    }
    public class ShapeEditsRequestHandler : IExternalEventHandler
    {
        public RequestId Request { get; set; }
        public ShapeEditsModel Model { get; set; }

        public List<Element> Targets { get; set; }
        public List<Element> HostElements { get; set; }
        public bool IgnoreInternalPoints { get; set; }
        public bool BoundaryPointOnly { get; set; }

        public bool IsTargetElement { get; set; }

        private bool Cancel { get; set; }

        public void Execute(UIApplication app)
        {
            try
            {
                switch (Request)
                {
                    case RequestId.None:
                        return;
                    case RequestId.Project:
                        Project(app);
                        break;
                    case RequestId.Select:
                        Select(app);
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

        private void Project(UIApplication app)
        {
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                //Event handler to deal with warnings
                app.DialogBoxShowing += UiAppOnDialogBoxShowing;


                var Doc = app.ActiveUIDocument.Document;
                var hostElements = HostElements;
                var targets = Targets;
                var ignoreInternalPoints = IgnoreInternalPoints;
                var boundaryPointOnly = BoundaryPointOnly;

                //Need logic control between floors and roofs
                var isRoof = false;

                //get host element geometry
                var hostGeometry = new List<Solid>();
                foreach (var hostElement in hostElements)
                {
                    var geometryElements = hostElement.get_Geometry(new Options());
                    foreach (var geometry in geometryElements)
                    {
                        hostGeometry.Add((Solid)geometry);
                    }
                }

                Solid mainGeometry;
                //if there's multiple selections for the host, we need to merge the geometry to have a singleton to project onto
                if (hostGeometry.Count > 1)
                {
                    mainGeometry = hostGeometry[0];
                    hostGeometry.RemoveAt(0);

                    foreach (var solid in hostGeometry)
                    {
                        mainGeometry =
                            BooleanOperationsUtils.ExecuteBooleanOperation(mainGeometry, solid,
                                BooleanOperationsType.Union);
                    }
                }
                //otherwise we're chilling - just grab the base geometry
                else
                {
                    mainGeometry = hostGeometry[0];
                }

                //get the faces from geometry, and find faces with a normal vector that's not facing sideways or down
                var faces = mainGeometry.Faces;
                var topFaces = ShapeEditUtils.GetTopFaces(faces);

                //get edges from the face geometry
                var allEdges = new List<Curve>();

                foreach (var face in topFaces)
                {
                    var edges = face.GetEdgesAsCurveLoops();
                    allEdges.AddRange(edges.SelectMany(loop => loop));
                }

                //remove duplicate lines from face edges
                var cleanedEdges = ShapeEditUtils.CleanLines(allEdges);


                var transactionGroup = new TransactionGroup(Doc, "Copy Shape Edits");
                transactionGroup.Start();

                foreach (var target in targets)
                {
                    var elemType = target.GetType();
                    dynamic targetElem;

                    if (elemType == typeof(FootPrintRoof))
                    {
                        targetElem = (FootPrintRoof)target;
                        isRoof = true;
                    }

                    else if (elemType == typeof(Floor))
                    {
                        targetElem = (Floor)target;
                    }
                    else
                    {

                        continue;
                    }

                    //the easiest way to deal with the boundary points is to just enable shape edits and get the resultant vertices
                    if (!targetElem.SlabShapeEditor.IsEnabled)
                    {
                        var t = new Transaction(Doc, "Enable Shape Edits");
                        t.Start();
                        targetElem.SlabShapeEditor.Enable();
                        t.Commit();
                    }

                    var shapePoints = targetElem.SlabShapeEditor.SlabShapeVertices;

                    var boundaryPoints = new List<XYZ>();
                    foreach (SlabShapeVertex point in shapePoints)
                    {
                        if (ignoreInternalPoints)
                        {
                            if (point.VertexType == SlabShapeVertexType.Corner)
                            {
                                boundaryPoints.Add(point.Position);
                            }
                        }
                        else
                        {
                            boundaryPoints.Add(point.Position);
                        }
                    }

                    //project boundary points onto surfaces
                    var projectedPoints = ShapeEditUtils.ProjectPointVerticallyToFaces(topFaces, boundaryPoints);


                    var splitLines = new List<List<Curve>>();
                    if (!boundaryPointOnly)
                    {
                        //need to make temporary elements for 2D intersection analysis
                        var t = new Transaction(Doc, "Temporary Element Creation");
                        t.Start();

                        //get target boundary edges, create model lines for later conversion to 2D


                        var boundaryCurveLoops = new List<CurveLoop>();
                        var singletonCurveLoop = new CurveLoop();
                        var boundaryModelCurves = new ModelCurveArray();
                        var flatIntersectingPoints = new List<List<XYZ>>();


                        if (isRoof)
                        {
                            //var shapeEdges = targetElem.GetProfiles();


                            //TODO Implement roof logic

                        }

                        if (!isRoof)
                        {
                            var shapeSketchId = targetElem.SketchId;
                            var sketch = (Sketch)Doc.GetElement(shapeSketchId);
                            var profile = sketch.Profile;
                            

                            foreach (var item in profile)
                            {
                                //Mutiloop sketches nest the curvearrays
                                if (item is CurveArray)
                                {
                                    var itemArray = (CurveArray)item;
                                    var curveLoop = new CurveLoop();
                                    foreach (Curve curve in itemArray)
                                    {
                                        curveLoop.Append(curve);
                                        boundaryModelCurves.Append(ShapeEditUtils.DrawModelCurve(Doc, curve));
                                    }
                                    boundaryCurveLoops.Add(curveLoop);
                                }
                                // Otherwise the logic is flat
                                else
                                {
                                    var itemCurve = (Curve)item;
                                    singletonCurveLoop.Append(itemCurve);
                                    boundaryModelCurves.Append(ShapeEditUtils.DrawModelCurve(Doc, itemCurve));
                                }

                            }

                            // keep downstream data consistent if it wasn't a multi loop sketch
                            if (boundaryCurveLoops.Count == 0)
                            {
                                boundaryCurveLoops.Add(singletonCurveLoop);
                            }

                            // find the point where the target's boundary intersects with the host face geometry, and get the relevant lines
                            // needs to be in a rollback transaction since we generate temporary elements to do so

                            var intersectionResult =
                                ShapeEditUtils.IntersectBoundariesWithFaceEdges(Doc, boundaryModelCurves, cleanedEdges);
                            var intersections = intersectionResult.LineIntersections;
                           

                            //splitLines.Add(intersections);

                            flatIntersectingPoints = intersectionResult.IntersectingPoints;

                        }
                        t.RollBack();

                        //since all these things are 3D,
                        //the easiest way to find what's relevant is to convert it all to 2D and intersect + trim
                        //var test = ShapeEditUtils.UseBoundaryCurvesToMakeSolidToTrimLines(Doc, boundaryCurveLoops,
                        //    cleanedEdges);
                        var edgesAtBoundaryIntersection = ShapeEditUtils.UseFilledRegionToTrimLines(Doc, boundaryCurveLoops,
                            flatIntersectingPoints, cleanedEdges);

                        splitLines.Add(edgesAtBoundaryIntersection);
                        //splitLines.Add(test);

                    }

                    var tIndividual = new Transaction(Doc, "Shape Edit Individuals");
                    tIndividual.Start();


                    //Add Split Lines
                    var flatList = splitLines.SelectMany(list => list).ToList();

                    var cleanLines = ShapeEditUtils.CleanLines(flatList);
                    ShapeEditUtils.AddSplitLines(cleanLines, targetElem);

                    foreach (var curve in flatList)
                    {
                        ShapeEditUtils.DrawModelCurve(Doc, curve);
                    }


                    //Add projected Points

                    foreach (var point in projectedPoints)
                    {
                        targetElem.SlabShapeEditor.DrawPoint(point);
                    }

                    tIndividual.Commit();


                }

                transactionGroup.Assimilate();

                //Un register to the event, we don't need it anymore.
                app.DialogBoxShowing -= UiAppOnDialogBoxShowing;
                stopwatch.Stop();
                TaskDialog.Show("Time Elapsed", stopwatch.Elapsed.ToString());

            }
            catch (Exception)
            {
                //Do nothing
            }

        }

        private static void UiAppOnDialogBoxShowing(object sender, DialogBoxShowingEventArgs args)
        {
            switch (args)
            {
                // Dismiss no open view pop-up. We pick a view dependent element so it is fast to find
                case DialogBoxShowingEventArgs args2:

                    args2.OverrideResult(1);
                    //if (args2.Message ==
                    //    "There is no open view that shows any of the highlighted elements.  Searching through the closed views to find a good view could take a long time.  Continue?")
                    //{
                    //    //This is from the windows forms dialog result enum. Direct cast to save a reference
                    //    args2.OverrideResult(1);
                    //}
                    break;
                default:
                    return;
            }
        }


        private void Select(UIApplication app)
        {
            var UiDoc = app.ActiveUIDocument;
            var Doc = UiDoc.Document;
            var targets = UiDoc.Selection.PickObjects(ObjectType.Element, new SelectionFilterMultipleCategories(new List<string> { "Roofs", "Floors" }));
            var selection = new List<Element>();
            foreach (var reference in targets)
            {
                selection.Add(Doc.GetElement(reference));
            }

            WeakReferenceMessenger.Default.Send(new ShapeEditsSelectionMessage(selection, IsTargetElement));
        }
        public string GetName()
        {
            return "Shape Edits Event Handler";
        }

        private void OnCancel(CancelMessage obj)
        {
            Cancel = obj.Cancel;
        }

    }
}
