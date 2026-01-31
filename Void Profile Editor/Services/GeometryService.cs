using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Void_Profile_Editor.Model;
using CSharpFunctionalExtensions;
using Void_Profile_Editor.Abstraction;

namespace Void_Profile_Editor.Services
{
    public class GeometryService :IGeometryService
    {        
        /// <summary>
        /// вычисление координаты точки с учетом Rotation вокруг LocationPoint
        /// </summary>
        /// <param name="point">точка которую нужно повернуть</param>
        /// <param name="center">точка вокруг котрой поворачиваем</param>
        /// <param name="axis">направление оси вращения</param>
        /// <param name="angle">угол поворота в радианах</param>
        /// <returns></returns>
        public XYZ RotatePointAroundAxis(XYZ point, XYZ center, XYZ axis, double angle)
        {
            // Создаем матрицу вращения
            Transform rotation = Transform.CreateRotation(axis, angle);

            // Смещаем точку относительно центра вращения
            XYZ translatedPoint = point - center;

            // Поворачиваем точку
            XYZ rotatedTranslatedPoint = rotation.OfPoint(translatedPoint);

            // Возвращаем точку в исходную систему координат
            return rotatedTranslatedPoint + center;
        }

        public CSharpFunctionalExtensions.Result<XYZ> LineWithContourIntersection(Line line, Contour contour, out string contourSide)
        {
            IntersectionResultArray result;
            foreach (var contourLine in contour)
            {
                SetComparisonResult comparison = contourLine.Value.Intersect(line, out result);
                if (result != null)
                {
                    if (result.Size == 1)
                    {
                        contourSide = contourLine.Key;
                        return result.get_Item(0).XYZPoint;
                        
                    }
                }
            }
            contourSide = null;
            return null;
        }
    }
}
