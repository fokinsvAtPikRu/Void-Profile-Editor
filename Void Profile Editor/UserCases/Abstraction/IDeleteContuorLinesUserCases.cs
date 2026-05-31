using CSharpFunctionalExtensions;
using System.Collections.Generic;

namespace Void_Profile_Editor.UserCases.Abstraction
{
    public interface IDeleteContuorLinesUserCases
    {
        Result DeleteLines(List<string> linesIdsDomain);
    }
}
