using Autodesk.Revit.DB;
using Line = Autodesk.Revit.DB.Line;

namespace Loop.Revit.Utilities.ShapeEdits
{
    public static class ShapeEditUtils
    {

        public static List<Curve> CleanLines(List<Curve> curves)
        {
            //var curveListA = new List<Curve>();
            //var curveListB = new List<Curve>();

            //foreach (var curve in curves)
            //{
            //    var start = curve.GetEndPoint(0);
            //    var end = curve.GetEndPoint(1);

            //    var flippedCurve = curve.CreateReversed();
            //    var flippedStart = flippedCurve.GetEndPoint(0);
            //    var flippedEnd = flippedCurve.GetEndPoint(1);

            //}

            HashSet<Curve> uniqueCurves = new HashSet<Curve>(new CurveComparer());

            foreach (Curve curve in curves)
            {
                uniqueCurves.Add(curve);
            }

            return new List<Curve>(uniqueCurves);
        }

        public static void AddSplitLines(List<Curve> edges, FootPrintRoof inputElement)
        {
            _addSplitLines(edges, inputElement, true);
        }

        public static void AddSplitLines(List<Curve> edges, Floor inputElement)
        {
            _addSplitLines(edges, inputElement, false);
        }

        private static void _addSplitLines(List<Curve> edges, Element inputElement, bool isRoof)
        {
            dynamic target = inputElement;
            if (isRoof)
            {
                target = (FootPrintRoof)inputElement;
            }
            else
            {
                target = (Floor)inputElement;
            }


            foreach (var curve in edges)
            {
                var startPoint = curve.GetEndPoint(0);
                var endPoint = curve.GetEndPoint(1);

                SlabShapeVertex slabShapeVertexStart = null;
                SlabShapeVertex slabShapeVertexEnd = null;

                #if (Revit2022 || Revit2023)
                {
                    try
                    {
                        slabShapeVertexStart = target.SlabShapeEditor.DrawPoint(startPoint);
                    }
                    catch (Exception)
                    {
                        //Do nothing
                    }
                    try
                    {
                        slabShapeVertexEnd = target.SlabShapeEditor.DrawPoint(endPoint);
                    }
                    catch (Exception)
                    {
                      //Do nothing
                    }
                    if (slabShapeVertexStart != null && slabShapeVertexEnd != null)
                    {
                        try
                        {
                            target.SlabShapeEditor.DrawSplitLine(slabShapeVertexStart, slabShapeVertexEnd);
                        }
                        catch (Exception e)
                        {
                          //Do nothing
                        }
                    }
                }
                #else
                {
                    var shapeEditor = target.GetSlabShapeEditor();
                    try
                    {
                        slabShapeVertexStart = shapeEditor.DrawPoint(startPoint);
                    }
                    catch (Exception)
                    {
                        //Do nothing
                    }

                    try
                    {
                        slabShapeVertexEnd = shapeEditor.DrawPoint(endPoint);
                    }
                    catch (Exception)
                    {
                        //Do nothing
                    }

                    if (slabShapeVertexStart != null && slabShapeVertexEnd != null)
                    {
                        try
                        {
                            shapeEditor.DrawSplitLine(slabShapeVertexStart, slabShapeVertexEnd);
                        }
                        catch (Exception e)
                        {
                            //Do nothing
                        }
                    }
                }
#endif


            }
        }

        public static ModelCurve DrawModelCurve(Document doc, Curve curve)
        {
            var startPoint = curve.GetEndPoint(0);
            var endPoint = curve.GetEndPoint(1);

            var dir = (endPoint- startPoint).Normalize();
            var xDir = dir.CrossProduct(XYZ.BasisZ);

            double x = dir.X, y = dir.Y, z = dir.Z;
            XYZ n = new XYZ(z - y, x - z, y - x);

            var geometryPlane = Plane.CreateByNormalAndOrigin(xDir, startPoint);
            if (curve.GetType() == typeof(Arc))
            {
                var arc = (Arc)curve;
                geometryPlane = Plane.CreateByNormalAndOrigin(arc.Normal, startPoint);
            }

            var sketchPlane = SketchPlane.Create(doc, geometryPlane);
            return doc.Create.NewModelCurve(curve, sketchPlane);

        }

