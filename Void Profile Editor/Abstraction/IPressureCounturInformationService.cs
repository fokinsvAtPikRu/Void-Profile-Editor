using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Void_Profile_Editor.Model;

namespace Void_Profile_Editor.Abstraction
{
    public interface IPressureCounturInformationService
    {
        CSharpFunctionalExtensions.Result<PressureContour> CreatePressureConturInfo(FamilyInstance instance);
        CSharpFunctionalExtensions.Result<XYZ> GetCenterPressureContur(PressureContour contour);
    }
}
