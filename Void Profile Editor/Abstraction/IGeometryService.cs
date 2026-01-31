using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Void_Profile_Editor.Model;

namespace Void_Profile_Editor.Abstraction
{
    public interface IGeometryService
    {
        XYZ RotatePointAroundAxis(XYZ point, XYZ center, XYZ axis, double angle);
        CSharpFunctionalExtensions.Result<XYZ> LineWithContourIntersection(Line line, Contour contour, out string contourSide);
    }
}