        public static DetailCurveArray CreateDetailLinesFromModelLines(Document doc, List<Line> curves)
        {
            var modelCurves = new ModelCurveArray();

            foreach (var curve in curves)
            {
                modelCurves.Append(DrawModelCurve(doc, curve));
            }

            var view = new FilteredElementCollector(doc).OfClass(typeof(ViewPlan)).WhereElementIsNotElementType()
                .FirstElement();

            return doc.ConvertModelToDetailCurves((View)view, modelCurves);

        }

        public static List<XYZ> IntersectLines(DetailCurveArray curveSet1, DetailCurveArray curveSet2)
        {
            return _intersectLines(curveSet1, curveSet2);
        }

        public static List<XYZ> IntersectLines(List<Curve> curveSet1, List<Curve> curveSet2)
        {
            return _intersectLines(curveSet1, curveSet2);
        }


        private static List<XYZ> _intersectLines(dynamic curveSet1, dynamic curveSet2)
        {
            var pointList = new List<XYZ>();

            foreach (var curve in curveSet1)
            {
                foreach (var curve2 in curveSet2)
                {
                    var results = new IntersectionResultArray();
                    var intersectionResult = curve.Intersect(curve2, out results);

                    if (intersectionResult != SetComparisonResult.Disjoint)
                    {
                        try
                        {
                            var intersectionEdges = results.get_Item(0);
                            pointList.Add(intersectionEdges.XYZPoint);

                        }
                        catch (Exception)
                        {
                            //Do nothing
                        }

                    }

                }
            }

            return pointList;

        }

        //this can for sure be rolled into the other method
        public static List<List<XYZ>> IntersectListOfLines(DetailCurveArray lineList1, DetailCurveArray lineList2)
        {
            var parentListOfPoints = new List<List<XYZ>>();

            foreach (var line1 in lineList1)
            {
                var line1Converted = (DetailCurve)line1;
                var flatPointsList = new List<XYZ>();
                foreach (var line2 in lineList2)
                {
                    var line2Converted = (DetailCurve)line2;

                    var results = new IntersectionResultArray();
                    var intersectionResult = line1Converted.GeometryCurve.Intersect(line2Converted.GeometryCurve, out results);

                    if (intersectionResult != SetComparisonResult.Disjoint)
                    {
                        try
                        {
                            if (results != null)
                            {
                                var intersectionEdges = results.get_Item(0);
                                flatPointsList.Add(intersectionEdges.XYZPoint);
                            }
                            

                        }
                        catch (Exception)
                        {
                            //Do nothing
                        }

                    }
                }
                parentListOfPoints.Add(flatPointsList);
            }
            return parentListOfPoints;
        }


