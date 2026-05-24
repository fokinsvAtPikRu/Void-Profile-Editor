using Autodesk.Revit.DB;
using Void_Profile_Editor.Domain.Model.Geometry;

namespace Void_Profile_Editor.Domain.Model.DTOs
{
    public class CreateCounturDto
    {
        public Element Instance {  get; set; }
        public PressureContour PressureContour { get; set; }
        public bool IsMirrored { get; set; }
    }
}
