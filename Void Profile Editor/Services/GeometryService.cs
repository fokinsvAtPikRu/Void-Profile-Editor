using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Void_Profile_Editor.Model;
using CSharpFunctionalExtensions;
using Void_Profile_Editor.Abstraction;
using System;
using JetBrains.Annotations;
using System.Windows.Markup.Localizer;

namespace Void_Profile_Editor.Services
{
    public class GeometryService : IGeometryService
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

        public CSharpFunctionalExtensions.Result<IntersectionPoint> LineWithContourIntersection(Line line, Contour contour)
        {
            IntersectionResultArray result;
            foreach (var contourLine in contour)
            {
                SetComparisonResult comparison = contourLine.Value.Intersect(line, out result);
                if (result != null)
                {
                    if (result.Size == 1)
                    {
                        return new IntersectionPoint(result.get_Item(0).XYZPoint, contourLine.Key);
                    }
                    return CSharpFunctionalExtensions.Result.Failure<IntersectionPoint>("Точек пересечения больше одной");
                }
            }
            return CSharpFunctionalExtensions.Result.Failure<IntersectionPoint>("Пересечения с контуром не найдены");
        }



        public CSharpFunctionalExtensions.Result CalculateParameters(Contour contourHalfH0, IntersectionPoint[] points, PressureContour pressureContour)
        {
            var parameters = pressureContour.ContourParameters;
            double distance;
            double offset;

            parameters.IntParameters["Вкл редактирование контура"] = 1;

            switch (points[0].SideName)
            {
                // если первое пересечение с левой гранью контура
                case ContourSideName.Left:
                    switch (points[1].SideName)
                    {
                        // если второе пересечение с верхней гранью контура - задаем отступ от верхней грани
                        case ContourSideName.NoIntersection:
                            // если вторая точка ближе к левой стороне
                            if (points[1].Point.DistanceTo(contourHalfH0.Left.GetEndPoint(0)) <
                                points[1].Point.DistanceTo(contourHalfH0.Right.GetEndPoint(1)))
                            //задаем смещение левой грани сверху
                            {
                                distance = CalculateDistance(points[0].Point, contourHalfH0.Left.GetEndPoint(0));
                                SetOffsetFromEdge(parameters, "Ст1.отступ линии от Ст4", distance);
                            }
                            // если вторая точка ближе к правой стороне - задаем смещение левой грани снизу и выключаем нижнюю и левую стороны
                            else
                            {
                                distance = CalculateDistance(points[0].Point, contourHalfH0.Left.GetEndPoint(1));
                                SetOffsetFromEdge(parameters, "Ст1.отступ линии от Ст2", distance);
                                parameters.IntParameters["Вкл сторона 2"] = 0;
                                parameters.IntParameters["Вкл сторона 3"] = 0;
                            }
                            break;
                        // если второе пересечение с левой гранью контура - задаем отверстие на левой грани
                        case ContourSideName.Left:
                            distance = CalculateDistance(points[0].Point, points[1].Point);
                            offset = CalculateOffset(points[0].Point, points[1].Point, contourHalfH0.Left.GetEndPoint(0));
                            SetHoleOnEdge(parameters, "Ст1.ширина отверстия", "Ст1.смещение отверстия от Ст4", distance, offset);
                            break;
                        // если второе пересечение с нижней гранью контура - задаем смещения для левой и нижней граней
                        case ContourSideName.Bottom:
                            distance = CalculateDistance(points[0].Point, contourHalfH0.Left.GetEndPoint(1));
                            SetOffsetFromEdge(parameters, "Ст1.отступ линии от Ст2", distance);
                            distance = CalculateDistance(points[1].Point, contourHalfH0.Bottom.GetEndPoint(0));
                            SetOffsetFromEdge(parameters, "Ст2.отступ линии от Ст1", distance);
                            break;
                        // если второе пересечение с правой стороной - смещение левой и правой грани снизу, выключаем нижнюю сторону 
                        case ContourSideName.Right:
                            distance = CalculateDistance(points[0].Point, contourHalfH0.Left.GetEndPoint(1));
                            SetOffsetFromEdge(parameters, "Ст1.отступ линии от Ст2", distance);
                            distance = CalculateDistance(points[1].Point, contourHalfH0.Right.GetEndPoint(0));
                            SetOffsetFromEdge(parameters, "Ст3.отступ линии от Ст2", distance);
                            parameters.IntParameters["Вкл сторона 2"] = 0;
                            break;
                    }
                    break;
                case ContourSideName.Bottom:
                    switch (points[1].SideName)
                    {
                        // если второе пересечение с верхней гранью контура 
                        case ContourSideName.NoIntersection:
                            // если вторая точка ближе к левой стороне
                            if (points[1].Point.DistanceTo(contourHalfH0.Left.GetEndPoint(0)) <
                                points[1].Point.DistanceTo(contourHalfH0.Right.GetEndPoint(1)))
                            //задаем смещение левой грани сверху
                            {
                                distance = CalculateDistance(points[0].Point, contourHalfH0.Bottom.GetEndPoint(0));
                                SetOffsetFromEdge(parameters, "Ст2.отступ линии от Ст1", distance);
                                parameters.IntParameters["Вкл сторона 1"] = 0;
                            }
                            // если вторая точка ближе к правой стороне - задаем смещение левой грани снизу и выключаем нижнюю и левую стороны
                            else
                            {
                                distance = CalculateDistance(points[0].Point, contourHalfH0.Bottom.GetEndPoint(1));
                                SetOffsetFromEdge(parameters, "Ст2.отступ линии от Ст3", distance);
                                parameters.IntParameters["Вкл сторона 3"] = 0;
                            }
                            break;
                        // если второе пересечение с левой гранью контура - задаем смещение левой снизу и нижней слева
                        case ContourSideName.Left:
                            distance = CalculateDistance(points[1].Point, contourHalfH0.Left.GetEndPoint(1));
                            SetOffsetFromEdge(parameters, "Ст1.отступ линии от Ст2", distance);
                            distance = CalculateDistance(points[0].Point, contourHalfH0.Bottom.GetEndPoint(0));
                            SetOffsetFromEdge(parameters, "Ст2.отступ линии от Ст1", distance);
                            break;
                        // если второе пересечение с нижней гранью контура - задаем отверстие на нижней грани
                        case ContourSideName.Bottom:
                            distance = CalculateDistance(points[0].Point, points[1].Point);
                            offset = CalculateOffset(points[0].Point, points[1].Point, contourHalfH0.Bottom.GetEndPoint(0));
                            SetHoleOnEdge(parameters, "Ст2.ширина отверстия", "Ст2.смещение отверстия от Ст1", distance, offset);
                            break;
                        // если второе пересечение с правой гранью контура - задаем смещение правой снизу и нижней справа
                        case ContourSideName.Right:
                            distance = CalculateDistance(points[1].Point, contourHalfH0.Right.GetEndPoint(0));
                            SetOffsetFromEdge(parameters, "Ст3.отступ линии от Ст2", distance);
                            distance = CalculateDistance(points[0].Point, contourHalfH0.Bottom.GetEndPoint(1));
                            SetOffsetFromEdge(parameters, "Ст2.отступ линии от Ст3", distance);
                            break;
                    }
                    break;
                case ContourSideName.Right:
                    switch (points[1].SideName)
                    {
                        // если второе пересечение с верхней гранью контура
                        case ContourSideName.NoIntersection:
                            // если вторая точка ближе к левой стороне
                            if (points[1].Point.DistanceTo(contourHalfH0.Left.GetEndPoint(0)) <
                                points[1].Point.DistanceTo(contourHalfH0.Right.GetEndPoint(1)))
                            // задаем смещение правой стороны снизу, выключаем левую и нижнюю грань
                            {
                                distance = CalculateDistance(points[0].Point, contourHalfH0.Right.GetEndPoint(0));
                                SetOffsetFromEdge(parameters, "Ст3.отступ линии от Ст2", distance);
                                parameters.IntParameters["Вкл сторона 1"] = 0;
                                parameters.IntParameters["Вкл сторона 2"] = 0;
                            }
                            // если вторая точка ближе к правой стороне - задаем смещение правой грани сверху
                            else
                            {
                                distance = CalculateDistance(points[0].Point, contourHalfH0.Right.GetEndPoint(1));
                                SetOffsetFromEdge(parameters, "Ст3.отступ линии от Ст4", distance);
                            }
                            break;
                            // если второе пересечение с левой гранью
                            case ContourSideName.Left:

                            break;

                        // если второе пересечение с правой гранью контура - задаем отверстие на правой грани
                        case ContourSideName.Right:
                            distance = CalculateDistance(points[0].Point, points[1].Point);
                            offset = CalculateOffset(points[0].Point, points[1].Point, contourHalfH0.Right.GetEndPoint(1));
                            SetHoleOnEdge(parameters, "Ст3.ширина отверстия", "Ст3.смещение отверстия от Ст4", distance, offset);
                            break;
                    }
                    break;
            }
            return CSharpFunctionalExtensions.Result.Success();
        }

