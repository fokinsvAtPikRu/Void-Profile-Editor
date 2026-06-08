using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
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
            new Point3DDomain(point.X, point.Y, point.Z);
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
                ContourParameters = new PressureContourParameters(),
                IsMirrored = instance.Mirrored
            };
            var parameters = new PressureContourParameters();
            var missingParameters = new List<string>();
            var typeMismatchParameters= new List<string>();
            foreach (var key in contour.ContourParameters.DoubleParameters.Keys)
            {
                var parametr = instance.LookupParameter(key);
                if (parametr == null)
                {
                    missingParameters.Add(key);
                    continue;
                }
                if (parametr.StorageType == StorageType.Double)
                {
                    parameters.DoubleParameters[key] = parametr.AsDouble();
                }
                else
                {
                    typeMismatchParameters.Add($"{key} ожидался Double, получен {parametr.StorageType}");
                }
            }
            foreach (var key in contour.ContourParameters.IntParameters.Keys)
            {
                var parametr = instance.LookupParameter(key);
                if (parametr == null)
                {
                    missingParameters.Add(key);
                    continue;
                }
                if (parametr.StorageType == StorageType.Integer)
                {
                    parameters.IntParameters[key] = parametr.AsInteger();
                }
                else
                {
                    typeMismatchParameters.Add($"{key} ожидался Integer, получен {parametr.StorageType}");
                }
            }
            contour.ContourParameters = parameters;
            return contour;
        }
    }
}

