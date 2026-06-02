using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Void_Profile_Editor.Domain.Model.Geometry;
using Void_Profile_Editor.Infrastructure.Adapters;

namespace Void_Profile_Editor.Domain.Services.Tests
{
    [TestFixture]
    public class GeometryServiceTests
    {
        private GeometryService _geometryService;

        [SetUp]
        public void SetUp()
        {
            _geometryService = new GeometryService();
        }

        #region RotatePointAroundAxis Tests

        [Test]
        public void RotatePointAroundAxis_Should_RotatePointCorrectly_When_AngleIs90DegreesAroundZAxis()
        {
            // Arrange
            var point = new Point3DDomain(1, 0, 0);
            var center = new Point3DDomain(0, 0, 0);
            var axis = new Point3DDomain(0, 0, 1);
            double angle = Math.PI / 2; // 90 degrees

            // Act
            var result = _geometryService.RotatePointAroundAxis(point, center, axis, angle);

            // Assert
            result.X.Should().BeApproximately(0, 1e-6);
            result.Y.Should().BeApproximately(1, 1e-6);
            result.Z.Should().BeApproximately(0, 1e-6);
        }

        [Test]
        public void RotatePointAroundAxis_Should_ReturnSamePoint_When_AngleIsZero()
        {
            // Arrange
            var point = new Point3DDomain(5, 3, 2);
            var center = new Point3DDomain(1, 1, 1);
            var axis = new Point3DDomain(0, 1, 0);
            double angle = 0;

            // Act
            var result = _geometryService.RotatePointAroundAxis(point, center, axis, angle);

            // Assert
            result.X.Should().Be(5);
            result.Y.Should().Be(3);
            result.Z.Should().Be(2);
        }

        [Test]
        public void RotatePointAroundAxis_Should_HandleNonZeroCenterCorrectly()
        {
            // Arrange
            var point = new Point3DDomain(3, 4, 0);
            var center = new Point3DDomain(1, 1, 0);
            var axis = new Point3DDomain(0, 0, 1);
            double angle = Math.PI; // 180 degrees

            // Act
            var result = _geometryService.RotatePointAroundAxis(point, center, axis, angle);

            // Assert
            result.X.Should().BeApproximately(-1, 1e-6);
            result.Y.Should().BeApproximately(-2, 1e-6);
            result.Z.Should().BeApproximately(0, 1e-6);
        }

        #endregion

        #region LineWithContourIntersection Tests

        [Test]
        public void LineWithContourIntersection_Should_ReturnFailure_When_LineHasMoreThanOneIntersection()
        {
            // Arrange
            var lines = new[]
            {
                CreateDetailLineDomain(0, 0, 10, 10),
                CreateDetailLineDomain(0, 0, 10, 10)
            };

            var contour = new Contour();
            contour.Left=CreateLineDomain(0, 0, 0, 10);
            contour.Bottom=CreateLineDomain(0, 0, 10, 0);
            contour.Right=CreateLineDomain(10, 0, 10, 10);
            contour.TopLeft=CreateLineDomain(0, 10, 5, 10);
            contour.TopRight=CreateLineDomain(5, 10, 10, 10);

            // Act
            var result = _geometryService.LineWithContourIntersection(lines, contour);

            // Assert
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Does.Contain("точек пересечения больше одной"));
        }

