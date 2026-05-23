using Autodesk.Revit.DB;

namespace Void_Profile_Editor.Model.DTOs
{
    public class CreateCounturDto
    {
        public Element Instance {  get; set; }
        public PressureContour PressureContour { get; set; }
        public bool IsMirrored { get; set; }
    }
}
