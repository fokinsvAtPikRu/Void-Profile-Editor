using Autodesk.Revit.DB;
using System.Collections;
using System.Collections.Generic;

namespace Void_Profile_Editor.Domain.Model.Geometry
{
    public class Contour : IEnumerable<KeyValuePair<ContourSideName, DetailLineDomain>>
    {
        public DetailLineDomain Bottom { get; set; }
        public DetailLineDomain Left { get; set; }
        public DetailLineDomain Right { get; set; }
        public DetailLineDomain TopLeft { get; set; }
        public DetailLineDomain TopRight { get; set; }
        public Point3DDomain Center { get; set; }

        public DetailLineDomain GetLine(ContourSideName side)
        {
            switch (side)
            {
                case ContourSideName.Bottom:
                    return Bottom;
                case ContourSideName.Left:
                    return Left;
                case ContourSideName.Right:
                    return Right;
                case ContourSideName.TopLeft:
                    return TopLeft;
                case ContourSideName.TopRight:
                    return TopRight;
                default:
                    return null;
            };
        }

        public IEnumerator<KeyValuePair<ContourSideName, DetailLineDomain>> GetEnumerator()
        {
            yield return new KeyValuePair<ContourSideName, DetailLineDomain>(ContourSideName.TopLeft, TopLeft);
            yield return new KeyValuePair<ContourSideName, DetailLineDomain>(ContourSideName.Left, Left);
            yield return new KeyValuePair<ContourSideName, DetailLineDomain>(ContourSideName.Bottom, Bottom);
            yield return new KeyValuePair<ContourSideName, DetailLineDomain>(ContourSideName.Right, Right);
            yield return new KeyValuePair<ContourSideName, DetailLineDomain>(ContourSideName.TopRight, TopRight);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
