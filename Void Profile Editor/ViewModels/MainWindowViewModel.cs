using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging.Messages;
using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Void_Profile_Editor.Abstraction;
using Void_Profile_Editor.Model;
using Void_Profile_Editor.Services;

namespace Void_Profile_Editor.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        #region Fields
        // RevitTask
        private RevitTask _revitTask;

        // Services
        private readonly ISelectionService _selectionService;
        private readonly IPressureCounturInformationService _pressureCounturInformationService;
        private readonly ICreateContourService _createContourService;
        private readonly IDrawLineService _drawLineService;
        private readonly IGeometryService _geometryService;

        // Commands
        private readonly AsyncRelayCommand _selectFamilyInstanceCommand;
        private readonly AsyncRelayCommand _createContourCommand;
        private readonly AsyncRelayCommand _createCutingLinesCommand;


        // Fields
        private Document _document;
        private FamilyInstance _instance;
        private PressureContour _pressureContour;
        private Contour _contour6H0;
        private Contour _contourHalfH0;
        private Line[] _cuttingLines;
        #endregion
        #region ctor
        public MainWindowViewModel(
            RevitTask revitTask,
            Document document,
            ISelectionService selection,
            IPressureCounturInformationService pressureCounturInformationService,
            ICreateContourService createContourService,
            IDrawLineService drawLineService,
            IGeometryService geometryService)
        {
            // Fields
            _revitTask = revitTask;
            _document = document;
            // Services
            _selectionService = selection;
            _pressureCounturInformationService = pressureCounturInformationService;
            _createContourService = createContourService;
            _drawLineService = drawLineService;
            _geometryService = geometryService;
            // Commands
            _selectFamilyInstanceCommand = new AsyncRelayCommand(AsyncSelectSelectFamilyInstance);
            _createContourCommand = new AsyncRelayCommand(CreateContour);
        }
        #endregion
        #region Properties for Command
        // Properties for Commands
        public AsyncRelayCommand SelectFamilyInstanceCommand
        {
            get => _selectFamilyInstanceCommand;
        }
        public AsyncRelayCommand CreateContourCommand
        {
            get => _createContourCommand;
        }
        public AsyncRelayCommand CreateCutingLinesCommand
        {
            get => _createCutingLinesCommand;
        }
        #endregion
        #region Method Execute for SelectFamilyInstanceCommand
        // Method Execute for SelectFamilyInstanceCommand
        private async Task AsyncSelectSelectFamilyInstance()
        {
            await _revitTask.Run(app => SelectFamilyInstance());
        }
        private void SelectFamilyInstance()
        {
            _selectionService.PickObject().
            Bind((instance) => { _instance = instance; return CSharpFunctionalExtensions.Result.Success<FamilyInstance>(instance); }).
            Bind(_pressureCounturInformationService.CreatePressureContourInfo).
            Bind((pressureContour) => { _pressureContour = pressureContour; return CSharpFunctionalExtensions.Result.Success(); });
        }

        #endregion
        #region Method Execute for CreateContourCommand
        // Method Execute for CreateContourCommand
        private async Task AsyncCreateContour()
        {
            await _revitTask.Run(app => CreateContour());
        }
        private void CreateContour()
        {
            Create6H0Contour().
            Bind(DrawContour).
            Bind(() => CreateHalfH0Contour());
        }
        private CSharpFunctionalExtensions.Result<Contour> Create6H0Contour()
        {
            _contour6H0 = _createContourService.Create(
                _pressureContour.InsertPoint,
                _pressureContour.Rotation,
                _pressureContour.H0,
                _pressureContour.WallThickness,
                6 * _pressureContour.H0,
                _instance.Mirrored).Value;
            return CSharpFunctionalExtensions.Result.Success<Contour>(_contour6H0);
        }
        private CSharpFunctionalExtensions.Result<Contour> CreateHalfH0Contour()
        {
            _contourHalfH0 = _createContourService.Create(
               _pressureContour.InsertPoint,
               _pressureContour.Rotation,
               _pressureContour.H0,
               _pressureContour.WallThickness,
               0.5 * _pressureContour.H0,
               _instance.Mirrored).Value;
            return CSharpFunctionalExtensions.Result.Success<Contour>(_contourHalfH0);
        }
        private CSharpFunctionalExtensions.Result DrawContour(Contour contour)
        {
            try
            {
                using (Transaction tr = new Transaction(_document, "Контур 6h0"))
                {
                    tr.Start();
                    foreach (var line in contour)
                        _drawLineService.DrawLine(line.Value, tr);
                    tr.Commit();
                }
                return CSharpFunctionalExtensions.Result.Success();
            }
            catch (Exception ex)
            {
                return CSharpFunctionalExtensions.Result.Failure(ex.Message);
            }
        }
        #endregion
        #region Execute for CreateCuttingLinesCommand
        private async Task AsyncCreateCuttingLines()
        {
            await _revitTask.Run(app => CreateCuttingLines());
        }
        private void CreateCuttingLines()
        {
            _selectionService.PickPoint().
                Bind((point) =>
                {
                    return CSharpFunctionalExtensions.Result.Success<XYZ>(new XYZ(point.X, point.Y, 0));
                }).
                Bind((point) =>
                {
                    if (_cuttingLines == null)
                        _cuttingLines = new Line[2];
                    _cuttingLines[0] = Line.CreateBound(point, _pressureContour.Center);
                    return CSharpFunctionalExtensions.Result.Success();
                }).
                Bind(() => _selectionService.PickPoint()).
                Bind((point) =>
                {
                    return CSharpFunctionalExtensions.Result.Success<XYZ>(new XYZ(point.X, point.Y, 0));
                }).
                Bind((point) =>
                {
                    _cuttingLines[1] = Line.CreateBound(point, _pressureContour.Center);
                    return CSharpFunctionalExtensions.Result.Success();
                }).
        }
        private void FindIntersection()
        {
            XYZ[] points=new XYZ[2];            
            ContourSideName[] sideName=new ContourSideName[2];

            for (var i = 0; i < 2; i++)
            {
                var result = _geometryService.LineWithContourIntersection(_cuttingLines[i], _contourHalfH0, out sideName[i]);
                if (result.IsSuccess)
                    points[i] = result.Value;
            }
            if (sideName[0] != null && sideName[0] == sideName[1])
            {
                var contourLine = _contourHalfH0.GetLine(sideName[0]);

            }
        }
        #endregion
    }
}
