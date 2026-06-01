namespace Void_Profile_Editor.Domain.Model.Geometry
{
    public class PressureContour
    {
        public string Id { get; set; }       
        public Point3DDomain InsertPoint { get; set; }
        public double Rotation { get; set; }        
        public bool IsMirrored { get; set; }
        public PressureContourParameters ContourParameters { get; set; }      
        

        public static PressureContour FromRevitData(
            string id,
            Point3DDomain insertPoint,
            double rotation,
            bool isMirrored,
            PressureContourParameters parameters)
        {
            return new PressureContour
            {
                Id = id,
                InsertPoint = insertPoint,
                Rotation = rotation,
                IsMirrored = isMirrored,
                ContourParameters = parameters
            };
        }
    }
}
