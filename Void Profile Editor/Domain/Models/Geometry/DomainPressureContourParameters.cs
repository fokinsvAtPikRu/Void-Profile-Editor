using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace Void_Profile_Editor.Domain.Model.Geometry
{
    public class DomainPressureContourParameters
    {        
        public Dictionary<string, double> DoubleParameters { get; set; }
        public Dictionary<string, int> IntParameters { get; set; }
       
        public DomainPressureContourParameters(            
            Dictionary<string,double> doubleParameters=null,
            Dictionary<string,int> intParameters = null) 
        {            
            DoubleParameters = doubleParameters;
            IntParameters = intParameters;
        }
    }
}
