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
            Double3x3 newRotation = new Double3x3(
                axis.X * axis.X * (1 - Cos(angle)) + Cos(angle),
                axis.X * axis.Y * (1 - Cos(angle)) - axis.Z * Sin(angle),
                axis.X * axis.Z * (1 - Cos(angle)) + axis.Y * Sin(angle),
                axis.Y * axis.X * (1 - Cos(angle)) + axis.Z * Sin(angle),
                axis.Y * axis.Y * (1 - Cos(angle)) + Cos(angle),
                axis.Y * axis.Z * (1 - Cos(angle)) - axis.X * Sin(angle),
                axis.Z * axis.X * (1 - Cos(angle)) - axis.Y * Sin(angle),
                axis.Z * axis.Y * (1 - Cos(angle)) + axis.X * Sin(angle),
                axis.Z * axis.Z * (1 - Cos(angle)) + Cos(angle)
            );

            rotation *= newRotation;
        }
    }
}
