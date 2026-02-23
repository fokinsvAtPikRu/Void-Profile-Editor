using Autodesk.Revit.DB;
using Void_Profile_Editor.Model;
using Void_Profile_Editor.Abstraction;
using System;

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

        public CSharpFunctionalExtensions.Result<IntersectionPoint[]> LineWithContourIntersection(Line[] lines, Contour contour)
        {
            IntersectionPoint[] results = new IntersectionPoint[2];
            IntersectionResultArray result;
            foreach (var contourLine in contour)
            {
                for (var i = 0; i < 2; i++)
                {
                    SetComparisonResult comparison = contourLine.Value.Intersect(lines[i], out result);
                    if (result != null)
                    {
                        if (result.Size != 1)
                        {
                            return CSharpFunctionalExtensions.Result.Failure<IntersectionPoint[]>("Точек пересечения больше одной");                            
                        }
                        results[i] = new IntersectionPoint(result.get_Item(0).XYZPoint, contourLine.Key);                        
                    }
                }
            }
            return CSharpFunctionalExtensions.Result.Success<IntersectionPoint[]>(results);
        }

        public CSharpFunctionalExtensions.Result CalculateParameters(Contour contourHalfH0, IntersectionPoint[] points, PressureContour pressureContour)
        {
            var parameters = pressureContour.ContourParameters;            

            parameters.IntParameters["Вкл редактирование контура"] = 1;
            bool firstPointIsFounded = false;
            foreach (var edge in contourHalfH0)
            {
                // флаг первая точка пересечения найдена - ложь и первая точка пересечения не на текущей грани - пропускаем шаг
                if (edge.Key != points[0].SideName && !firstPointIsFounded)
                    continue;
                // первая точка пересечения найдена
                if (edge.Key == points[0].SideName && !firstPointIsFounded)
                {
                    // устанавливаем флаг
                    firstPointIsFounded = true;
                    // проверяем вторую точку пеерсечения
                    if (edge.Key == points[1].SideName)
                    // если вторая точка на той же грани - задаем отверстие 
                    {
                        var distance = CalculateDistance(points[0].Point, points[1].Point);
                        var offset = CalculateOffset(points[0].Point, points[1].Point, edge.Value.GetEndPoint(0));
                        string parameterNameHoleOffset;
                        string parameterNameHoleWidth;
                        GetParameterNameForSetHole(edge.Key, out parameterNameHoleWidth, out parameterNameHoleOffset);
                        // если пересечение с верхней стороной, имена параметров - пустая строка - пропускаем шаг
                        if (String.IsNullOrEmpty(parameterNameHoleOffset) || String.IsNullOrEmpty(parameterNameHoleWidth))
                            continue;
                        SetHoleOnEdge(parameters, parameterNameHoleWidth, parameterNameHoleOffset, distance, offset);
                        continue;
                    }
                    // если вторая точка не на этой грани устанавливаем смещение последней точки грани
                    else
                    {
                        var distance = CalculateDistance(points[0].Point, edge.Value.GetEndPoint(1));
                        string parameterNameOffset = GetParameterNameForOffset(edge.Key, false);
                        SetOffsetFromEdge(parameters, parameterNameOffset, distance);
                        continue;
                    }
                }
                // первая точка пересечения уже найдена, вторая не на текущей стороне - выключаем текущую сторону
                if (firstPointIsFounded && edge.Key != points[1].SideName)
                {
                    string parameterNameSwitchOffEdge= GetParameterNameSwitchOffEdge(edge.Key);
                    parameters.IntParameters[parameterNameSwitchOffEdge] = 0;
                    continue;
                }
                // первая точка пересечения уже найдена, вторая точка на текущей стороне - задаем смещение от начала
                if (firstPointIsFounded && edge.Key == points[1].SideName)
                {
                    var distance = CalculateDistance(points[1].Point, edge.Value.GetEndPoint(0));
                    string parameterNameOffset = GetParameterNameForOffset(edge.Key, true);
                    SetOffsetFromEdge(parameters, parameterNameOffset, distance);
                    continue;
                }
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
        private void GetParameterNameForSetHole(ContourSideName edgeName,
            out string parameterNameHoleWidth,
            out string parameterNameHoleOffset)
        {
            parameterNameHoleOffset = String.Empty;
            parameterNameHoleWidth = String.Empty;
            switch (edgeName)
            {
                case ContourSideName.Left:
                    parameterNameHoleOffset = "Ст1.смещение отверстия от Ст4";
                    parameterNameHoleWidth = "Ст1.ширина отверстия";
                    break;
                case ContourSideName.Bottom:
                    parameterNameHoleOffset = "Ст2.смещение отверстия от Ст1";
                    parameterNameHoleWidth = "Ст2.ширина отверстия";
                    break;
                case ContourSideName.Right:
                    parameterNameHoleOffset = "Ст3.смещение отверстия от Ст4";
                    parameterNameHoleWidth = "Ст3.ширина отверстия";
                    break;
            }
        }

        private string GetParameterNameForOffset(ContourSideName edgeName, bool isStartPoint)
        {
            string parameterNameOffset = String.Empty;
            switch (edgeName)
            {
                case ContourSideName.Left:
                    parameterNameOffset = isStartPoint ? "Ст1.отступ линии от Ст4" : "Ст1.отступ линии от Ст2";

                    break;
                case ContourSideName.Bottom:
                    parameterNameOffset = isStartPoint ? "Ст2.отступ линии от Ст1" : "Ст2.отступ линии от Ст3";

                    break;
                case ContourSideName.Right:
                    parameterNameOffset = isStartPoint ? "Ст3.отступ линии от Ст2" : "Ст3.отступ линии от Ст4";

                    break;
            }
            return parameterNameOffset;
        }

        private string GetParameterNameSwitchOffEdge(ContourSideName edgeName)
        {
            string result = String.Empty;
            switch (edgeName)
            {
                case ContourSideName.Left:
                    result = "Вкл сторона 1";
                    break;
                case ContourSideName.Bottom:
                    result = "Вкл сторона 2";
                    break;
                case ContourSideName.Right:
                    result = "Вкл сторона 3";
                    break;
            }
            return result;
        }
    }
}

