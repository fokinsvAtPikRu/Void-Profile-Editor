using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using Void_Profile_Editor.Domain.Configuration;

namespace Void_Profile_Editor.Infrastructure.Services
{
    public class CalculatedPushingSelectionFilter : ISelectionFilter
    {
        private readonly IAllowedFamiliesConfig _config;
        private readonly bool _showWarnings;

        public CalculatedPushingSelectionFilter(IAllowedFamiliesConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));            
        }

        public bool AllowElement(Element elem)
        {
            if (elem is FamilyInstance instance)
            {
                var familyName = instance.Symbol?.FamilyName;
                if (string.IsNullOrEmpty(familyName))
                    return false;
                return _config.IsAllowed(familyName);
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position) => false;
    }
}
