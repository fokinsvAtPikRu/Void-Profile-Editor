using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;
using System.Threading.Tasks;
using Void_Profile_Editor.Domain.Model.Geometry;



namespace Void_Profile_Editor.Infrastructure.Abstraction
{
    public interface IRevitSelectionServices
    {        
            Task<Result<Point3DDomain>> PickPointAsync(string prompt);
            Task<Result<PressureContour>> PickFamilyInstanceAsync(string prompt);        
    }
}
