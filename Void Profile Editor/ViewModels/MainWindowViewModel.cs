using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Void_Profile_Editor.Abstraction;
using Void_Profile_Editor.Model;

namespace Void_Profile_Editor.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
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

        // Новый сервис для применения усилий (нужно создать интерфейс)
        private readonly IForceApplicationService _forceApplicationService;

        // Commands
        private readonly AsyncRelayCommand _selectFamilyInstanceCommand;
        private readonly AsyncRelayCommand _createContourCommand;
        private readonly AsyncRelayCommand _createCutingLinesCommand;
        private readonly AsyncRelayCommand _deleteContourCommand;

        // Новые команды для работы с усилиями
        private readonly RelayCommand _applyForcesCommand;
        private readonly RelayCommand _resetAllForcesCommand;
        private readonly RelayCommand<NodeForceData> _resetNodeForcesCommand;
        private readonly RelayCommand<NodeForceData> _copyNodeForcesCommand;

        // Fields
        private Document _document;
        private FamilyInstance _instance;
        private PressureContour _pressureContour;
        private Contour _contour6H0;
        private Contour _contourHalfH0;
        private Line[] _cuttingLines;
        private IntersectionPoint[] _intersectionPoints;
        private ObservableCollection<ElementId> _createdLineIds = new ObservableCollection<ElementId>();

        // Поля для информации и усилий
        private string _selectedElementInfo;
        private string _lengthInfo;
        private int _nodeCount;
        private ObservableCollection<NodeForceData> _nodes;

        #endregion

        #region Observable Properties

        public FamilyInstance Instance
        {
            get => _instance;
            set
            {
                if (SetProperty(ref _instance, value))
                {
                    _createContourCommand.NotifyCanExecuteChanged();

                    // При выборе нового элемента обновляем информацию
                    if (value != null)
                    {
                        UpdateElementInfo();
                    }
                }
            }
        }

        public PressureContour PressureContour
        {
            get => _pressureContour;
            set
            {
                if (SetProperty(ref _pressureContour, value))
                {
                    _createCutingLinesCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public Contour ContourHalfH0
        {
            get => _contourHalfH0;
            set
            {
                if (SetProperty(ref _contourHalfH0, value))
                {
                    _createCutingLinesCommand.NotifyCanExecuteChanged();

                    // При создании контура обновляем количество узлов
                    if (value != null)
                    {
                        UpdateNodeCountFromContour();
                    }
                }
            }
        }

        public ObservableCollection<ElementId> CreatedLineIds
        {
            get => _createdLineIds;
            set => SetProperty(ref _createdLineIds, value);
        }

        public string SelectedElementInfo
        {
            get => _selectedElementInfo;
            set => SetProperty(ref _selectedElementInfo, value);
        }

        public string LengthInfo
        {
            get => _lengthInfo;
            set => SetProperty(ref _lengthInfo, value);
        }

        public int NodeCount
        {
            get => _nodeCount;
            set
            {
                if (SetProperty(ref _nodeCount, value))
                {
                    UpdateNodesCollection(value);
                    _applyForcesCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<NodeForceData> Nodes
        {
            get => _nodes;
            set => SetProperty(ref _nodes, value);
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
            IGeometryService geometryService,
            IForceApplicationService forceApplicationService) // Добавляем новый сервис
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
            _forceApplicationService = forceApplicationService;

            // Initialize collections
            _createdLineIds = new ObservableCollection<ElementId>();
            _nodes = new ObservableCollection<NodeForceData>();

            // Commands
            _selectFamilyInstanceCommand = new AsyncRelayCommand(AsyncSelectSelectFamilyInstance);
            _createContourCommand = new AsyncRelayCommand(AsyncCreateContour, CanCreateContourCommandExecute);
            _createCutingLinesCommand = new AsyncRelayCommand(AsyncCreateCuttingLinesExecute, CanCreateCuttingLinesExecuted);
            _deleteContourCommand = new AsyncRelayCommand(AsyncDeleteLinesExecute, CanDeleteLinesExecuted);

            // Новые команды для усилий
            _applyForcesCommand = new RelayCommand(ApplyForces, CanApplyForces);
            _resetAllForcesCommand = new RelayCommand(ResetAllForces, CanResetAllForces);
            _resetNodeForcesCommand = new RelayCommand<NodeForceData>(ResetNodeForces, CanResetNodeForces);
            _copyNodeForcesCommand = new RelayCommand<NodeForceData>(CopyNodeForces, CanCopyNodeForces);

            // Подписка на изменения коллекции линий
            _createdLineIds.CollectionChanged += (s, e) =>
            {
                _deleteContourCommand.NotifyCanExecuteChanged();
            };
        }

        #endregion

        #region Properties for Command

        public AsyncRelayCommand SelectFamilyInstanceCommand => _selectFamilyInstanceCommand;
        public AsyncRelayCommand CreateContourCommand => _createContourCommand;
        public AsyncRelayCommand CreateCutingLinesCommand => _createCutingLinesCommand;
        public AsyncRelayCommand DeleteContourCommand => _deleteContourCommand;

        // Новые свойства команд для усилий
        public RelayCommand ApplyForcesCommand => _applyForcesCommand;
        public RelayCommand ResetAllForcesCommand => _resetAllForcesCommand;
        public RelayCommand<NodeForceData> ResetNodeForcesCommand => _resetNodeForcesCommand;
        public RelayCommand<NodeForceData> CopyNodeForcesCommand => _copyNodeForcesCommand;

        #endregion

        #region Method Execute for SelectFamilyInstanceCommand

        private async Task AsyncSelectSelectFamilyInstance()
        {
            await _revitTask.Run(app => SelectFamilyInstance());
        }

        private void SelectFamilyInstance()
        {
            var result = _selectionService.PickObject()
                        .Tap(instance => Instance = instance)
                        .Tap(instance => PressureContour = null)
                        .Tap(instance => ContourHalfH0 = null)
                        .Bind(i => _pressureCounturInformationService.CreatePressureContourInfo(i))
                        .Tap(pc => PressureContour = pc);

            if (result.IsFailure)
                ShowErrorMessage($"Ошибка: {result.Error}");
        }

        private void UpdateElementInfo()
        {
            if (Instance == null) return;

            SelectedElementInfo = $"Выбран элемент: {Instance.Name}";

            // Получаем длину элемента (пример - нужно адаптировать под вашу логику)
            var lengthParam = Instance.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
            if (lengthParam != null && lengthParam.HasValue)
            {
                double lengthMm = lengthParam.AsDouble() * 304.8; // перевод в мм
                LengthInfo = $"Длина: {lengthMm:F1} мм";
            }
        }

        #endregion

        #region Method Execute for CreateContourCommand

        private async Task AsyncCreateContour()
        {
            await _revitTask.Run(app => CreateContour());
        }

        private void CreateContour()
        {
            if (Instance == null)
            {
                ShowWarningMessage("Семейство не выбрано");
                return;
            }
            if (PressureContour == null)
            {
                ShowWarningMessage("Контур не создан");
                return;
            }

            var result = Create6H0Contour()
                        .Bind(c => DrawContour(c))
                        .Bind(() => CreateHalfH0Contour());

            if (result.IsFailure)
                ShowErrorMessage($"Ошибка: {result.Error}");
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
            return CSharpFunctionalExtensions.Result.Success(_contour6H0);
        }

        private CSharpFunctionalExtensions.Result<Contour> CreateHalfH0Contour()
        {
            ContourHalfH0 = _createContourService.Create(
               _pressureContour.InsertPoint,
               _pressureContour.Rotation,
               _pressureContour.ContourParameters.DoubleParameters["h0"],
               _pressureContour.ContourParameters.DoubleParameters["Толщина"],
               0.5 * _pressureContour.ContourParameters.DoubleParameters["h0"],
               _instance.Mirrored).Value;
            return CSharpFunctionalExtensions.Result.Success(ContourHalfH0);
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
                        if (line.Key == ContourSideName.TopLeft || line.Key == ContourSideName.TopRight)
                            continue;
                        _drawLineService.DrawLine(line: line.Value,
                            transaction: tr,
                            createdLineIds: _createdLineIds);
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

        private bool CanCreateContourCommandExecute() => Instance != null;

        #endregion

        #region Method Execute for CreateCuttingLinesCommand

        private async Task AsyncCreateCuttingLinesExecute()
        {
            await _revitTask.Run(app => CreateCuttingLinesExecute());
        }

        private bool CanCreateCuttingLinesExecuted() =>
            Instance != null && PressureContour != null && ContourHalfH0 != null;

        private void CreateCuttingLinesExecute()
        {
            try
            {
                _selectionService.PickPoint()
                    .Bind(point => CSharpFunctionalExtensions.Result.Success(new XYZ(point.X, point.Y, 0)))
                    .Bind(point =>
                    {
                        if (_contourHalfH0 == null)
                            return CSharpFunctionalExtensions.Result.Failure("Контур 0,5H0 не создан");
                        _cuttingLines = new Line[2];
                        _cuttingLines[0] = Line.CreateBound(point, _contourHalfH0.Center);
                        return CSharpFunctionalExtensions.Result.Success();
                    })
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
                    .Bind(() => _selectionService.PickPoint())
                    .Bind(point => CSharpFunctionalExtensions.Result.Success(new XYZ(point.X, point.Y, 0)))
                    .Bind(point =>
                    {
                        _cuttingLines[1] = Line.CreateBound(point, _contourHalfH0.Center);
                        return CSharpFunctionalExtensions.Result.Success();
                    })
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
                    .Bind(() => FindIntersection())
                    .Tap(() =>
                    {
                        var orderedPoints = _intersectionPoints
                            .OrderBy(p => p.SideName)
                            .ThenBy(p => p.Point.DistanceTo(_contourHalfH0.GetLine(p.SideName).GetEndPoint(0)))
                            .ToArray();
                        _intersectionPoints = orderedPoints;
                    })
                    .Bind(() => _geometryService.CalculateParameters(_contourHalfH0, _intersectionPoints, _pressureContour))
                    .Bind(() => _pressureCounturInformationService.UpdateParameters(_document, _instance, _pressureContour.ContourParameters))
                    .Tap(() => UpdateNodeCountFromContour()); // Обновляем количество узлов после расчета
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка: {ex.Message}");
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

        #region Method Execute for Delete Contour Line Command

        private async Task AsyncDeleteLinesExecute()
        {
            await _revitTask.Run(app => DeleteLinesExecute(CreatedLineIds));
        }

        public CSharpFunctionalExtensions.Result DeleteLinesExecute(ObservableCollection<ElementId> lineIds)
        {
            if (lineIds == null || lineIds.Count == 0)
                return CSharpFunctionalExtensions.Result.Success();

            if (_document == null)
                return CSharpFunctionalExtensions.Result.Failure("Документ не инициализирован");

            try
            {
                using (Transaction tr = new Transaction(_document, "Удаление линий"))
                {
                    tr.Start();

                    var validIds = lineIds
                        .Where(id => id != null && _document.GetElement(id) != null)
                        .ToList();

                    if (validIds.Count == 0)
                    {
                        tr.RollBack();
                        return CSharpFunctionalExtensions.Result.Success();
                    }

                    _document.Delete(validIds);
                    tr.Commit();

                    lineIds.Clear();

                    return CSharpFunctionalExtensions.Result.Success();
                }
            }
            catch (Exception ex)
            {
                return CSharpFunctionalExtensions.Result.Failure($"Ошибка при удалении линий: {ex.Message}");
            }
        }

        #endregion

        #region Can Executed Delete Contour Lines Command

        private bool CanDeleteLinesExecuted() => CreatedLineIds.Count > 0;

        #endregion

        #region Methods for Node Forces

        /// <summary>
        /// Обновление количества узлов на основе контура
        /// </summary>
        private void UpdateNodeCountFromContour()
        {
            var length = _pressureContour.ContourParameters.DoubleParameters["ho"] / 2 + _pressureContour.ContourParameters.DoubleParameters["Толщина"];
            if (ContourHalfH0 == null)
            {
                NodeCount = 0;
                return;
            }

            // Логика определения количества узлов в зависимости от длины/периметра контура
            // Это пример - нужно адаптировать под вашу логику
            double perimeter = 0;
            foreach (var line in ContourHalfH0.Values)
            {
                perimeter += line.Length;
            }

            if (perimeter <= 1000) NodeCount = 2;
            else if (perimeter <= 2000) NodeCount = 3;
            else if (perimeter <= 3000) NodeCount = 4;
            else NodeCount = 5;
        }

        /// <summary>
        /// Обновление коллекции узлов
        /// </summary>
        private void UpdateNodesCollection(int count)
        {
            var newNodes = new ObservableCollection<NodeForceData>();

            // Сохраняем существующие значения, если они есть
            for (int i = 1; i <= count; i++)
            {
                var existingNode = Nodes?.FirstOrDefault(n => n.NodeName == $"Узел {i}");
                if (existingNode != null)
                {
                    newNodes.Add(existingNode);
                }
                else
                {
                    newNodes.Add(new NodeForceData($"Узел {i}", 0, 0, 0));
                }
            }

            Nodes = newNodes;
        }

        /// <summary>
        /// Применение усилий к узлам
        /// </summary>
        private void ApplyForces()
        {
            if (Nodes == null || Nodes.Count == 0)
            {
                ShowWarningMessage("Нет узлов для применения усилий!");
                return;
            }

            // Проверка валидности значений
            foreach (var node in Nodes)
            {
                if (!node.HasValidValues)
                {
                    var result = ShowQuestionMessage(
                        $"В узле {node.NodeName} обнаружены некорректные значения.\nПродолжить?",
                        "Подтверждение");

                    if (result == MessageBoxResult.No)
                        return;

                    break;
                }
            }

            // Применение усилий через сервис
            try
            {
                _revitTask.Run(async app =>
                {
                    var result = _forceApplicationService.ApplyForces(_document, _instance, Nodes.ToList());

                    if (result.IsFailure)
                    {
                        ShowErrorMessage($"Ошибка при применении усилий: {result.Error}");
                    }
                    else
                    {
                        ShowInfoMessage("Усилия успешно применены!");
                    }
                });
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка: {ex.Message}");
            }
        }

        private bool CanApplyForces()
        {
            return Nodes != null && Nodes.Count > 0 && Instance != null;
        }

        /// <summary>
        /// Сброс всех усилий
        /// </summary>
        private void ResetAllForces()
        {
            var result = ShowQuestionMessage("Сбросить все значения усилий?", "Подтверждение");

            if (result == MessageBoxResult.Yes)
            {
                foreach (var node in Nodes)
                {
                    node.Reset();
                }
                ShowInfoMessage("Все значения сброшены!");
            }
        }

        private bool CanResetAllForces()
        {
            return Nodes != null && Nodes.Count > 0;
        }

        /// <summary>
        /// Сброс усилий для конкретного узла
        /// </summary>
        private void ResetNodeForces(NodeForceData node)
        {
            if (node != null)
            {
                var result = ShowQuestionMessage(
                    $"Сбросить значения для {node.NodeName}?",
                    "Подтверждение");

                if (result == MessageBoxResult.Yes)
                {
                    node.Reset();
                }
            }
        }

        private bool CanResetNodeForces(NodeForceData node)
        {
            return node != null;
        }

        /// <summary>
        /// Копирование значений узла во все остальные узлы
        /// </summary>
        private void CopyNodeForces(NodeForceData sourceNode)
        {
            if (sourceNode != null && Nodes.Count > 1)
            {
                var result = ShowQuestionMessage(
                    "Копировать значения во все узлы?",
                    "Подтверждение");

                if (result == MessageBoxResult.Yes)
                {
                    foreach (var node in Nodes)
                    {
                        if (node != sourceNode)
                        {
                            node.N = sourceNode.N;
                            node.Mx = sourceNode.Mx;
                            node.My = sourceNode.My;
                        }
                    }

                    ShowInfoMessage("Значения скопированы!");
                }
            }
        }

        private bool CanCopyNodeForces(NodeForceData sourceNode)
        {
            return sourceNode != null && Nodes != null && Nodes.Count > 1;
        }

        #endregion

        #region Helper Methods for UI Dialogs

        private void ShowErrorMessage(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                TaskDialog.Show("Ошибка", message);
            });
        }

        private void ShowWarningMessage(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                TaskDialog.Show("Предупреждение", message);
            });
        }

        private void ShowInfoMessage(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                TaskDialog.Show("Информация", message);
            });
        }

        private MessageBoxResult ShowQuestionMessage(string message, string title = "Подтверждение")
        {
            return (MessageBoxResult)Application.Current.Dispatcher.Invoke(new Func<MessageBoxResult>(() =>
            {
                return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            }));
        }

        #endregion
    }
}