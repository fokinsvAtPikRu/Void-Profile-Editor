namespace Void_Profile_Editor.Domain.Model.Geometry
{
    public class IntersectionPoint
    {
        public Point3DDomain Point { get; set; }
        public ContourSideName SideName { get; set; }
        public IntersectionPoint(Point3DDomain point, ContourSideName sideName)
        {
            Point = point;
            SideName = sideName;
        }        
    }
}