        private double CalculateDistance(XYZ pointOnEdge, XYZ pointEndEdge) =>
            pointOnEdge.DistanceTo(pointEndEdge);
        private double CalculateOffset(XYZ firstPointOnEdge, XYZ secondPointOnEdge, XYZ pointEndEdge)
        {
            var offset = firstPointOnEdge.DistanceTo(pointEndEdge) < secondPointOnEdge.DistanceTo(pointEndEdge) ?
                firstPointOnEdge.DistanceTo(pointEndEdge) :
                secondPointOnEdge.DistanceTo(pointEndEdge);
            return offset;
        }
        private void SetOffsetFromEdge(PressureContourParameters parameters, string parameterName, double distance)
        {
            if (parameters.DoubleParameters[parameterName] < distance)
                parameters.DoubleParameters[parameterName] = distance;
        }
        private void SetHoleOnEdge(PressureContourParameters parameters,
            string parameterNameHoleWidth,
            string parameterNameHoleOffset,
            double distance,
            double offset)
        {
            if (parameters.DoubleParameters[parameterNameHoleWidth] != 0 &&
                parameters.DoubleParameters[parameterNameHoleOffset] != 0)
            {
                var currentDistance = parameters.DoubleParameters[parameterNameHoleWidth];
                var currentOffset = parameters.DoubleParameters[parameterNameHoleOffset];
                parameters.DoubleParameters[parameterNameHoleWidth] =
                    Math.Max(offset + distance, currentOffset + currentDistance) - Math.Min(offset, currentOffset);
                parameters.DoubleParameters[parameterNameHoleOffset] = Math.Min(offset, currentOffset);
            }
            else
            {
                parameters.DoubleParameters[parameterNameHoleWidth] = distance;
                parameters.DoubleParameters[parameterNameHoleOffset] = offset;
            }
        }

    }
}

