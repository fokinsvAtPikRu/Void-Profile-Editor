using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
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
        private IntersectionPoint[] _intersectionPoints;

        // Observable Properties
        public FamilyInstance Instance
        {
            get => _instance;
            set
            {
                if (SetProperty(ref _instance, value))
                {
                    _createContourCommand.NotifyCanExecuteChanged();
                     OnPropertyChanged(nameof(CanCreateContourCommandExecute));                    
                }
            }
        }





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
            _createContourCommand = new AsyncRelayCommand(AsyncCreateContour/*, CanCreateContourCommandExecute*/);
            _createCutingLinesCommand = new AsyncRelayCommand(AsyncCreateCuttingLines/*, CanExecuteCreateCuttingLines*/);

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
            var result = _selectionService.PickObject().
                        Tap(instance => _instance = instance).
                        Bind(i => _pressureCounturInformationService.CreatePressureContourInfo(i)).
                        Tap(pc => _pressureContour = pc);
            if (result.IsFailure)
                TaskDialog.Show("Test", $"Error:{result.Error}");
            
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
            if (_instance == null)
            {
                TaskDialog.Show("Ошибка", "Семейство не выбрано");
                return;
            }
            if (_pressureContour == null)
            {
                TaskDialog.Show("Ошибка", "Контур не создан");
                return;
            }
            var result = Create6H0Contour().
                        Bind(c => DrawContour(c)).
                        Bind(() => CreateHalfH0Contour());
            if (result.IsFailure)
                TaskDialog.Show("Test", $"Error:{result.Error}");            
        }
        private CSharpFunctionalExtensions.Result<Contour> Create6H0Contour()
        {
            _contour6H0 = _createContourService.Create(
                _pressureContour.InsertPoint,
                _pressureContour.Rotation,
                _pressureContour.ContourParameters.DoubleParameters["h0"],
                _pressureContour.ContourParameters.DoubleParameters["Толщина"],
                6 * _pressureContour.ContourParameters.DoubleParameters["h0"],
                _instance.Mirrored).Value;
            return CSharpFunctionalExtensions.Result.Success<Contour>(_contour6H0);
        }
        private CSharpFunctionalExtensions.Result<Contour> CreateHalfH0Contour()
        {
            _contourHalfH0 = _createContourService.Create(
               _pressureContour.InsertPoint,
               _pressureContour.Rotation,
               _pressureContour.ContourParameters.DoubleParameters["h0"],
               _pressureContour.ContourParameters.DoubleParameters["Толщина"],
               0.5 * _pressureContour.ContourParameters.DoubleParameters["h0"],
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
                    {
                        if (line.Key == ContourSideName.TopLeft || line.Key==ContourSideName.TopRight)
                            continue;
                        _drawLineService.DrawLine(line.Value, tr);
                    }
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
        #region CanExecute for CreateContourCommand
        private bool CanCreateContourCommandExecute() => Instance!=null;
        #endregion
        #region Execute for CreateCuttingLinesCommand
        private async Task AsyncCreateCuttingLines()
        {
            await _revitTask.Run(app => CreateCuttingLines());
        }
        private bool CanExecuteCreateCuttingLines() =>
            _instance!=null && _pressureContour!=null && _contourHalfH0!=null;
        private void CreateCuttingLines()
        {
            try
            {
                // указываем первую точку для создания секущей линии 
                _selectionService.PickPoint()
                    // обнуляем координату Z у точки 
                    .Bind((point) =>
                    {
                        return CSharpFunctionalExtensions.Result.Success<XYZ>(new XYZ(point.X, point.Y, 0));
                    })
                    // строим первую секущую линию
                    .Bind((point) =>
                    {
                        if (_contourHalfH0 == null)
                            return CSharpFunctionalExtensions.Result.Failure("Контур 0,5H0 не создан");
                        _cuttingLines = new Line[2];
                        _cuttingLines[0] = Line.CreateBound(point, _contourHalfH0.Center);
                        return CSharpFunctionalExtensions.Result.Success();
                    })
                    // test method
                    .Bind(() =>
                    {
                        using (Transaction tr = new Transaction(_document, "Draw Cutting Line"))
                        {
                            tr.Start();
                            if (_cuttingLines == null)
                                tr.RollBack();
                            else
                                _drawLineService.DrawLine(_cuttingLines[0], tr);
                            tr.Commit();
                        }
                        return CSharpFunctionalExtensions.Result.Success();
                    })
                    // повторяем, строим вторую секущую линию
                    .Bind(() => _selectionService.PickPoint())
                    .Bind((point) =>
                    {
                        return CSharpFunctionalExtensions.Result.Success<XYZ>(new XYZ(point.X, point.Y, 0));
                    })
                    .Bind((point) =>
                    {
                        _cuttingLines[1] = Line.CreateBound(point, _contourHalfH0.Center);
                        return CSharpFunctionalExtensions.Result.Success();
                    })
                    // test method
                    .Bind(() =>
                    {
                        using (Transaction tr = new Transaction(_document, "Draw Cutting Line"))
                        {
                            tr.Start();
                            if (_cuttingLines == null)
                                tr.RollBack();
                            else
                                _drawLineService.DrawLine(_cuttingLines[1], tr);
                            tr.Commit();
                        }
                        return CSharpFunctionalExtensions.Result.Success();
                    })
                    // ищем точки пересечения секущих линий с контуром 0.5H0
                    .Bind(() => FindIntersection())
                    //// test method
                    //.Bind(() =>
                    //{
                    //    using (Transaction tr = new Transaction(_document, "Draw Cutting Line"))
                    //    {
                    //        tr.Start();
                    //        if (_cuttingLines == null)
                    //            tr.RollBack();
                    //        else                            
                    //            _drawLineService.DrawLine(Line.CreateBound(_intersectionPoints[0].Point, _intersectionPoints[1].Point), tr);
                    //        tr.Commit();
                    //    }
                    //    return CSharpFunctionalExtensions.Result.Success();
                    //})
                    // вычисляем параметры
                    .Bind(() => _geometryService.CalculateParameters(_contourHalfH0, _intersectionPoints, _pressureContour))
                    // сохраняем параметры
                    .Bind(() => _pressureCounturInformationService.UpdateParameters(_document, _instance, _pressureContour.ContourParameters));
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", $"Error:{ex.Message}");
            }

        }
        private CSharpFunctionalExtensions.Result FindIntersection()
        {
            if (_contourHalfH0 == null)
                return CSharpFunctionalExtensions.Result.Failure("Контур 0,5H0 не создан");
            if (_cuttingLines == null)
                return CSharpFunctionalExtensions.Result.Failure("Секущие линии не созданы");
            _intersectionPoints = new IntersectionPoint[2];
            var result = _geometryService.LineWithContourIntersection(_cuttingLines, _contourHalfH0);
            if (result.IsSuccess)
                _intersectionPoints = result.Value;
            return CSharpFunctionalExtensions.Result.Success();
        }
        #endregion
    }
}
