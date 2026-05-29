using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Void_Profile_Editor.Domain.Model.Geometry;

namespace Void_Profile_Editor.Domain.Abstraction.Services
{
    public interface IGeometryService
    {
        Point3DDomain RotatePointAroundAxis(Point3DDomain point, Point3DDomain center, Point3DDomain axis, double angle);
        CSharpFunctionalExtensions.Result<IntersectionPoint[]> LineWithContourIntersection(Line[] lines, Contour contour);
        CSharpFunctionalExtensions.Result CalculateParameters(Contour contourHalfH0, IntersectionPoint[] points, PressureContour pressureContour);
    }
}
