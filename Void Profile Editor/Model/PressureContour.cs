using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Void_Profile_Editor.Model
{
    public class PressureContour
    {
        public ElementId Id {  get; set; }
        public double H0 { get; set; }
        public double WallThickness { get; set; }
        public XYZ InsertPoint { get; set; }
        public double Rotation { get; set; }
        public XYZ Center { get; set; }
    }
}
