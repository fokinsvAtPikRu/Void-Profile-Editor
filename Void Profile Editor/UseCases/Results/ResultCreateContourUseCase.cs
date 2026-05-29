using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Void_Profile_Editor.Domain.Model.Geometry;

namespace Void_Profile_Editor.UseCases.Results
{
    public class ResultCreateContourUseCase
    {
        public Contour Contour6H0 { get; set; }
        public Contour ContourHalfH0 { get; set; }
        public List<string> LinesIdsForDelete { get; set; }
    }
}