        //Heavy Chatgpt, check thoroughly
        public static BoundaryFaceIntersections IntersectBoundariesWithFaceEdges(Document doc, ModelCurveArray boundaryModelCurves,
            List<Curve> faceEdges)
        {
            //Create model curves from face edges
            var modelCurves = new ModelCurveArray();

            foreach (var edge in faceEdges)
            {
                modelCurves.Append(DrawModelCurve(doc, edge));
            }

            //Get a random plan view, doesn't matter what one. Need to explicity filter for templates
            var viewElement = new FilteredElementCollector(doc).OfClass(typeof(ViewPlan))
                .WhereElementIsNotElementType().Cast<ViewPlan>().FirstOrDefault(viewPlan => !viewPlan.IsTemplate);
            
            var view = (View)viewElement;
            //flatten the model curves to detail lines
            var flatLines = new DetailCurveArray();
            try
            {
                flatLines = doc.ConvertModelToDetailCurves(view, modelCurves);
            }
            catch (Exception e)
            {
                //do nothing
            }
            var flatBoundaries = doc.ConvertModelToDetailCurves(view, boundaryModelCurves);

            //find intersecting points from lines
            var flatIntersectingPoints = IntersectListOfLines(flatLines, flatBoundaries);

            //find pairs of lines that intersect
            var intersectingLines = new List<Curve>();

            foreach (var (listOfPts, edge) in flatIntersectingPoints.Zip(faceEdges, Tuple.Create))
            {
                var listLen = listOfPts.Count;
                // not a pair of intersecting lines if count < 2
                if (listLen > 1)
                {
                    // find where the line pairs intersect, and where that point is relative to one of the curves
                    List<double> paramOnEdge = new List<double>();
                    List<XYZ> projectedPts = new List<XYZ>();

                    foreach (var pt in listOfPts)
                    {
                        Line unboundLine = Line.CreateUnbound(pt, XYZ.BasisZ);
                        IntersectionResultArray intResults;
                        edge.Intersect(unboundLine, out intResults);
                        IntersectionResult intersectionResult = intResults.get_Item(0);

                        XYZ intersectionPoint = intersectionResult.XYZPoint;
                        projectedPts.Add(intersectionPoint);

                        IntersectionResult intersection = edge.Project(intersectionPoint);
                        paramOnEdge.Add(intersection.Parameter);
                    }

                    // sort the list of points based on where they land along the curve
                    var sortedList = paramOnEdge.Zip(projectedPts, (param, pts) => new { param, pts })
                        .OrderBy(x => x.param)
                        .Select(x => x.pts)
                        .ToList();

                    // Create a line segment between all ordered points in the list
                    for (int i = 0; i < sortedList.Count - 1; i++)
                    {
                        XYZ pt1 = sortedList[i];
                        XYZ pt2 = sortedList[i + 1];

                        //TODO check why too small lines get created
                        try
                        {
                            intersectingLines.Add(Line.CreateBound(pt1, pt2));
                        }
                        catch (Exception e)
                        {
                           //pass
                        }
                        
                    }
                }
            }

            return new BoundaryFaceIntersections(intersectingLines, flatIntersectingPoints);

        }

        public static List<Face> GetTopFaces(FaceArray faces)
        {
            var topFaces = new List<Face>();

            foreach (var face in faces)
            {
                var convertedFace = (Face)face;
                var normal = convertedFace.ComputeNormal(new UV(0.5, 0.5));
                if (normal[2] > 0)
                {
                    topFaces.Add(convertedFace);
                }
            }

            return topFaces;
        }

        public static List<Curve> ProjectCurvesVerticallyToFaces(List<Face> faces, List<Curve> curveList)
        {
            var projectedCurves = new List<Curve>();
            foreach (var curve in curveList)
            {
                var startPoint = curve.GetEndPoint(0);
                var endPoint = curve.GetEndPoint(1);

                XYZ projectedStart = null;
                XYZ projectedEnd = null;

                foreach (Face face in faces)
                {
                    var startPt = ProjectPointToFace(startPoint, face);
                    if (startPt != null)
                    {
                        projectedStart = startPt;
                    }
                    var endPt = ProjectPointToFace(endPoint, face);
                    if (endPt != null)
                    {
                        projectedEnd = endPt;
                    }

                }

                if (projectedStart != null && projectedEnd != null)
                {
                    projectedCurves.Add(Line.CreateBound(startPoint, endPoint));
                }
            }
            return projectedCurves;

        }

        private static XYZ ProjectPointToFace(XYZ point, Face face)
        {
            XYZ projectedPoint = null;
            var line = Line.CreateUnbound(point, XYZ.BasisZ);
            var results = new IntersectionResultArray();
            var result = face.Intersect(line, out results);

            if (result != SetComparisonResult.Disjoint)
            {
                try
                {
                    var intersection = results.get_Item(0);
                    projectedPoint = intersection.XYZPoint;

                }
                catch (Exception)
                {
                    //Do nothing
                }
            }
            return projectedPoint;
        }


        public static PointProjectionResult ProjectPointVerticallyToFaces(List<Face> faces, List<XYZ> pointList, double verticalOffset = 0.0)
        {
            var projectedPoints = new List<XYZ>();
            var missingPoints = pointList.ToList();
            foreach (Face face in faces)
            {
                foreach (var boundaryPoint in pointList)
                {
                    var projectedPoint = ProjectPointToFace(boundaryPoint, face);
                    if (projectedPoint != null)
                    {
                        projectedPoints.Add(projectedPoint+ new XYZ(0,0, verticalOffset));
                        missingPoints.Remove(boundaryPoint);
                    }

                }
            }

            var results = new PointProjectionResult(projectedPoints, missingPoints);
            return results;
        }

