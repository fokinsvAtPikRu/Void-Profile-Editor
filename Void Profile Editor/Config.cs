using Microsoft.Extensions.DependencyInjection;
using RxBim.Di;
using Void_Profile_Editor.Domain.Abstraction.Services;
using Void_Profile_Editor.Domain.Services;
using Void_Profile_Editor.Infrastructure.Abstraction;
using Void_Profile_Editor.Infrastructure.Services;
using Void_Profile_Editor.UserCases.Abstraction;
using Void_Profile_Editor.UserCases.Cases;
using Void_Profile_Editor.ViewModels;
using Void_Profile_Editor.Views;

namespace Void_Profile_Editor
{
    public class Config : ICommandConfiguration
    {
        public void Configure(IServiceCollection services)
        {
            // RevitTask
            services.AddSingleton<RevitTask>(new RevitTask());
            // Domain/Sevices
            services.AddSingleton<ICreateContourService, CreateContourService>();
            services.AddSingleton<IGeometryService, GeometryService>();
            // Infrastructure/Services
            services.AddSingleton<IRevitLineService, RevitLineService>();
            services.AddSingleton<IRevitMessageService, RevitMessageService>();
            services.AddSingleton<IRevitSelectionServices,RevitSelectionServices>();
            services.AddSingleton<IRevitUpdateParametersService, RevitUpdateParametersService>();
            services.AddSingleton<ISelectionService, SelectionService>();
            // Window Service
            services.AddSingleton<IWindowService, WindowService>();
            // UserCases
            services.AddSingleton<ICreateContourUserCase, CreateCountourUserCase>();
            services.AddSingleton<ICreateCuttingLinesUserCase, CreateCuttingLinesUserCase>();
            services.AddSingleton<ISelectInstanceUserCase, SelectInstanceUserCase>();
            services.AddSingleton<IDeleteContuorLinesUserCases, DeleteContuorLinesUserCases>();

            // View Model
            services.AddSingleton<MainWindowViewModel>();
            // Main Window
            services.AddSingleton<MainWindow>(provider =>
            {
                var viewModel = provider.GetRequiredService<MainWindowViewModel>();

                var window = ActivatorUtilities.CreateInstance<MainWindow>(provider, viewModel);

                var windowService = provider.GetRequiredService<IWindowService>() as WindowService;
                windowService?.SetWindow(window);

                return window;
            });

        }
    }
}
