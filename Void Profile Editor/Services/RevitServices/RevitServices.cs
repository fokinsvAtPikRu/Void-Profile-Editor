using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Void_Profile_Editor.Services.RevitServices
{
    public class RevitServices
    {   
        private Document _document;
        public RevitServices(Document document)
        {
            _document = document;
        }

    }
}
