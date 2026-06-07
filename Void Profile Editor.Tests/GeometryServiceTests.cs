//using Autodesk.Revit.DB;
//using CSharpFunctionalExtensions;
//using Moq;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Void_Profile_Editor.Domain.Abstraction.Services;
//using Void_Profile_Editor.Domain.Model.Geometry;
//using Void_Profile_Editor.Infrastructure.Adapters;

//namespace Void_Profile_Editor.Domain.Services.Tests
//{
//    [TestFixture]
//    public class GeometryServiceTests
//    {
//        private GeometryService _geometryService;
//        private const double EPSILON = 0.0001;

//        [SetUp]
//        public void SetUp()
//        {
//            _geometryService = new GeometryService();
//        }

//        #region RotatePointAroundAxis Tests

//        [Test]
//        public void RotatePointAroundAxis_ZeroAngle_ReturnsSamePoint()
//        {
//            // Arrange
//            var point = new Point3DDomain(10, 5, 0);
//            var center = new Point3DDomain(0, 0, 0);
//            var axis = new Point3DDomain(0, 0, 1);
//            double angle = 0;

//            // Act
//            var result = _geometryService.RotatePointAroundAxis(point, center, axis, angle);

//            // Assert
//            Assert.AreEqual(point.X, result.X, EPSILON);
//            Assert.AreEqual(point.Y, result.Y, EPSILON);
//            Assert.AreEqual(point.Z, result.Z, EPSILON);
//        }

//        [Test]
//        public void RotatePointAroundAxis_90DegreesAroundZ_ReturnsCorrectPoint()
//        {
//            // Arrange
//            var point = new Point3DDomain(10, 0, 0);
//            var center = new Point3DDomain(0, 0, 0);
//            var axis = new Point3DDomain(0, 0, 1);
//            double angle = Math.PI / 2; // 90 градусов

//            // Act
//            var result = _geometryService.RotatePointAroundAxis(point, center, axis, angle);

//            // Assert
//            Assert.AreEqual(0, result.X, EPSILON);
//            Assert.AreEqual(10, result.Y, EPSILON);
//            Assert.AreEqual(0, result.Z, EPSILON);
//        }

//        [Test]
//        public void RotatePointAroundAxis_180DegreesAroundZ_ReturnsOppositePoint()
//        {
//            // Arrange
//            var point = new Point3DDomain(10, 5, 0);
//            var center = new Point3DDomain(0, 0, 0);
//            var axis = new Point3DDomain(0, 0, 1);
//            double angle = Math.PI;

//            // Act
//            var result = _geometryService.RotatePointAroundAxis(point, center, axis, angle);

//            // Assert
//            Assert.AreEqual(-10, result.X, EPSILON);
//            Assert.AreEqual(-5, result.Y, EPSILON);
//            Assert.AreEqual(0, result.Z, EPSILON);
//        }

//        [Test]
//        public void RotatePointAroundAxis_WithNonZeroCenter_ReturnsCorrectPoint()
//        {
//            // Arrange
//            var point = new Point3DDomain(15, 5, 0);
//            var center = new Point3DDomain(10, 5, 0);
//            var axis = new Point3DDomain(0, 0, 1);
//            double angle = Math.PI / 2;

//            // Act
//            var result = _geometryService.RotatePointAroundAxis(point, center, axis, angle);

//            // Assert
//            Assert.AreEqual(10, result.X, EPSILON);
//            Assert.AreEqual(10, result.Y, EPSILON);
//            Assert.AreEqual(0, result.Z, EPSILON);
//        }

//        [Test]
//        public void RotatePointAroundAxis_RotationAroundXAxis_ReturnsCorrectPoint()
//        {
//            // Arrange
//            var point = new Point3DDomain(0, 10, 0);
//            var center = new Point3DDomain(0, 0, 0);
//            var axis = new Point3DDomain(1, 0, 0);
//            double angle = Math.PI / 2;