        public static List<XYZ> FindClosestPointsFromListOfPoints(List<XYZ> inputPoints, List<XYZ> listOfPointsToReadFrom, double verticalOffset = 0.0)
        {
            //This is somewhat gnarly, there's probably a more efficient way, but it's probably not worth the complexity / effort

            var matchedPoints = new List<XYZ>();
            var cleanedListOfPoints = listOfPointsToReadFrom.ToList();

            foreach (var point in listOfPointsToReadFrom)
            {
                foreach (var inputPoint in inputPoints)
                {
                    if (inputPoint.IsAlmostEqualTo(point))
                    {
                        cleanedListOfPoints.Remove(point);
                    }
                } 
            }

            foreach (var missingPoint in inputPoints)
            {
                XYZ? closestPoint = null;
                var minDistance = double.MaxValue;
                var sortedPoints = cleanedListOfPoints.OrderBy(p => p.X).ToList();

                foreach (var sortedPoint in sortedPoints)
                {
                    var distanceAlongAxis = Math.Abs(missingPoint.X - sortedPoint.X);

                    // Early exit if the closest possible distance along the axis is already greater than minDistance
                    if (distanceAlongAxis >= minDistance)
                    {
                        break;
                    }

                    var distance = missingPoint.DistanceTo(sortedPoint);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestPoint = sortedPoint;
                    }
                }

                if (closestPoint != null)
                {
                    var newPoint = new XYZ(missingPoint.X, missingPoint.Y, closestPoint.Z + verticalOffset);
                    matchedPoints.Add(newPoint);
                }
            }
            return matchedPoints;
        }
        public static List<Curve> UseFilledRegionToTrimLines(Document doc, List<CurveLoop> boundaryEdgeLoops,
            List<List<XYZ>> intersectingPoints, List<Curve> edges)
        {
            var tg = new TransactionGroup(doc, "Temporary Filled Region");
            tg.Start();
            var t = new Transaction(doc, "Create Temp Filled Region");
            t.Start();

            var view = new FilteredElementCollector(doc).OfClass(typeof(ViewPlan)).WhereElementIsNotElementType().Cast<ViewPlan>().FirstOrDefault(viewPlan => !viewPlan.IsTemplate).Id;
            var filledRegionType =
                new FilteredElementCollector(doc).OfClass(typeof(FilledRegionType)).FirstElement().Id;

            var filledRegion = FilledRegion.Create(doc, filledRegionType, view, boundaryEdgeLoops);
            t.Commit();

            var opts = new Options();
            opts.View = (View)doc.GetElement(view);

            var geometry = filledRegion.get_Geometry(opts);
            Face filledRegionFace = null;

            foreach (var g in geometry)
            {
                //this is for sure going to fail, guessed the data type
                var gConversion = (Solid)g;
                var faceArray = gConversion.Faces;

                foreach (var f in faceArray)
                {
                    var fConversion = (Face)f;
                    filledRegionFace = fConversion;
                }
            }

            var newEdges = new List<Curve>();

            foreach (var edgePoints in intersectingPoints)
            {
                foreach (var point in edgePoints)
                {
                    var rayLine = Line.CreateUnbound(point, new XYZ(0, 0, -1));
                    foreach (var edge in edges)
                    {
                        var boundaryIntersectionResults = new IntersectionResultArray();
                        var boundaryIntersectionResult = filledRegionFace.Intersect(rayLine, out boundaryIntersectionResults);
                        if (boundaryIntersectionResult != SetComparisonResult.Disjoint)
                        {
                            try
                            {
                                var start = edge.GetEndPoint(0);
                                var startRay = Line.CreateUnbound(start, XYZ.BasisZ);

                                var startIntersectionResults = new IntersectionResultArray();
                                ;
                                var startIntersection =
                                    filledRegionFace.Intersect(startRay, out startIntersectionResults);

                                XYZ trimmedEnd = null;
                                if (startIntersection != SetComparisonResult.Disjoint)
                                {
                                     trimmedEnd = start;
                                }
                                else
                                {
                                    trimmedEnd = edge.GetEndPoint(1);
                                }

                                var trimmedStart = boundaryIntersectionResults.get_Item(0).XYZPoint;

                                var newRayLine = Line.CreateUnbound(trimmedStart, new XYZ(0, 0, -1));

                                var newIntersectionResults = new IntersectionResultArray();
                                var newIntersectionResult = edge.Intersect(newRayLine, out newIntersectionResults);
                                if (newIntersectionResult != SetComparisonResult.Disjoint)
                                {
                                    var newStart = newIntersectionResults.get_Item(0).XYZPoint;
                                    newEdges.Add(Line.CreateBound(newStart, trimmedEnd));
                                }
                                
                                //newEdges.Add(Line.CreateBound(trimmedStart, trimmedEnd));

                            }
                            catch (Exception)
                            {
                                //Do nothing
                            }
                        }
                    }

                }
            }

            tg.RollBack();

            return newEdges;

        }

