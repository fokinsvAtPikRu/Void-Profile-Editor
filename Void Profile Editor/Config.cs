using Autodesk.Revit.UI.Selection;
using Microsoft.Extensions.DependencyInjection;
using RxBim.Di;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Void_Profile_Editor.Abstraction;
using Void_Profile_Editor.Services;
using Void_Profile_Editor.ViewModels;
using Void_Profile_Editor.Views;

namespace Void_Profile_Editor
{
    public class Config : ICommandConfiguration
    {
        public void Configure(IServiceCollection services)
        {
            // MyServices
            services.AddSingleton<RevitTask>(new RevitTask());
            services.AddSingleton<ICreateContourService, CreateContourService>();
            services.AddSingleton<IDrawLineService, DrawLineService>();
            services.AddSingleton<IGeometryService, GeometryService>();
            services.AddSingleton<IPressureCounturInformationService, PressureCounturInformationService>();
            services.AddSingleton<ISelectionService, SelectionService>();
            // Window Service
            services.AddSingleton<IWindowService, WindowService>();
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