//            // Act
//            var result = _geometryService.RotatePointAroundAxis(point, center, axis, angle);

//            // Assert
//            Assert.AreEqual(0, result.X, EPSILON);
//            Assert.AreEqual(0, result.Y, EPSILON);
//            Assert.AreEqual(10, result.Z, EPSILON);
//        }

//        #endregion

//        #region LineWithContourIntersection Tests

//        [Test]
//        public void LineWithContourIntersection_TwoLinesIntersectContour_ReturnsIntersectionPoints()
//        {
//            // Arrange
//            var contour = CreateTestContour();
//            var lines = CreateTwoTestLines();

//            // Act
//            var result = _geometryService.LineWithContourIntersection(lines, contour);

//            // Assert
//            Assert.IsTrue(result.IsSuccess);
//            Assert.IsNotNull(result.Value);
//            Assert.AreEqual(2, result.Value.Length);
//        }

//        [Test]
//        public void LineWithContourIntersection_LinesDoNotIntersect_ReturnsNullIntersections()
//        {
//            // Arrange
//            var contour = CreateTestContour();
//            var lines = new DetailLineDomain[]
//            {
//                CreateDetailLine(new Point3DDomain(100, 100, 0), new Point3DDomain(200, 100, 0)),
//                CreateDetailLine(new Point3DDomain(100, 200, 0), new Point3DDomain(200, 200, 0))
//            };

//            // Act
//            var result = _geometryService.LineWithContourIntersection(lines, contour);

//            // Assert
//            Assert.IsTrue(result.IsSuccess);
//            Assert.IsNull(result.Value);
//        }

//        [Test]
//        public void LineWithContourIntersection_WithNullLines_ShouldHandleGracefully()
//        {
//            // Arrange
//            var contour = CreateTestContour();
//            DetailLineDomain[] lines = null;

//            // Act & Assert
//            Assert.Throws<NullReferenceException>(() =>
//                _geometryService.LineWithContourIntersection(lines, contour));
//        }

//        #endregion

//        #region CalculateParameters Tests

//        [Test]
//        public void CalculateParameters_WithValidInput_ReturnsSuccess()
//        {
//            // Arrange
//            var contourHalfH0 = CreateTestContour();
//            var points = CreateTwoIntersectionPoints();
//            var pressureContour = CreateTestPressureContour();

//            // Act
//            var result = _geometryService.CalculateParameters(contourHalfH0, points, pressureContour);

//            // Assert
//            Assert.IsTrue(result.IsSuccess);
//        }

//        [Test]
//        public void CalculateParameters_WithNullContour_ReturnsFailure()
//        {
//            // Arrange
//            var points = CreateTwoIntersectionPoints();
//            var pressureContour = CreateTestPressureContour();

//            // Act
//            var result = _geometryService.CalculateParameters(null, points, pressureContour);

//            // Assert
//            Assert.IsTrue(result.IsFailure);
//            Assert.AreEqual("contourHalfH0 == null", result.Error);
//        }

//        [Test]
//        public void CalculateParameters_WithNullPoints_ReturnsFailure()
//        {
//            // Arrange
//            var contourHalfH0 = CreateTestContour();
//            var pressureContour = CreateTestPressureContour();

//            // Act
//            var result = _geometryService.CalculateParameters(contourHalfH0, null, pressureContour);

//            // Assert
//            Assert.IsTrue(result.IsFailure);
//            Assert.AreEqual("Точки пересечения с контуром null", result.Error);
//        }

//        [Test]
//        public void CalculateParameters_WithNullPressureContour_ReturnsFailure()
//        {
//            // Arrange
//            var contourHalfH0 = CreateTestContour();
//            var points = CreateTwoIntersectionPoints();

//            // Act
//            var result = _geometryService.CalculateParameters(contourHalfH0, points, null);

