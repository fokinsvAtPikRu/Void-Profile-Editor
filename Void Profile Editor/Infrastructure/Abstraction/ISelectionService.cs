using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Void_Profile_Editor.Domain.Model.Geometry;

namespace Void_Profile_Editor.Infrastructure.Abstraction
{
    public interface ISelectionService
    {
        CSharpFunctionalExtensions.Result<FamilyInstance> PickObject();
        CSharpFunctionalExtensions.Result<Point3DDomain> PickPoint();
    }
}
