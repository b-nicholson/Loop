

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Media.Media3D;
using Autodesk.Revit.DB;

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

        public static ModelCurve DrawModelCurve(Document doc, Line line)
        {
            var dir = line.Direction;
            var xDir = dir.CrossProduct(XYZ.BasisZ);
            var startPoint = line.Origin;

            var geometryPlane = Plane.CreateByNormalAndOrigin(xDir, startPoint);
            var sketchPlane = SketchPlane.Create(doc, geometryPlane);
            return doc.Create.NewModelCurve(line, sketchPlane);

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
                var line1Converted = (Line)line1;
                var flatPointsList = new List<XYZ>();
                foreach (var line2 in lineList2)
                {
                    var line2Converted = (Line)line2;

                    var results = new IntersectionResultArray();
                    var intersectionResult = line1Converted.Intersect(line2Converted, out results);

                    if (intersectionResult != SetComparisonResult.Disjoint)
                    {
                        try
                        {
                            var intersectionEdges = results.get_Item(0);
                            flatPointsList.Add(intersectionEdges.XYZPoint);

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
            List<Line> faceEdges)
        {
            //Create model curves from face edges
            var modelCurves = new ModelCurveArray();
            foreach (var edge in faceEdges)
            {
                modelCurves.Append(DrawModelCurve(doc, edge));
            }

            //Get a random plan view, doesn't matter what one
            var viewElement = new FilteredElementCollector(doc).OfClass(typeof(ViewPlan)).WhereElementIsNotElementType()
                .FirstElement();
            var view = (View)viewElement;
            //flatten the model curves to detail lines
            var flatLines = doc.ConvertModelToDetailCurves(view, modelCurves);
            var flatBoundaries = doc.ConvertModelToDetailCurves(view, boundaryModelCurves);

            //find intersecting points from lines
            var flatIntersectingPoints = IntersectListOfLines(flatLines, flatBoundaries);

            //find pairs of lines that intersect
            var intersectingLines = new List<Line>();

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
                        intersectingLines.Add(Line.CreateBound(pt1, pt2));
                    }
                }
            }

            return new BoundaryFaceIntersections(intersectingLines, flatIntersectingPoints);

        }

        public static List<PlanarFace> GetTopFaces(FaceArray faces)
        {
            var topFaces = new List<PlanarFace>();

            foreach (var face in faces)
            {
                var convertedFace = (PlanarFace)face;

                if (convertedFace.FaceNormal[2] > 0)
                {
                    topFaces.Add(convertedFace);
                }
            }

            return topFaces;
        }

        public static List<XYZ> ProjectPointVerticallyToFaces(List<PlanarFace> faces, List<XYZ> pointList)
        {
            var projectedPoints = new List<XYZ>();
            foreach (PlanarFace face in faces)
            {
                foreach (var boundaryPoint in pointList)
                {
                    var line = Line.CreateUnbound(boundaryPoint, XYZ.BasisZ);
                    var results = new IntersectionResultArray();
                    var result = face.Intersect(line, out results);

                    if (result != SetComparisonResult.Disjoint)
                    {
                        try
                        {
                            var intersection = results.get_Item(0);
                            projectedPoints.Add(intersection.XYZPoint);

                        }
                        catch (Exception)
                        {
                           //Do nothing
                        }
                    }

                }
            }
            return projectedPoints;
        }

        public static List<Line> UseFilledRegionToTrimLines(Document doc, List<CurveLoop> boundaryEdgeLoops,
            List<List<XYZ>> intersectingPoints, List<Line> edges)
        {
            var tg = new TransactionGroup(doc, "Temporary Filled Region");
            tg.Start();
            var t = new Transaction(doc, "Create Temp Filled Region");
            t.Start();

            var view = new FilteredElementCollector(doc).OfClass(typeof(ViewPlan)).WhereElementIsNotElementType()
                .FirstElement().Id;
            var filledRegiontype =
                new FilteredElementCollector(doc).OfClass(typeof(FilledRegionType)).FirstElement().Id;

            var filledRegion = FilledRegion.Create(doc, filledRegiontype, view, boundaryEdgeLoops);
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

            var newEdges = new List<Line>();

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
                                newEdges.Add(Line.CreateBound(trimmedStart, trimmedEnd));

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


        public static List<PlanarFace> ConvertTopographyToFaces(Element topographyElement)
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
            var newFaces = new List<PlanarFace>();

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

        }

    }
}