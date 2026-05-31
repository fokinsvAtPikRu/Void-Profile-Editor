using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using Void_Profile_Editor.Infrastructure.Abstraction;
using Void_Profile_Editor.UserCases.Abstraction;
using Void_Profile_Editor.UserCases.Results;
using Void_Profile_Editor.UserCases.Abstraction;

namespace Void_Profile_Editor.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        #region UseCases
        private readonly ISelectInstanceUserCase _selectInstanceUseCase;
        private readonly ICreateContourUserCase _createContourUserCase;
        private readonly ICreateCuttingLinesUserCase _createCuttingLinesUserCase;
        private readonly IDeleteContuorLinesUserCases _deleteContuorLinesUserCases;
        #endregion
        #region Fields
        // RevitTask
        private RevitTask _revitTask;
        // Services
        private readonly IRevitMessageService _revitMsaageService;

        // Commands
        private readonly AsyncRelayCommand _selectFamilyInstanceCommand;
        private readonly AsyncRelayCommand _createContourCommand;
        private readonly AsyncRelayCommand _createCutingLinesCommand;
        private readonly AsyncRelayCommand _deleteContourCommand;

        // Results UserCase
        private ResultSelectInstanceUserCase _resultSelectInstanceUseCase;
        private ResultCreateContourUserCase _resultCreateContourUseCase;
       

        // Observable Properties
        public ResultSelectInstanceUserCase ResultSelectInstanceUseCase
        {
            get => _resultSelectInstanceUseCase;
            set
            {
                if (SetProperty(ref _resultSelectInstanceUseCase, value))
                {
                    _createContourCommand.NotifyCanExecuteChanged();
                }
            }
        }
        public ResultCreateContourUserCase ResultCreateContourUseCase
        {
            get => _resultCreateContourUseCase;
            set
            {
                if (SetProperty(ref _resultCreateContourUseCase, value))
                {
                    _createCutingLinesCommand.NotifyCanExecuteChanged();
                }
            }
        }
        #endregion
        #region ctor
        public MainWindowViewModel(
            // Revit Task
            RevitTask revitTask,
            // UserCases
            ISelectInstanceUserCase selectInstanceUseCase,
            ICreateContourUserCase createContourUserCase,
            ICreateCuttingLinesUserCase createCuttingLinesUserCase,
            IDeleteContuorLinesUserCases deleteContuorLinesUserCases,
            // Services
            IRevitMessageService revitMessageService)
        {
            // Revit Task
            _revitTask = revitTask;
            // UserCases
            _selectInstanceUseCase = selectInstanceUseCase;
            _createContourUserCase = createContourUserCase;
            _createCuttingLinesUserCase = createCuttingLinesUserCase;            
            _deleteContuorLinesUserCases = deleteContuorLinesUserCases;
            // Commands
            _selectFamilyInstanceCommand = new AsyncRelayCommand(AsyncSelectSelectFamilyInstance);
            _createContourCommand = new AsyncRelayCommand(AsyncCreateContour, CanCreateContourCommandExecute);
            _createCutingLinesCommand = new AsyncRelayCommand(AsyncCreateCuttingLinesExecute, CanCreateCuttingLinesExecuted);
            _deleteContourCommand = new AsyncRelayCommand(AsyncDeleteLinesExecute, CanDeleteLinesExecuted);
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
        public AsyncRelayCommand DeleteContourCommand
        {
            get => _deleteContourCommand;
        }
        #endregion
        #region Method Execute for SelectFamilyInstanceCommand
        // Method Execute for SelectFamilyInstanceCommand
        private async Task AsyncSelectSelectFamilyInstance()
        {
            _resultSelectInstanceUseCase=null;
            _resultCreateContourUseCase=null;
            var result = await _selectInstanceUseCase.RunAsync();
            if (result.IsSuccess)
                _resultSelectInstanceUseCase = result.Value;
            else 
                _revitMsaageService.ShowMessage("Error", $"Error:{result.Error}");
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
            var result = _createContourUserCase.CreateContour(_resultSelectInstanceUseCase);
            if (result.IsFailure)
                _revitMsaageService.ShowMessage("Ошибка", $"Error:{result.Error}");
            else
                _resultCreateContourUseCase = result.Value;
        }
        #endregion
        #region CanExecute for CreateContourCommand
        private bool CanCreateContourCommandExecute() =>
            _resultSelectInstanceUseCase != null;
        #endregion
        #region Method Execute for CreateCuttingLinesCommand
        private async Task AsyncCreateCuttingLinesExecute()
        {
            await _revitTask.Run(app => CreateCuttingLinesExecute());
        }
        private bool CanCreateCuttingLinesExecuted() =>
            _resultSelectInstanceUseCase != null && _resultCreateContourUseCase != null;
        private void CreateCuttingLinesExecute()
        {
            var result = _createCuttingLinesUserCase.CreateCuttingLines(
                _resultCreateContourUseCase.ContourHalfH0,
                _resultSelectInstanceUseCase.PressureContour);
            if (result.IsFailure)
                _revitMsaageService.ShowMessage("Ошибка", result.Error);
        }
        
        #endregion
        #region Method Execute for Delete Contour Line Command
        private async Task AsyncDeleteLinesExecute()
        {
            await _revitTask.Run(app => DeleteLinesExecute());
        }
        public void DeleteLinesExecute()
        {
            var result = _deleteContuorLinesUserCases.DeleteLines(_resultCreateContourUseCase.LinesIdsForDelete);
            if (result.IsFailure)            
                _revitMsaageService.ShowMessage("Ошибка", result.Error);            
        }
        #endregion
        #region Can Executed Delete Contour Lines Command
        private bool CanDeleteLinesExecuted() => _resultCreateContourUseCase == null;
        #endregion
    }
}
