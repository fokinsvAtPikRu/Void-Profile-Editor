using System;
using CSharpFunctionalExtensions;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Void_Profile_Editor.Model;

namespace Void_Profile_Editor.Services
{
    public class Create6H0ContourService
    {
        public CSharpFunctionalExtensions.Result<Contour6H0> Create
            (XYZ locationPoint,
            double rotationAngle,
            double h0,
            double thickness)
        {
            // вычисляем координаты углов контура
            XYZ bottomRight = new XYZ
                (locationPoint.X + thickness / 2 + 6 * h0,
                locationPoint.Y - 6 * h0,
                0);            
            XYZ bottomLeft = new XYZ
                (locationPoint.X - thickness / 2 - 6 * h0,
                locationPoint.Y - 6 * h0,
                0);
            XYZ topRight = new XYZ
                (locationPoint.X + thickness / 2 + 6 * h0,
                locationPoint.Y + thickness + 0.5 * h0,
                0);
            XYZ topLeft = new XYZ
                (locationPoint.X - thickness / 2 - 6 * h0,
                locationPoint.Y + thickness + 0.5 * h0,
                0);
            // поворачиваем контур
            bottomRight = RotatePointAroundAxis(bottomRight, locationPoint, XYZ.BasisZ, rotationAngle);
            bottomLeft = RotatePointAroundAxis(bottomLeft, locationPoint, XYZ.BasisZ, rotationAngle);
            topRight = RotatePointAroundAxis(topRight, locationPoint, XYZ.BasisZ, rotationAngle);
            topLeft = RotatePointAroundAxis(topLeft, locationPoint, XYZ.BasisZ, rotationAngle);
            Contour6H0 contour = new Contour6H0()
            {
                Bottom = Line.CreateBound(bottomRight, bottomLeft),
                Left = Line.CreateBound(topLeft, bottomLeft),
                Right = Line.CreateBound(topRight, bottomRight)
            };
            return contour;
        }
        /// <summary>
        /// Поворот точки относительно оси Z проходящей через center на угол angle в радианах
        /// </summary>
        /// <param name="point">точка которую нужно повернуть</param>
        /// <param name="center">ось поворота</param>
        /// <param name="axis">направление оси поворота</param>
        /// <param name="angle">угол в радианах</param>
        /// <returns></returns>
        private XYZ RotatePointAroundAxis(XYZ point, XYZ center, XYZ axis, double angle)
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
