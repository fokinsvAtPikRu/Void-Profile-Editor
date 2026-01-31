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

namespace Void_Profile_Editor
{
    public class Config : ICommandConfiguration
    {
        public void Configure(IServiceCollection services)
        {
            services.AddSingleton<RevitTask>(new RevitTask());
            services.AddSingleton<ISelectionService, SelectionService>();
            services.AddSingleton<IPressureCounturInformationService, PressureCounturInformationService>();
            services.AddSingleton<ISelectionService, SelectionService>();
            services.AddSingleton<IDrawLineService, DrawLineService>();
            services.AddSingleton<IGeometryService, GeometryService>();

        }
    }
}
