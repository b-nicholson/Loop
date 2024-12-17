<<<<<<< Updated upstream
﻿using System.Diagnostics;
=======
﻿using System;
using System.Collections.Generic;
<<<<<<< Updated upstream
=======
using System.Diagnostics;
>>>>>>> Stashed changes
>>>>>>> Stashed changes
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using CommunityToolkit.Mvvm.Messaging;
using Loop.Revit.ShapeEdits.Helpers;
using Loop.Revit.Utilities.Selection;
using Loop.Revit.Utilities.ShapeEdits;
using Loop.Revit.Utilities.Warnings;
using Loop.Revit.ViewTitles.Helpers;

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

        public bool FindClosestPointIfMissingTarget { get; set; }
        public bool IsTargetElement { get; set; }
        public double VerticalOffset { get; set; }
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
                var Doc = app.ActiveUIDocument.Document;
                var hostElements = HostElements;
                var targets = Targets;
                var ignoreInternalPoints = IgnoreInternalPoints;
                var boundaryPointOnly = BoundaryPointOnly;
<<<<<<< Updated upstream
                var findClosestPoint = FindClosestPointIfMissingTarget;
                var verticalOffset = VerticalOffset;
                verticalOffset = 0.0;
                findClosestPoint = true;
=======
<<<<<<< Updated upstream
=======
                var findClosestPoint = FindClosestPointIfMissingTarget;
                var rawOffset = VerticalOffset;
                findClosestPoint = true;
>>>>>>> Stashed changes


                var verticalOffset = UnitUtils.ConvertToInternalUnits(rawOffset, UnitTypeId.Millimeters);


