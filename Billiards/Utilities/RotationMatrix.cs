using System.Windows.Media.Media3D;
using static System.Math;

namespace Billiard.Utilities
{
    public class RotationMatrix
    {
        private float3x3 rotation;

        public RotationMatrix()
        {
            rotation = new float3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);
        }

        public Point3D Column0 { get { return new Point3D(rotation.Matrix[0, 0], rotation.Matrix[1, 0], rotation.Matrix[2, 0]); } }
        public Point3D Column1 { get { return new Point3D(rotation.Matrix[0, 1], rotation.Matrix[1, 1], rotation.Matrix[2, 1]); } }
        public Point3D Column2 { get { return new Point3D(rotation.Matrix[0, 2], rotation.Matrix[1, 2], rotation.Matrix[2, 2]); } }

        public void RotateAroundAxisWithAngle(Point3D axis, float angle)
        {
            float x = (float)axis.X;
            float y = (float)axis.Y;
            float z = (float)axis.Z;

            float sin = (float)Sin(angle);
            float cos = (float)Cos(angle);
            float ncos = 1 - cos;

            float3x3 newRotation = new float3x3(
                x * x * ncos + cos,
                x * y * ncos - z * sin,
                x * z * ncos + y * sin,
                y * x * ncos + z * sin,
                y * y * ncos + cos,
                y * z * ncos - x * sin,
                z * x * ncos - y * sin,
                z * y * ncos + x * sin,
                z * z * ncos + cos
            );

            rotation *= newRotation;
        }
    }
}
