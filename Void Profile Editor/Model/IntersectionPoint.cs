using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Void_Profile_Editor.Model
{
    public class IntersectionPoint
    {
        public IntersectionPoint(XYZ point, ContourSideName sideName)
        {
            Point = point;
            SideName = sideName;
        }

        public XYZ Point {  get; set; }
        public ContourSideName SideName {get; set;} 
    }
}
