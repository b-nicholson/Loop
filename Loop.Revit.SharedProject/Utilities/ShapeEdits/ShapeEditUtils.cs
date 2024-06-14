

using System;
using System.Collections.Generic;
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


    }
}





