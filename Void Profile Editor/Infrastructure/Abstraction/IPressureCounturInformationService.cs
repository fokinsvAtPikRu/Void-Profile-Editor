using Autodesk.Revit.DB;
using Void_Profile_Editor.Domain.Model.Geometry;

namespace Void_Profile_Editor.Infrastructure.Abstraction
{
    public interface IPressureCounturInformationService
    {        
        Point3DDomain GetCenterPressureContur(PressureContour contour);
        CSharpFunctionalExtensions.Result UpdateParameters(PressureContour pressureContour);
    }
}
