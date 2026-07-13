using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Void_Profile_Editor.Domain.Model.Geometry;

namespace Void_Profile_Editor.Domain.Abstraction.Services
{
    public interface ICreateContourService
    {
        CSharpFunctionalExtensions.Result<Contour> Create
            (string familyName,
            Point3DDomain locationPoint,
            double rotationAngle,
            double h0,
            double thickness,
            double offset,
            bool isMirrored);
        CSharpFunctionalExtensions.Result<List<string>> DrawContour(Contour contour);

    }
}
