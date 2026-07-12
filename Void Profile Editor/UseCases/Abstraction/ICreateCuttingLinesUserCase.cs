using CSharpFunctionalExtensions;
using Void_Profile_Editor.Domain.Model.Geometry;

namespace Void_Profile_Editor.UserCases.Abstraction
{
    public interface ICreateCuttingLinesUserCase
    {
        Result CreateCuttingLines(Contour contourHalfH0, PressureContour pressureContour);
    }
}
