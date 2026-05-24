using Autodesk.Revit.DB;
using System.Collections;
using System.Collections.Generic;

namespace Void_Profile_Editor.Domain.Model.Geometry
{
    public class Contour : IEnumerable<KeyValuePair<ContourSideName, LineSegmentDomain>>
    {
        public LineSegmentDomain Bottom { get; set; }
        public LineSegmentDomain Left { get; set; }
        public LineSegmentDomain Right { get; set; }
        public LineSegmentDomain TopLeft { get; set; }
        public LineSegmentDomain TopRight { get; set; }
        public Point3DDomain Center { get; set; }

        public LineSegmentDomain GetLine(ContourSideName side)
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

        public IEnumerator<KeyValuePair<ContourSideName, LineSegmentDomain>> GetEnumerator()
        {
            yield return new KeyValuePair<ContourSideName, LineSegmentDomain>(ContourSideName.TopLeft, TopLeft);
            yield return new KeyValuePair<ContourSideName, LineSegmentDomain>(ContourSideName.Left, Left);
            yield return new KeyValuePair<ContourSideName, LineSegmentDomain>(ContourSideName.Bottom, Bottom);
            yield return new KeyValuePair<ContourSideName, LineSegmentDomain>(ContourSideName.Right, Right);
            yield return new KeyValuePair<ContourSideName, LineSegmentDomain>(ContourSideName.TopRight, TopRight);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
