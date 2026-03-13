using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Void_Profile_Editor.Abstraction
{
    public interface IDrawLineService
    {
        CSharpFunctionalExtensions.Result DrawLine(Line line,
            Transaction transaction = null,
            View view = null,
            string lineStyleName = "Тонкие линии",
            ObservableCollection<ElementId> createdLineIds = null);
        CSharpFunctionalExtensions.Result DeleteLines(
                ObservableCollection<ElementId> lineIds,
                Transaction transaction = null);
    }
}
