using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Void_Profile_Editor.Abstraction
{
    public interface ISelectionService
    {
        CSharpFunctionalExtensions.Result<FamilyInstance> PickObject();
        CSharpFunctionalExtensions.Result<XYZ> PickPoint();
    }
}
