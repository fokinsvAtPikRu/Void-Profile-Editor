using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using System.Threading.Tasks;
using Void_Profile_Editor.Abstraction;
using Void_Profile_Editor.Model;

namespace Void_Profile_Editor.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        // RevitTask
        private RevitTask _revitTask;

        // Services
        private readonly ISelectionService _selectionService;
        private readonly IPressureCounturInformationService _pressureCounturInformationService;
        private readonly ICreateContourService _createContourService;

        // Commands
        private readonly AsyncRelayCommand _selectFamilyInstanceCommand;
        private readonly AsyncRelayCommand _createContourCommand;


        // Fields
        private FamilyInstance _instance;
        private PressureContour _pressureContour;
        private Contour _contour6H0;
        private Contour _contourHalfH0;

        public MainWindowViewModel(
            RevitTask revitTask,
            ISelectionService selection,
            IPressureCounturInformationService pressureCounturInformationService,
            ICreateContourService createContourService)
        {
            _revitTask = revitTask;
            _selectionService = selection;
            _pressureCounturInformationService = pressureCounturInformationService;
            _createContourService = createContourService;
            _selectFamilyInstanceCommand = new AsyncRelayCommand(SelectFamilyInstance);
        }

        // Properties for Commands
        public AsyncRelayCommand SelectFamilyInstanceCommand
        {
            get => _selectFamilyInstanceCommand;
        }
        public AsyncRelayCommand CreateContourCommand
        {
            get => _createContourCommand;
        }

        // Method Execute for SelectFamilyInstanceCommand
        private async Task SelectFamilyInstance()
        {
            await SelectInstance().
                 Bind(SetInstance).
                 Bind(CreatePressureContourInfo).
                 Tap(SetPressureContour);            
        }
        private async Task<CSharpFunctionalExtensions.Result<FamilyInstance>> SelectInstance() =>
            await _revitTask.Run<CSharpFunctionalExtensions.Result<FamilyInstance>>(app => _selectionService.PickObject());
        private CSharpFunctionalExtensions.Result<FamilyInstance> SetInstance(FamilyInstance instance)
        {
            _instance = instance;
            return _instance;
        }
        private CSharpFunctionalExtensions.Result<PressureContour> CreatePressureContourInfo(FamilyInstance instance) => 
            _pressureCounturInformationService.CreatePressureContourInfo(_instance);
        private void SetPressureContour(PressureContour pressureContour)
        {
            _pressureContour = pressureContour;
        }

        // Method Execute for CreateContourCommand
        private async Task CreateContour()
        {
            await Create6H0Contour();
        }
        private async Task<CSharpFunctionalExtensions.Result<Contour>> Create6H0Contour() =>
            await _revitTask.Run<CSharpFunctionalExtensions.Result<Contour>>
                (app => _createContourService.Create(
                _pressureContour.InsertPoint,
                _pressureContour.Rotation,
                _pressureContour.H0,
                _pressureContour.WallThickness,
                6 * _pressureContour.H0,
                _instance.Mirrored));
        private async Task<CSharpFunctionalExtensions.Result> Draw6H0Contour(Contour contour)
        {
            await _revitTask.Run
        }

    }
}
