using Autodesk.Revit.DB;

namespace Void_Profile_Editor.Services
{
    public class GeometryService
    {
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
    }
}
