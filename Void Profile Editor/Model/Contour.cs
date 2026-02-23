using Autodesk.Revit.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Void_Profile_Editor.Model
{
    public class Contour : IEnumerable<KeyValuePair<ContourSideName, Line>>
    {
        public Line Bottom { get; set; }
        public Line Left { get; set; }
        public Line Right { get; set; }
        public Line TopLeft { get; set; }
        public Line TopRight { get; set; }
        public XYZ Center { get; set; }

        public Line GetLine(ContourSideName side)
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
            }
            ;
        }

        public IEnumerator<KeyValuePair<ContourSideName, Line>> GetEnumerator()
        {
            yield return new KeyValuePair<ContourSideName, Line>(ContourSideName.TopLeft, TopLeft);
            yield return new KeyValuePair<ContourSideName, Line>(ContourSideName.Left, Left);
            yield return new KeyValuePair<ContourSideName, Line>(ContourSideName.Bottom, Bottom);
            yield return new KeyValuePair<ContourSideName, Line>(ContourSideName.Right, Right);
            yield return new KeyValuePair<ContourSideName, Line>(ContourSideName.TopRight, TopRight);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
