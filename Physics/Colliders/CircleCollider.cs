using System.Numerics;

namespace Billiard.Physics.Colliders
{

    public class CircleCollider : Collider
    {
        public float r;
        public CircleCollider(Vector2 _center, float _r, ColliderMode _mode = ColliderMode.Union, float _k = 0.0f) : base(_mode, _center, _k)
        {
            center = _center;
            r = _r;
        }

        public override float MinDistance(Vector2 p)
        {
            return (p - center).Length() - r;
        }
    }

}
