using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Void_Profile_Editor.Domain.Model.Geometry
{
    public class Point3DDomain : IEquatable<Point3DDomain>
    {
        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public Point3DDomain(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double DistanceTo(Point3DDomain other)
        {
            var dx=X - other.X;
            var dy=Y - other.Y;
            var dz=Z - other.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
        public bool Equals(Point3DDomain other) =>
            other != null &&
            Math.Abs(X - other.X) < 1e-9 &&
            Math.Abs(Y - other.Y) < 1e-9 &&
            Math.Abs(Z - other.Z) < 1e-9;

        public static Point3DDomain operator +(Point3DDomain a, Point3DDomain b) =>
            new Point3DDomain(a.X+b.X,a.Y+b.Y,a.Z+b.Z);
        public static Point3DDomain operator -(Point3DDomain a, Point3DDomain b) =>
            new Point3DDomain(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Point3DDomain operator /(Point3DDomain a, double k) =>
            new Point3DDomain(a.X/k, a.Y/k, a.Z/k);
    }
}
