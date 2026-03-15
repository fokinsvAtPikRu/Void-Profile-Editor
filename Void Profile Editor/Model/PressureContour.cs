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
        public XYZ InsertPoint { get; set; }
        public double Rotation { get; set; }        
        public bool IsMirrored { get; set; }

        public PressureContourParameters ContourParameters { get; set; }
        
        public PressureContour()
        {
            ContourParameters=new PressureContourParameters();
        }
    }
}
