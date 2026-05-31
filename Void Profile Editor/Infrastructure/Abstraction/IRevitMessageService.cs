using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Void_Profile_Editor.Infrastructure.Abstraction
{
    public interface IRevitMessageService
    {
        void ShowMessage(string head, string message);
    }
}