        public static List<Curve> UseBoundaryCurvesToMakeSolidToTrimLines(Document doc, List<CurveLoop> boundaryEdgeLoops,
            List<Curve> edges, double verticalOffset =0.0)
        {
            var distance = 10000;
            var solid = GeometryCreationUtilities.CreateExtrusionGeometry(boundaryEdgeLoops, XYZ.BasisZ, distance);
            var transform = Transform.CreateTranslation(new XYZ(0,0, -(distance/2)));
            var translatedSolid = SolidUtils.CreateTransformed(solid, transform);

            var boundingBox = translatedSolid.GetBoundingBox();



            var edgeList = new List<Curve>();


            var intlist = new List<SolidCurveIntersection>();
            foreach (var edge in edges)
            {
                if (_isCurveWithinBoundingBox(edge, boundingBox))
                {
                    edgeList.Add(edge);
                }
                //var intersection = translatedSolid.IntersectWithCurve(edge, new SolidCurveIntersectionOptions());
                //if (intersection.ResultType == SolidCurveIntersectionMode.CurveSegmentsInside)
                //{
                //    var count = intersection.SegmentCount;
                //    var oi = intersection.GetEnumerator();
                //    if (count > 0)
                //    {
                //        var intCrv = intersection.GetCurveSegment(0);
                //        intlist.Add(intersection);
                //    }

                //}
            }

            var viableEdges = new List<Curve>();
            foreach (var edge in edges)
            {
                var intersection = translatedSolid.IntersectWithCurve(edge, new SolidCurveIntersectionOptions());
                if (intersection.ResultType == SolidCurveIntersectionMode.CurveSegmentsInside)
                {
                    var count = intersection.SegmentCount;
                    var enumerator = intersection.GetEnumerator();
                    intlist.Add(intersection);
                    if (count > 0)
                    {
                        while (enumerator.MoveNext())
                        {
                            var currrentIntersectionResult = enumerator.Current;
                            var midpoint = currrentIntersectionResult.Evaluate(0.5, false);
                            var midpointMoved = midpoint + new XYZ(0, 0, 0.1);
                            var checkLine = Line.CreateBound(midpoint, midpointMoved);

                            var segmentIntersection =
                                translatedSolid.IntersectWithCurve(checkLine, new SolidCurveIntersectionOptions());

                            if (segmentIntersection.ResultType == SolidCurveIntersectionMode.CurveSegmentsInside)
                            {
                                var transformOffsetVertically = Transform.CreateTranslation(new XYZ(0, 0,verticalOffset));
                                var transformedCurve = currrentIntersectionResult.CreateTransformed(transformOffsetVertically);
                                viableEdges.Add(transformedCurve);
                            }
                        }
                    }
                }
            }

            return viableEdges;
        }

        private static bool _isPointWithinBoundingBox(XYZ point, XYZ minPoint, XYZ maxPoint)
        {
            return (point.X >= minPoint.X && point.X <= maxPoint.X &&
                    point.Y >= minPoint.Y && point.Y <= maxPoint.Y &&
                    point.Z >= minPoint.Z && point.Z <= maxPoint.Z);
        }

