using CSharpFunctionalExtensions;
using System.Collections.Generic;
using Void_Profile_Editor.Infrastructure.Abstraction;
using Void_Profile_Editor.UserCases.Abstraction;

namespace Void_Profile_Editor.UserCases.Cases
{
    public class DeleteContuorLinesUserCases : IDeleteContuorLinesUserCases
    {
        private readonly IRevitLineService _revitLineService;
        public DeleteContuorLinesUserCases(
            IRevitLineService revitLineService)
        {
            _revitLineService = revitLineService;
        }
        public Result DeleteLines(List<string> linesIdsDomain) =>
            _revitLineService.DeleteLines(linesIdsDomain);

    }
}
