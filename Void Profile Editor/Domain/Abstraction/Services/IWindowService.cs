using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Void_Profile_Editor.Domain.Abstraction.Services
{
    internal interface IWindowService
    {
        void HideWindow();
        void ShowWindow();
        void BringToFront();
    }
}
