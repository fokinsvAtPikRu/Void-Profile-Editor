using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;
using System.Collections.Generic;
using Void_Profile_Editor.Domain.Abstraction.Configuration;
using Void_Profile_Editor.Domain.Model.Geometry;
using Void_Profile_Editor.DTOs;

namespace Void_Profile_Editor.Infrastructure.Model
{
    public class RevitPressureContour
    {        
        // fields
        private RevitPressureContourParameters _revitParameters;
        private DomainPressureContourParameters _domainParameters;        

        // ctor
        public RevitPressureContour(
            string id, 
            Configuration.FamilyType familyType, 
            Dimensions dimensions, 
            double rotation, 
            bool isMirrored, 
            Dictionary<string,double> doubleParameters,
            Dictionary<string,int> intParameters)
        {
            _revitParameters = new RevitPressureContourParameters(id, familyType, dimensions, rotation, isMirrored);
            _domainParameters= new DomainPressureContourParameters(doubleParameters, intParameters);
        }


        public Result<RevitPressureContour> Create(FamilyInstance instance, dou)
        {

        }
    }
}
