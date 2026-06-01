using Autodesk.Revit.DB;
using System;
using System.Xml.Serialization;
using Void_Profile_Editor.Domain.Model.Geometry;


namespace Void_Profile_Editor.Infrastructure.Adapters
{
    public static class GeometryAdapter
    {
        // Domain -> Revit
        public static XYZ ToRevit(this Point3DDomain point) =>
            new XYZ(point.X, point.Y, point.Z);
        public static Line ToRevit(this DetailLineDomain line) =>
            Line.CreateBound(line.Start.ToRevit(), line.End.ToRevit());
        public static ElementId ToRevitId(this string domainId) =>
            int.TryParse(domainId, out var id) ? new ElementId(id) : ElementId.InvalidElementId;


        // Revit -> Domain
        public static Point3DDomain ToDomain(this XYZ point) =>
            new Point3DDomain(point.X,point.Y,point.Z);
        public static DetailLineDomain ToDomain(this Line line) =>
            new DetailLineDomain(line.GetEndPoint(0).ToDomain(), line.GetEndPoint(1).ToDomain());
        public static string ToDomain(this ElementId revitId) =>
            revitId.IntegerValue.ToString();
        public static PressureContour ToDomain(this FamilyInstance instance)
        {            
           
                PressureContour contour = new PressureContour()
                {
                    Id = instance.Id.ToDomain(),
                    InsertPoint = ((LocationPoint)instance.Location).Point.ToDomain(),
                    Rotation = ((LocationPoint)instance.Location).Rotation,
                    ContourParameters = new PressureContourParameters()
                };
                PressureContourParameters parameters = new PressureContourParameters();
                foreach (var key in contour.ContourParameters.DoubleParameters.Keys)
                {
                    var parametr = instance.LookupParameter(key);
                    if (parametr == null)
                    {
                        throw new Exception($"Параметр {key} не найден");
                    }
                    parameters.DoubleParameters[key] = parametr.AsDouble();
                }
                foreach (var key in contour.ContourParameters.IntParameters.Keys)
                {
                    var parametr = instance.LookupParameter(key);
                    if (parametr == null)
                    {
                        throw new Exception($"Параметр {key} не найден");
                    }
                    parameters.IntParameters[key] = parametr.AsInteger();
                }
                contour.ContourParameters = parameters;
                return contour;
            }            
        }
    }