//            // Assert
//            Assert.IsTrue(result.IsFailure);
//            Assert.AreEqual("pressureContour == null", result.Error);
//        }

//        [Test]
//        public void CalculateParameters_WithNullContourParameters_ReturnsFailure()
//        {
//            // Arrange
//            var contourHalfH0 = CreateTestContour();
//            var points = CreateTwoIntersectionPoints();
//            var pressureContour = CreateTestPressureContour();
//            pressureContour.ContourParameters = null;

//            // Act
//            var result = _geometryService.CalculateParameters(contourHalfH0, points, pressureContour);

//            // Assert
//            Assert.IsTrue(result.IsFailure);
//            Assert.AreEqual("pressureContour.ContourParameters == null", result.Error);
//        }

//        [Test]
//        public void CalculateParameters_PointsOnSameEdge_SetsHoleParameters()
//        {
//            // Arrange
//            var contourHalfH0 = CreateContourWithEdges();
//            var points = new IntersectionPoint[]
//            {
//                new IntersectionPoint(new Point3DDomain(2, 5, 0), ContourSideName.Bottom),
//                new IntersectionPoint(new Point3DDomain(8, 5, 0), ContourSideName.Bottom)
//            };
//            var pressureContour = CreateTestPressureContour();
//            pressureContour.ContourParameters.DoubleParameters["Ст2.ширина отверстия"] = 0;
//            pressureContour.ContourParameters.DoubleParameters["Ст2.смещение отверстия от Ст1"] = 0;

//            // Act
//            var result = _geometryService.CalculateParameters(contourHalfH0, points, pressureContour);

//            // Assert
//            Assert.IsTrue(result.IsSuccess);
//            Assert.AreNotEqual(0, pressureContour.ContourParameters.DoubleParameters["Ст2.ширина отверстия"]);
//        }

//        [Test]
//        public void CalculateParameters_FirstPointOnLeftSecondOnBottom_SetsOffsetAndDisablesEdge()
//        {
//            // Arrange
//            var contourHalfH0 = CreateContourWithEdges();
//            var points = new IntersectionPoint[]
//            {
//                new IntersectionPoint(new Point3DDomain(0, 2, 0), ContourSideName.Left),
//                new IntersectionPoint(new Point3DDomain(3, 0, 0), ContourSideName.Bottom)
//            };
//            var pressureContour = CreateTestPressureContour();

//            // Act
//            var result = _geometryService.CalculateParameters(contourHalfH0, points, pressureContour);

//            // Assert
//            Assert.IsTrue(result.IsSuccess);
//            Assert.AreEqual(0, pressureContour.ContourParameters.IntParameters["Вкл сторона 1"]);
//        }

//        [Test]
//        public void CalculateParameters_WithRightEdge_HandlesCorrectIndexing()
//        {
//            // Arrange
//            var contourHalfH0 = CreateContourWithRightEdge();
//            var points = new IntersectionPoint[]
//            {
//                new IntersectionPoint(new Point3DDomain(10, 2, 0), ContourSideName.Right),
//                new IntersectionPoint(new Point3DDomain(10, 8, 0), ContourSideName.Right)
//            };
//            var pressureContour = CreateTestPressureContour();

//            // Act
//            var result = _geometryService.CalculateParameters(contourHalfH0, points, pressureContour);

//            // Assert
//            Assert.IsTrue(result.IsSuccess);
//        }

//        #endregion

//        #region Helper Methods

//        private Contour CreateTestContour()
//        {
//            var contour = new Contour();
//            contour.Left= new DetailLineDomain(
//                new Point3DDomain(0, 0, 0),
//                new Point3DDomain(0, 10, 0));
//            contour.Bottom= new DetailLineDomain(
//                new Point3DDomain(0, 0, 0),
//                new Point3DDomain(10, 0, 0));
//            contour.Right= new DetailLineDomain(
//                new Point3DDomain(10, 0, 0),
//                new Point3DDomain(10, 10, 0));
//            contour.TopLeft= new DetailLineDomain(
//                new Point3DDomain(0, 10, 0),
//                new Point3DDomain(5, 10, 0));
//            contour.TopRight = new DetailLineDomain(
//                new Point3DDomain(5, 10, 0),
//                new Point3DDomain(10, 10, 0));
//            return contour;
//        }

