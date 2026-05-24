using Autodesk.Revit.DB;
using Void_Profile_Editor.Domain.Model.Geometry;

namespace Void_Profile_Editor.Domain.Abstraction.Services
{
    public interface IPressureCounturInformationService
    {
        //CSharpFunctionalExtensions.Result<PressureContour> CreatePressureContourInfo(FamilyInstance instance);
        XYZ GetCenterPressureContur(PressureContour contour);
        CSharpFunctionalExtensions.Result UpdateParameters(Document doc, FamilyInstance instance, PressureContourParameters parameters);
    }
}
