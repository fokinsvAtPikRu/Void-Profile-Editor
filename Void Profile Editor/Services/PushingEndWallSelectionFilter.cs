using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Void_Profile_Editor.Services
{
    public class PushingEndWallSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is FamilyInstance)
            {
                return ((FamilyInstance)elem).Symbol.FamilyName == "075_Расчет на продавливание торцом стены (ОбщМод_Плита)";            
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position) => false;
    }
}