>>>>>>> Stashed changes

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
                    try
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
                        isRoof = false;
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
                    var projectedPointResult = ShapeEditUtils.ProjectPointVerticallyToFaces(topFaces, boundaryPoints, verticalOffset);
                    var projectedPoints = projectedPointResult.SuccessfulProjections;


                    var splitLines = new List<List<Curve>>();
                    if (!boundaryPointOnly)
                    {
                        //get target boundary edges 
                        var boundaryCurveLoops = new List<CurveLoop>();
                        var singletonCurveLoop = new CurveLoop();

                        if (isRoof)
                        {
                            var revisedElem = (FootPrintRoof)targetElem;
                            var shapeEdges = revisedElem.GetProfiles();

<<<<<<< Updated upstream
                            foreach (ModelCurveArray boundary in shapeEdges)
                            {
                                var curveList = new List<Curve>();
                                var curveLoop = new CurveLoop();
                                foreach (ModelCurve modelLine in boundary)
                                {
                                    var curve = modelLine.GeometryCurve;
                                    //is it a circle?
                                    if (!curve.IsBound && curve.IsClosed)
                                    {
                                        var circle = (Arc)curve;
                                        // Get the center, radius, and normal of the circle
                                        XYZ center = circle.Center;
                                        double radius = circle.Radius;
                                        XYZ normal = circle.Normal;
=======
<<<<<<< Updated upstream
>>>>>>> Stashed changes

                                        // Define the start angle and angle increment for each segment
                                        double startAngle = 0;
                                        double angleIncrement = Math.PI / 2; // 90 degrees

<<<<<<< Updated upstream
                                        for (int i = 0; i < 4; i++)
                                        {
                                            // Calculate the start and end angles for the current segment
                                            double segmentStartAngle = startAngle + i * angleIncrement;
                                            double segmentEndAngle = segmentStartAngle + angleIncrement;

                                            // Create the arc segment
                                            Arc arcSegment = Arc.Create(center, radius, segmentStartAngle,
                                                segmentEndAngle, circle.XDirection, circle.YDirection);
                                            curveLoop.Append(arcSegment);
                                        }
                                    }
                                    //curveList.Add(curve);
                                    //curveLoop.Append(curve);
                                }

                                //var sortedCurveloop = ShapeEditUtils.SortCurvesContiguously(curveList);
                                //curveLoop = CurveLoop.Create(sortedCurveloop);
                                boundaryCurveLoops.Add(curveLoop);
                            }
=======
=======
                            foreach (ModelCurveArray boundary in shapeEdges)
                            {
                                var curveList = new List<Curve>();
                                var curveLoop = new CurveLoop();
                                foreach (ModelCurve modelLine in boundary)
                                {
                                    var curve = modelLine.GeometryCurve;

                                    //is it a circle?
                                    //if (!curve.IsBound && curve.IsClosed)
                                    //{
                                    //    var circle = (Arc)curve;
                                    //    // Get the center, radius, and normal of the circle
                                    //    XYZ center = circle.Center;
                                    //    double radius = circle.Radius;
                                    //    XYZ normal = circle.Normal;

                                    //    // Define the start angle and angle increment for each segment
                                    //    double startAngle = 0;
                                    //    double angleIncrement = Math.PI / 2; // 90 degrees

                                    //    for (int i = 0; i < 4; i++)
                                    //    {
                                    //        // Calculate the start and end angles for the current segment
                                    //        double segmentStartAngle = startAngle + i * angleIncrement;
                                    //        double segmentEndAngle = segmentStartAngle + angleIncrement;

                                    //        // Create the arc segment
                                    //        Arc arcSegment = Arc.Create(center, radius, segmentStartAngle,
                                    //            segmentEndAngle, circle.XDirection, circle.YDirection);
                                    //        curveLoop.Append(arcSegment);
                                    //    }
                                    //}

                                    curveList.Add(curve);
                                    //curveLoop.Append(curve);
                                }

                                var sortedCurveloop = ShapeEditUtils.SortCurvesContiguously(curveList);
                                curveLoop = CurveLoop.Create(sortedCurveloop);
                                boundaryCurveLoops.Add(curveLoop);
                            }
>>>>>>> Stashed changes
>>>>>>> Stashed changes
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
                                    }
                                    boundaryCurveLoops.Add(curveLoop);
                                }
                                // Otherwise the logic is flat
                                else
                                {
                                    var itemCurve = (Curve)item;
                                    singletonCurveLoop.Append(itemCurve);
                                }
                            }

                            // keep downstream data consistent if it wasn't a multi loop sketch
                            if (boundaryCurveLoops.Count == 0)
                            {
                                boundaryCurveLoops.Add(singletonCurveLoop);
                            }
                        }

                        //Take the boundaries of the top face, and intersect them with an extended solid of the target footprint
                        var linesFromSolid = ShapeEditUtils.UseBoundaryCurvesToMakeSolidToTrimLines(Doc, boundaryCurveLoops,
                            cleanedEdges, verticalOffset);

                        splitLines.Add(linesFromSolid);
                    }

                    var tIndividual = new Transaction(Doc, "Shape Edit Individuals");
                    tIndividual.Start();

                    var failureOpts = tIndividual.GetFailureHandlingOptions();
                    failureOpts.SetFailuresPreprocessor(new SuppressAllWarnings());
                    tIndividual.SetFailureHandlingOptions(failureOpts);


                    //Add Split Lines
                    var flatList = splitLines.SelectMany(list => list).ToList();
                    var cleanLines = ShapeEditUtils.CleanLines(flatList);
                    ShapeEditUtils.AddSplitLines(cleanLines, targetElem);


                    //Add projected Points
                    foreach (var point in projectedPoints)
                    {
                        targetElem.SlabShapeEditor.DrawPoint(point);
                    }

                    if (findClosestPoint)
                    {
                        SlabShapeVertexArray points = targetElem.SlabShapeEditor.SlabShapeVertices;
                        var existingShapePoints = new List<XYZ>();
                        foreach (SlabShapeVertex point in points)
                        {
                            existingShapePoints.Add(point.Position);
                       
                        }

                        var outstandingPoints = projectedPointResult.FailedProjections;
                        var matchedPoints =
                            ShapeEditUtils.FindClosestPointsFromListOfPoints(outstandingPoints,
                                existingShapePoints);
                        foreach (var matchedPoint in matchedPoints)
                        {
                            targetElem.SlabShapeEditor.DrawPoint(matchedPoint);
                        }
                    }


                    tIndividual.Commit();
                    }
                    catch (Exception e)
                    {
                     //Do nothing
                    }

                }

                transactionGroup.Assimilate();

            }
            catch (Exception)
            {
                //Do nothing
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
