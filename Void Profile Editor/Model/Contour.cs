using Autodesk.Revit.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Void_Profile_Editor.Model
{
    public class Contour : IEnumerable<KeyValuePair<string, Line>>
    {
        public Line Bottom { get; set; }
        public Line Left { get; set; }
        public Line Right { get; set; }

        public IEnumerator<KeyValuePair<string, Line>> GetEnumerator()
        {
            yield return new KeyValuePair<string, Line>("Bottom", Bottom);
            yield return new KeyValuePair<string, Line>("Left", Left);
            yield return new KeyValuePair<string, Line>("Right", Right);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