        [Test]
        public void LineWithContourIntersection_Should_ReturnSuccess_When_EachLineHasOneIntersection()
        {
            // Arrange
            var mockLine1 = new Mock<DetailLineDomain>();
            var mockLine2 = new Mock<DetailLineDomain>();

            var line1 = Line.CreateBound(new XYZ(5, -5, 0), new XYZ(5, 15, 0));
            var line2 = Line.CreateBound(new XYZ(-5, 5, 0), new XYZ(15, 5, 0));

            mockLine1.Setup(l => l.ToRevit()).Returns(line1);
            mockLine2.Setup(l => l.ToRevit()).Returns(line2);

            var lines = new[] { mockLine1.Object, mockLine2.Object };

            var contour = new Contour();
            var leftLine = CreateLineDomain(0, 0, 0, 10);
            var bottomLine = CreateLineDomain(0, 0, 10, 0);
            var rightLine = CreateLineDomain(10, 0, 10, 10);
            var topLeftLine = CreateLineDomain(0, 10, 5, 10);
            var topRightLine = CreateLineDomain(5, 10, 10, 10);

            contour.Left=leftLine;
            contour.Bottom = bottomLine;
            contour.Right=rightLine;
            contour.TopLeft= topLeftLine;
            contour.TopRight= topRightLine;

            // Act
            var result = _geometryService.LineWithContourIntersection(lines, contour);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.Length, Is.EqualTo(2));
        }

        #endregion

        #region CalculateParameters Tests

        [Test]
        public void CalculateParameters_Should_ReturnFailure_When_ContourHalfH0IsNull()
        {
            // Arrange
            var points = Array.Empty<IntersectionPoint>();
            var pressureContour = new PressureContour();

            // Act
            var result = _geometryService.CalculateParameters(null, points, pressureContour);

            // Assert
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("contourHalfH0 == null"));
        }

        [Test]
        public void CalculateParameters_Should_ReturnFailure_When_PointsIsNull()
        {
            // Arrange
            var contourHalfH0 = new Contour();
            var pressureContour = new PressureContour();

            // Act
            var result = _geometryService.CalculateParameters(contourHalfH0, null, pressureContour);

            // Assert
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("Точки пересечения с контуром null"));
        }

        [Test]
        public void CalculateParameters_Should_ReturnFailure_When_PressureContourIsNull()
        {
            // Arrange
            var contourHalfH0 = new Contour();
            var points = Array.Empty<IntersectionPoint>();

            // Act
            var result = _geometryService.CalculateParameters(contourHalfH0, points, null);

            // Assert
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("pressureContour == null"));
        }

        [Test]
        public void CalculateParameters_Should_ReturnFailure_When_ContourParametersIsNull()
        {
            // Arrange
            var contourHalfH0 = new Contour();
            var points = Array.Empty<IntersectionPoint>();
            var pressureContour = new PressureContour { ContourParameters = null };

            // Act
            var result = _geometryService.CalculateParameters(contourHalfH0, points, pressureContour);

            // Assert
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("pressureContour.ContourParameters == null"));
        }

        [Test]
        public void CalculateParameters_Should_SetEditContourParameter_When_ValidInput()
        {
            // Arrange
            var contourHalfH0 = CreateTestContour();

            var points = new[]
            {
                new IntersectionPoint(new Point3DDomain(0, 5, 0), ContourSideName.Left),
                new IntersectionPoint(new Point3DDomain(10, 5, 0), ContourSideName.Right)
            };

            var parameters = new PressureContourParameters();
            var pressureContour = new PressureContour { ContourParameters = parameters };

            // Act
            var result = _geometryService.CalculateParameters(contourHalfH0, points, pressureContour);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(parameters.IntParameters["Вкл редактирование контура"], Is.EqualTo(1));
        }

        [Test]
        public void CalculateParameters_Should_SetHoleOnEdge_When_BothPointsOnSameSide()
        {
            // Arrange
            var contourHalfH0 = new Contour();
            var bottomLineMock = CreateLineDomain(0, 0, 20, 0);
            contourHalfH0.Bottom= bottomLineMock;

            var points = new[]
            {
                new IntersectionPoint(new Point3DDomain(5, 0, 0), ContourSideName.Bottom),
                new IntersectionPoint(new Point3DDomain(15, 0, 0), ContourSideName.Bottom)
            };

            var parameters = new PressureContourParameters();
            var pressureContour = new PressureContour { ContourParameters = parameters };

            // Act
            var result = _geometryService.CalculateParameters(contourHalfH0, points, pressureContour);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(parameters.DoubleParameters["Ст2.ширина отверстия"], Is.EqualTo(10));
            Assert.That(parameters.DoubleParameters["Ст2.смещение отверстия от Ст1"], Is.EqualTo(5));
        }

        [Test]
        public void CalculateParameters_Should_SetHoleOnEdge_When_HoleAlreadyExistsAndNewHoleIsLarger()
        {
            // Arrange
            var contourHalfH0 = new Contour();
            var bottomLineMock = CreateLineDomain(0, 0, 20, 0);
            contourHalfH0.Bottom=bottomLineMock;

            var points = new[]
            {
                new IntersectionPoint(new Point3DDomain(5, 0, 0), ContourSideName.Bottom),
                new IntersectionPoint(new Point3DDomain(15, 0, 0), ContourSideName.Bottom)
            };

            var parameters = new PressureContourParameters();
            parameters.DoubleParameters["Ст2.ширина отверстия"] = 3;
            parameters.DoubleParameters["Ст2.смещение отверстия от Ст1"] = 6;

            var pressureContour = new PressureContour { ContourParameters = parameters };

            // Act
            var result = _geometryService.CalculateParameters(contourHalfH0, points, pressureContour);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            // Ожидаем объединение отверстий: от 5 до 15 (ширина 10, смещение 5)
            Assert.That(parameters.DoubleParameters["Ст2.ширина отверстия"], Is.EqualTo(10));
            Assert.That(parameters.DoubleParameters["Ст2.смещение отверстия от Ст1"], Is.EqualTo(5));
        }

        [Test]
        public void CalculateParameters_Should_SetOffsetAndDisableEdges_When_PointsOnDifferentSides()
        {
            // Arrange
            var contourHalfH0 = CreateTestContour();

            var points = new[]
            {
                new IntersectionPoint(new Point3DDomain(0, 5, 0), ContourSideName.Left),
                new IntersectionPoint(new Point3DDomain(20, 15, 0), ContourSideName.Right)
            };

            var parameters = new PressureContourParameters();
            var pressureContour = new PressureContour { ContourParameters = parameters };

            // Act
            var result = _geometryService.CalculateParameters(contourHalfH0, points, pressureContour);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(parameters.DoubleParameters.ContainsKey("Ст1.отступ линии от Ст4"), Is.True);
            Assert.That(parameters.DoubleParameters.ContainsKey("Ст3.отступ линии от Ст4"), Is.True);
            Assert.That(parameters.IntParameters["Вкл сторона 2"], Is.EqualTo(0));
        }

        [Test]
        public void CalculateParameters_Should_HandlePointsOnSameSide_When_FirstPointFoundAndSecondOnDifferentSide()
        {
            // Arrange
            var contourHalfH0 = CreateTestContour();

            var points = new[]
            {
                new IntersectionPoint(new Point3DDomain(0, 5, 0), ContourSideName.Left),
                new IntersectionPoint(new Point3DDomain(5, 0, 0), ContourSideName.Bottom)
            };

            var parameters = new PressureContourParameters();
            var pressureContour = new PressureContour { ContourParameters = parameters };

            // Act
            var result = _geometryService.CalculateParameters(contourHalfH0, points, pressureContour);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(parameters.DoubleParameters.ContainsKey("Ст1.отступ линии от Ст4"), Is.True);
            Assert.That(parameters.DoubleParameters.ContainsKey("Ст2.отступ линии от Ст1"), Is.True);
        }

        #endregion

        #region Edge Cases Tests

        [Test]
        public void CalculateParameters_Should_SkipProcessing_When_ParameterNamesAreEmpty()
        {
            // Arrange
            var contourHalfH0 = new Contour();
            var topLineMock = CreateLineDomain(0, 10, 10, 10);
            contourHalfH0.TopLeft= topLineMock;

            var points = new[]
            {
                new IntersectionPoint(new Point3DDomain(2, 10, 0), ContourSideName.TopLeft),
                new IntersectionPoint(new Point3DDomain(8, 10, 0), ContourSideName.TopLeft)
            };

            var parameters = new PressureContourParameters();
            var pressureContour = new PressureContour { ContourParameters = parameters };

            // Act
            var result = _geometryService.CalculateParameters(contourHalfH0, points, pressureContour);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            // Для верхней стороны имена параметров пустые, ничего не должно быть установлено
            Assert.That(parameters.DoubleParameters.Count, Is.EqualTo(0));
        }

        [Test]
        public void CalculateParameters_Should_UpdateExistingParameters_When_TheyAlreadyExist()
        {
            // Arrange
            var contourHalfH0 = new Contour();
            var bottomLineMock = CreateLineDomain(0, 0, 20, 0);
            contourHalfH0.Bottom= bottomLineMock;

            var points = new[]
            {
                new IntersectionPoint(new Point3DDomain(5, 0, 0), ContourSideName.Bottom),
                new IntersectionPoint(new Point3DDomain(15, 0, 0), ContourSideName.Bottom)
            };

            var parameters = new PressureContourParameters();
            parameters.DoubleParameters["Ст2.ширина отверстия"] = 0;
            parameters.DoubleParameters["Ст2.смещение отверстия от Ст1"] = 0;

            var pressureContour = new PressureContour { ContourParameters = parameters };

            // Act
            var result = _geometryService.CalculateParameters(contourHalfH0, points, pressureContour);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(parameters.DoubleParameters["Ст2.ширина отверстия"], Is.EqualTo(10));
            Assert.That(parameters.DoubleParameters["Ст2.смещение отверстия от Ст1"], Is.EqualTo(5));
        }

        #endregion

        #region Helper Methods

        private DetailLineDomain CreateDetailLineDomain(double x1, double y1, double x2, double y2)
        {
            var mock = new Mock<DetailLineDomain>();
            var line = Line.CreateBound(new XYZ(x1, y1, 0), new XYZ(x2, y2, 0));
            mock.Setup(d => d.ToRevit()).Returns(line);
            return mock.Object;
        }

        private DetailLineDomain CreateLineDomain(double x1, double y1, double x2, double y2)
        {
            var mock = new Mock<DetailLineDomain>();
            var line = Line.CreateBound(new XYZ(x1, y1, 0), new XYZ(x2, y2, 0));
            var startPoint = new Point3DDomain(x1, y1, 0);
            var endPoint = new Point3DDomain(x2, y2, 0);

            mock.Setup(l => l.ToRevit()).Returns(line);
            mock.Setup(l => l.Start).Returns(startPoint);
            mock.Setup(l => l.End).Returns(endPoint);            

            return mock.Object;
        }

        private Contour CreateTestContour()
        {
            var contour = new Contour();
            contour.Left= CreateLineDomain(0, 0, 0, 20);
            contour.Bottom= CreateLineDomain(0, 0, 20, 0);
            contour.Right= CreateLineDomain(20, 0, 20, 20);
            contour.TopLeft =  CreateLineDomain(0, 20, 10, 20);
            contour.TopRight = CreateLineDomain(10, 20, 20, 20);
            return contour;
        }

        #endregion
    }
}