//        private Contour CreateContourWithEdges()
//        {
//            var contour = new Contour();
//            contour.Left= new DetailLineDomain(
//                new Point3DDomain(0, 0, 0),
//                new Point3DDomain(0, 10, 0));
//            contour.Bottom= new DetailLineDomain(
//                new Point3DDomain(0, 0, 0),
//                new Point3DDomain(10, 0, 0));
//            contour.Right= new DetailLineDomain(
//                new Point3DDomain(10, 0, 0),
//                new Point3DDomain(10, 10, 0));
//            return contour;
//        }

//        private Contour CreateContourWithRightEdge()
//        {
//            var contour = new Contour();
//            contour.Right= new DetailLineDomain(
//                new Point3DDomain(10, 0, 0),
//                new Point3DDomain(10, 10, 0));
//            return contour;
//        }

//        private DetailLineDomain[] CreateTwoTestLines()
//        {
//            return new DetailLineDomain[]
//            {
//                CreateDetailLine(new Point3DDomain(-5, 5, 0), new Point3DDomain(5, 5, 0)),
//                CreateDetailLine(new Point3DDomain(5, -5, 0), new Point3DDomain(5, 5, 0))
//            };
//        }

//        private DetailLineDomain CreateDetailLine(Point3DDomain start, Point3DDomain end)
//        {
//            var mockLine = new Mock<DetailLineDomain>(start, end);
//            return mockLine.Object;
//        }

//        private IntersectionPoint[] CreateTwoIntersectionPoints()
//        {
//            return new IntersectionPoint[]
//            {
//                new IntersectionPoint(new Point3DDomain(0, 5, 0), ContourSideName.Left),
//                new IntersectionPoint(new Point3DDomain(5, 0, 0), ContourSideName.Bottom)
//            };
//        }

//        private PressureContour CreateTestPressureContour()
//        {
//            var parameters = new PressureContourParameters
//            {
//                IntParameters = new Dictionary<string, int>(),
//                DoubleParameters = new Dictionary<string, double>()
//            };

//            // Инициализация необходимых параметров
//            parameters.IntParameters["Вкл редактирование контура"] = 0;
//            parameters.IntParameters["Вкл сторона 1"] = 1;
//            parameters.IntParameters["Вкл сторона 2"] = 1;
//            parameters.IntParameters["Вкл сторона 3"] = 1;

//            parameters.DoubleParameters["Ст1.отступ линии от Ст4"] = 0;
//            parameters.DoubleParameters["Ст1.отступ линии от Ст2"] = 0;
//            parameters.DoubleParameters["Ст2.отступ линии от Ст1"] = 0;
//            parameters.DoubleParameters["Ст2.отступ линии от Ст3"] = 0;
//            parameters.DoubleParameters["Ст3.отступ линии от Ст2"] = 0;
//            parameters.DoubleParameters["Ст3.отступ линии от Ст4"] = 0;
//            parameters.DoubleParameters["Ст1.ширина отверстия"] = 0;
//            parameters.DoubleParameters["Ст1.смещение отверстия от Ст4"] = 0;
//            parameters.DoubleParameters["Ст2.ширина отверстия"] = 0;
//            parameters.DoubleParameters["Ст2.смещение отверстия от Ст1"] = 0;
//            parameters.DoubleParameters["Ст3.ширина отверстия"] = 0;
//            parameters.DoubleParameters["Ст3.смещение отверстия от Ст4"] = 0;

//            return new PressureContour { ContourParameters = parameters };
//        }

//        #endregion
//    }
//}