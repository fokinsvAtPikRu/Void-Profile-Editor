using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;
using Void_Profile_Editor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Creation;

namespace Void_Profile_Editor.Services
{
    public class PressureCounturInformationService
    {
        private ExternalCommandData _commandData;
        public PressureCounturInformationService(ExternalCommandData commandData)
        {
            _commandData = commandData;
        }

        public CSharpFunctionalExtensions.Result<PressureContour> CreatePressureConturInfo(FamilyInstance instance)
        {
            Autodesk.Revit.DB.Document doc=_commandData.Application.ActiveUIDocument.Document;
            PressureContour contour = new PressureContour()
            {
                Id = instance.Id,
                H0 = instance.LookupParameter("h0").AsDouble(),
                WallThickness = instance.LookupParameter("Тодщина").AsDouble(),
                InsertPoint = ((LocationPoint)instance.Location).Point,
                Rotation = ((LocationPoint)instance.Location).Rotation
            };
            return CSharpFunctionalExtensions.Result.Success(contour);
        }
    }
}
