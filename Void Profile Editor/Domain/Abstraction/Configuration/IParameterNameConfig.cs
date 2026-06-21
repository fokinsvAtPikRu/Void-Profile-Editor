using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Void_Profile_Editor.Domain.Model.Geometry;

namespace Void_Profile_Editor.Domain.Abstraction.Configuration
{
    public interface IParameterNameConfig
    {
        string GetEdgeEnabledParameter(ContourSideName side);
        (string offset, string width) GetHoleParameters(ContourSideName side);
        string GetOffsetParameter(ContourSideName side,bool isStartPoint);
        bool ValidateParameters(PressureContourParameters parameters);
    }
}
