using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using Void_Profile_Editor.Infrastructure.Abstraction;
using Void_Profile_Editor.UserCases.Abstraction;
using Void_Profile_Editor.UserCases.Results;

namespace Void_Profile_Editor.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        #region UserCases
        private readonly ISelectInstanceUserCase _selectInstanceUseCase;
        private readonly ICreateContourUserCase _createContourUserCase;
        private readonly ICreateCuttingLinesUserCase _createCuttingLinesUserCase;
        private readonly IDeleteContuorLinesUserCases _deleteContuorLinesUserCases;
        #endregion
        #region Fields
        // RevitTask
        private RevitTask _revitTask;
        // Services
        private readonly IRevitMessageService _revitMessageService;

        // Commands
        private readonly AsyncRelayCommand _selectFamilyInstanceCommand;
        private readonly AsyncRelayCommand _createContourCommand;
        private readonly AsyncRelayCommand _createCutingLinesCommand;
        private readonly AsyncRelayCommand _deleteContourCommand;

        // Results UserCase
        private ResultSelectInstanceUserCase _resultSelectInstanceUserCase;
        private ResultCreateContourUserCase _resultCreateContourUserCase;
       

        // Observable Properties
        public ResultSelectInstanceUserCase ResultSelectInstance
        {
            get => _resultSelectInstanceUserCase;
            set
            {
                if (SetProperty(ref _resultSelectInstanceUserCase, value))
                {
                    _createContourCommand.NotifyCanExecuteChanged();
                    _createCutingLinesCommand.NotifyCanExecuteChanged();
                    _deleteContourCommand.NotifyCanExecuteChanged();
                }
            }
        }
        public ResultCreateContourUserCase ResultCreateContour
        {
            get => _resultCreateContourUserCase;
            set
            {
                if (SetProperty(ref _resultCreateContourUserCase, value))
                {
                    _createCutingLinesCommand.NotifyCanExecuteChanged();
                    _deleteContourCommand.NotifyCanExecuteChanged();
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
            // Message Service
            _revitMessageService=revitMessageService;
            // Commands
            _selectFamilyInstanceCommand = new AsyncRelayCommand(AsyncSelectSelectFamilyInstance);
            _createContourCommand = new AsyncRelayCommand(AsyncCreateContour, CanCreateContourCommandExecuted);
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
            ResultSelectInstance=null;
            ResultCreateContour=null;
            var result = await _selectInstanceUseCase.RunAsync();
            if (result.IsSuccess)
                ResultSelectInstance = result.Value;
            else 
                _revitMessageService.ShowMessage("Error", $"Error:{result.Error}");
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
            var result = _createContourUserCase.CreateContour(ResultSelectInstance);
            if (result.IsFailure)
                _revitMessageService.ShowMessage("Ошибка", $"Error:{result.Error}");
            else
                ResultCreateContour = result.Value;
        }
        #endregion
        #region CanExecute for CreateContourCommand
        private bool CanCreateContourCommandExecuted() =>
            ResultSelectInstance != null;
        #endregion
        #region Method Execute for CreateCuttingLinesCommand
        private async Task AsyncCreateCuttingLinesExecute()
        {
            await _revitTask.Run(app => CreateCuttingLinesExecute());
        }
        private bool CanCreateCuttingLinesExecuted() =>
            ResultSelectInstance != null && ResultCreateContour != null;
        private void CreateCuttingLinesExecute()
        {
            var result = _createCuttingLinesUserCase.CreateCuttingLines(
                ResultCreateContour.ContourHalfH0,
                ResultSelectInstance.PressureContour);
            if (result.IsFailure)
                _revitMessageService.ShowMessage("Ошибка", result.Error);
        }
        
        #endregion
        #region Method Execute for Delete Contour Line Command
        private async Task AsyncDeleteLinesExecute()
        {
            await _revitTask.Run(app => DeleteLinesExecute());
        }
        public void DeleteLinesExecute()
        {
            var result = _deleteContuorLinesUserCases.DeleteLines(ResultCreateContour.LinesIdsForDelete);
            if (result.IsFailure)            
                _revitMessageService.ShowMessage("Ошибка", result.Error);
            else
            {
                ResultCreateContour = null;
                ResultSelectInstance = null;
            }
        }
        #endregion
        #region Can Executed Delete Contour Lines Command
        private bool CanDeleteLinesExecuted() => ResultCreateContour != null;
        #endregion
    }
}
