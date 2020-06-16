using Utilities;
namespace Physics
{
    namespace Colliders
    {
        abstract class Collider
        {
            public ColliderMode mode;
            public double k;
            public Vector2D center;
            public Collider(ColliderMode _mode, Vector2D _center, double _k = 0.0f)
            {
                mode = _mode;
                center = _center;
                k = _k;
            }
            public abstract double MinDistance(Vector2D p);
        }
    }
}
