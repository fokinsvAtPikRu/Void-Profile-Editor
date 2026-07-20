using CSharpFunctionalExtensions;
using System.Threading.Tasks;
using Void_Profile_Editor.Domain.Model.Geometry;
using Void_Profile_Editor.Infrastructure.Model;



namespace Void_Profile_Editor.Infrastructure.Abstraction
{
    public interface IRevitSelectionServices
    {        
            Task<Result<Point3DDomain>> PickPointAsync(string prompt);
            Task<Result<RevitPressureContour>> PickFamilyInstanceAsync(string prompt);        
    }
}
