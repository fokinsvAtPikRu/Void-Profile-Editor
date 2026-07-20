using Autodesk.Revit.DB;
using Void_Profile_Editor.DTOs;

namespace Void_Profile_Editor.Infrastructure.Model
{
    public class RevitPressureContourParameters
    {
        private string _id;
        private Configuration.FamilyType _familyType;
        private Dimensions _dimensions;
        private double _rotation;
        private bool _isMirrored;

        public RevitPressureContourParameters(
            string id,
            Configuration.FamilyType familyType,
            Dimensions dimensions,
            double rotation,
            bool isMirrored)
        {
            _id = id;
            _familyType = familyType;
            _dimensions = dimensions;
            _rotation = rotation;
            _isMirrored = isMirrored;
        }
    }
}
