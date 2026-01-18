using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Void_Profile_Editor.Model;

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
            Autodesk.Revit.DB.Document doc = _commandData.Application.ActiveUIDocument.Document;
            PressureContour contour = new PressureContour()
            {
                Id = instance.Id,
                H0 = instance.LookupParameter("h0").AsDouble(),
                WallThickness = instance.LookupParameter("Толщина").AsDouble(),
                InsertPoint = ((LocationPoint)instance.Location).Point,
                Rotation = ((LocationPoint)instance.Location).Rotation
            };
            return contour;
        }
        // код дублируется, вынести куда то
        public CSharpFunctionalExtensions.Result<XYZ> GetCenterPressureContur(PressureContour contour)
        {
            XYZ center = new XYZ(
                contour.InsertPoint.X,
                contour.InsertPoint.Y + contour.WallThickness + contour.H0 / 2,
                0);
            // Создаем матрицу вращения
            Transform rotation = Transform.CreateRotation(XYZ.BasisZ, contour.Rotation);

            // Смещаем точку относительно центра вращения
            XYZ translatedPoint = center - contour.InsertPoint;

            // Поворачиваем точку
            XYZ rotatedTranslatedPoint = rotation.OfPoint(translatedPoint);

            // Возвращаем точку в исходную систему координат
            return rotatedTranslatedPoint + center;
        }
    }
}
