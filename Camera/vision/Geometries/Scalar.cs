using Emgu.CV.Structure;

namespace Billiard.Camera.vision.Geometries;

public class Scalar 
{
    public float x;
    public float y;
    public float z;

    public Scalar(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public MCvScalar AsMCvScalar()
    {
        return new MCvScalar(x, y, z);
    }
}