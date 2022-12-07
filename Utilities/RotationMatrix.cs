using System.Windows.Media.Media3D;
using static System.Math;

namespace Utilities
{
    class RotationMatrix
    {
        private Double3x3 rotation;

        public RotationMatrix()
        {
            rotation = new Double3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);
        }

        public Point3D Column0 { get { return new Point3D(rotation.Matrix[0, 0], rotation.Matrix[1, 0], rotation.Matrix[2, 0]); } }
        public Point3D Column1 { get { return new Point3D(rotation.Matrix[0, 1], rotation.Matrix[1, 1], rotation.Matrix[2, 1]); } }
        public Point3D Column2 { get { return new Point3D(rotation.Matrix[0, 2], rotation.Matrix[1, 2], rotation.Matrix[2, 2]); } }

        public void RotateAroundAxisWithAngle(Point3D axis, double angle)
        {
            double sin = Sin(angle);
            double cos = Cos(angle);
            double ncos = 1 - cos;

            Double3x3 newRotation = new Double3x3(
                axis.X * axis.X * ncos + cos,
                axis.X * axis.Y * ncos - axis.Z * sin,
                axis.X * axis.Z * ncos + axis.Y * sin,
                axis.Y * axis.X * ncos + axis.Z * sin,
                axis.Y * axis.Y * ncos + cos,
                axis.Y * axis.Z * ncos - axis.X * sin,
                axis.Z * axis.X * ncos - axis.Y * sin,
                axis.Z * axis.Y * ncos + axis.X * sin,
                axis.Z * axis.Z * ncos + cos
            );

            rotation *= newRotation;
        }
    }
}
