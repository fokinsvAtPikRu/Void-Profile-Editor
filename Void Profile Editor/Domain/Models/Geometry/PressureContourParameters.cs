using System.Collections.Generic;

namespace Void_Profile_Editor.Domain.Model.Geometry
{
    public class PressureContourParameters
    {
        public string FamilyName { get; }
        public List<ContourSideName> ActiveEdge { get; set; }
        public Dictionary<string, double> DoubleParameters { get; set; }
        public Dictionary<string, int> IntParameters { get; set; }
        // public PressureContourParameters() { }
        public PressureContourParameters(
            string familyName,
            List<ContourSideName> activeEdge,
            Dictionary<string,double> doubleParameters=null,
            Dictionary<string,int> intParameters = null) 
        {
            FamilyName = familyName;
            ActiveEdge = activeEdge;
            DoubleParameters = doubleParameters;
            IntParameters = intParameters;
        }
    }
}
