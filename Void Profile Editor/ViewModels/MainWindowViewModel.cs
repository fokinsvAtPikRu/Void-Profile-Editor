using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using Void_Profile_Editor.Abstraction;
using Void_Profile_Editor.Model;
using Void_Profile_Editor.Services;

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
            _pressureCounturInformationService=pressureCounturInformationService;
            _createContourService= createContourService;
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
            CSharpFunctionalExtensions.Result<FamilyInstance> resultSelectInstance = await _revitTask.Run<CSharpFunctionalExtensions.Result<FamilyInstance>>(app =>
                  _selectionService.PickObject());
            if (resultSelectInstance.IsSuccess)
            {
                _instance = resultSelectInstance.Value;
                var  resultCreatePressureContourInfo= _pressureCounturInformationService.CreatePressureConturInfo(_instance);
                if (resultCreatePressureContourInfo.IsSuccess) 
                    _pressureContour=resultCreatePressureContourInfo.Value;
            }
        }
        // Method Execute for CreateContourCommand
        private async Task CreateContour()
        {
            CSharpFunctionalExtensions.Result<Contour> result = await _revitTask.Run<CSharpFunctionalExtensions.Result<Contour>>
                (app => _createContourService.Create(
                _pressureContour.InsertPoint,
                _pressureContour.Rotation,
                _pressureContour.H0,
                _pressureContour.WallThickness,
                6 * _pressureContour.H0,
                _instance.Mirrored));
        }
    }
}
