namespace Void_Profile_Editor.Domain.Model.Geometry
{
    public class DetailLineDomain
    {
        public Point3DDomain Start { get; }
        public Point3DDomain End { get; }

        public DetailLineDomain(Point3DDomain start, Point3DDomain end)
        {
            Start = start;
            End = end;
        }
    }

}
