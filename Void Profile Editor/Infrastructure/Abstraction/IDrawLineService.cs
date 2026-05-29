using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Void_Profile_Editor.Domain.Model.Geometry;

namespace Void_Profile_Editor.Infrastructure.Abstraction
{
    public interface IDrawLineService
    {
        CSharpFunctionalExtensions.Result<List<string>> DrawLines(
            string trMessage,
            List<DetailLineDomain> linesDomain,
            View view = null,
            string lineStyleName = "Тонкие линии");
        CSharpFunctionalExtensions.Result DeleteLines(
                List<string> lineIdString,
                Transaction transaction = null);
    }
}
