using System.Numerics;

namespace Billiard.Physics.Colliders
{

    public abstract class Collider
    {
        public ColliderMode mode;
        public float k;
        public Vector2 center;
        public Collider(ColliderMode _mode, Vector2 _center, float _k = 0.0f)
        {
            mode = _mode;
            center = _center;
            k = _k;
        }
        public abstract float MinDistance(Vector2 p);
    }

}