        public static List<Curve> SortCurvesContiguously(List<Curve> curves)
        {
            if (curves == null || curves.Count == 0)
                return curves;

            // Start with the first curve
            List<Curve> sortedCurves = new List<Curve>();
            Curve currentCurve = curves[0];
            sortedCurves.Add(currentCurve);
            curves.RemoveAt(0);

            var isBound = currentCurve.IsBound;

            if (!isBound)
            {
                var oi = 2;
            }

            var isClosed = currentCurve.IsClosed;
            {
                var oii = 2;
            }



            XYZ currentEndPoint = currentCurve.GetEndPoint(1); // End point of the current curve

            // Loop until all curves are sorted
            while (curves.Count > 0)
            {
                // Find the next curve that starts or ends at the current endpoint
                Curve nextCurve = null;
                bool shouldReverse = false;
                for (int i = 0; i < curves.Count; i++)
                {
                    Curve candidate = curves[i];
                    if (candidate.GetEndPoint(0).IsAlmostEqualTo(currentEndPoint))
                    {
                        nextCurve = candidate;
                        shouldReverse = false;
                        break;
                    }
                    else if (candidate.GetEndPoint(1).IsAlmostEqualTo(currentEndPoint))
                    {
                        nextCurve = candidate;
                        shouldReverse = true;
                        break;
                    }
                }

                // If no matching curve is found, break out of the loop
                if (nextCurve == null)
                {
                    break;
                }

                // Reverse the curve if needed
                if (shouldReverse)
                {
                    nextCurve = nextCurve.CreateReversed();
                }

                // Add the next curve to the sorted list
                sortedCurves.Add(nextCurve);
                curves.Remove(nextCurve);

                // Update the current end point
                currentEndPoint = nextCurve.GetEndPoint(1);
            }

            return sortedCurves;
        }

    private static bool _isCurveWithinBoundingBox(Curve curve, BoundingBoxXYZ boundingBox, int numSamples = 10)
        {
            // Get the minimum and maximum points of the bounding box
            XYZ minPoint = boundingBox.Min;
            XYZ maxPoint = boundingBox.Max;

            // Check start and end points
            if (_isPointWithinBoundingBox(curve.GetEndPoint(0), minPoint, maxPoint) ||
                _isPointWithinBoundingBox(curve.GetEndPoint(1), minPoint, maxPoint))
            {
                return true;
            }


            // For more complex curves (e.g., splines), sample points along the curve
            for (int i = 1; i < numSamples; i++)
            {
                double parameter = curve.GetEndParameter(0) + (curve.GetEndParameter(1) - curve.GetEndParameter(0)) * i / (double)numSamples;
                XYZ point = curve.Evaluate(parameter, false);
                if (_isPointWithinBoundingBox(point, minPoint, maxPoint))
                {
                    return true;
                }
            }

            // No points are within the bounding box
            return false;
        }

        public static List<Face> ConvertTopographyToFaces(Element topographyElement)
        {
            var geo = topographyElement.get_Geometry(new Options());
            Mesh mesh = null;

            foreach (var g in geo)
            {
                if (g.GetType() == typeof(Mesh))
                {
                    mesh = (Mesh)g;
                }
            }

            int triangleCounter = 0;
            var meshTriangles = mesh.NumTriangles;
            var newFaces = new List<Face>();

            while (triangleCounter != (meshTriangles-1))
            {
                var triangle = mesh.get_Triangle(0);
                var pt0 = triangle.get_Vertex(0);
                var pt1 = triangle.get_Vertex(1);
                var pt2 = triangle.get_Vertex(2);

                var line0 = Line.CreateBound(pt0, pt1);
                var line1 = Line.CreateBound(pt1, pt2);
                var line2 = Line.CreateBound(pt2, pt0);

                var curveloop = new CurveLoop();
                curveloop.Append(line0);
                curveloop.Append(line1);
                curveloop.Append(line2);

                var curveLoopList = new List<CurveLoop>();
                curveLoopList.Add(curveloop);

                var newGeo = GeometryCreationUtilities.CreateExtrusionGeometry(curveLoopList, new XYZ(0, 0, -1), 1);
                var faces = newGeo.Faces;
                var topFace = GetTopFaces(faces);
                newFaces.Add(topFace[0]);


                triangleCounter += 1;
            }

            return newFaces;

        }

    }
}