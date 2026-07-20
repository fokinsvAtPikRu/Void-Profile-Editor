using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Void_Profile_Editor.Domain.Model.Geometry;
using Autodesk.Revit.DB.Structure;
using Void_Profile_Editor.Domain.Abstraction.Configuration;
using Void_Profile_Editor.Infrastructure.Model;


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
        public static Result<RevitPressureContour> ToDomain(this FamilyInstance instance, IAllowedFamiliesConfig config)
        {
            if (instance == null)
                return Result.Failure<RevitPressureContour>("Adapter Revit to Domain PressureContour: instsnce == null");
            if (config == null)
                return Result.Failure<RevitPressureContour>("Adapter Revit to Domain PressureContour: config == null");
            var familyName = instance.Symbol?.FamilyName;
            if (string.IsNullOrEmpty(familyName))
                return Result.Failure<RevitPressureContour>("Adapter Revit to Domain PressureContour: familyName is null or empty");
            // получаем имена параметров из конфига
            var result = config.GetParameterNamesForFamily(familyName);
            if (result.IsFailure)
                return Result.Failure<RevitPressureContour>(result.Error);
            var parameters = result.Value;
            var doubleParameters = new Dictionary<string, double>();
            var intParameters = new Dictionary<string, int>();
            var missingParameters = new List<string>();
            var typeMismatchParameters = new List<string>();
            foreach (var key in parameters.DoubleParameters.Keys)
            {
                var value = instance.LookupParameter(key);
                if (value == null)
                {
                    missingParameters.Add(key);
                    continue;
                }
                if (value.StorageType == StorageType.Double)
                {
                    doubleParameters.Add(key, value.AsDouble());                   
                }
                else
                {
                    typeMismatchParameters.Add($"{key} ожидался Double, получен {value.StorageType}");
                }
            }
            foreach (var key in parameters.IntParameters.Keys)
            {
                var value = instance.LookupParameter(key);
                if (value == null)
                {
                    missingParameters.Add(key);
                    continue;
                }
                if (value.StorageType == StorageType.Integer)
                {
                    intParameters.Add(key, value.AsInteger());                    
                }
                else
                {
                    typeMismatchParameters.Add($"{key} ожидался Integer, получен {value.StorageType}");
                }
            }
            return new RevitPressureContour(
                instance.Id.ToDomain(),
                instance.Name,

                )
            {
                Id = instance.Id.ToDomain(),
                FamilyName=familyName,
                FamilyType=parameters.
                InsertPoint = ((LocationPoint)instance.Location).Point.ToDomain(),
                Rotation = ((LocationPoint)instance.Location).Rotation,
                ContourParameters = new PressureContourParameters(
                    parameters.FamilyName,
                    parameters.ActiveEdge,
                    doubleParameters,
                    intParameters),
                IsMirrored = instance.Mirrored
            };
        }
    }
}

