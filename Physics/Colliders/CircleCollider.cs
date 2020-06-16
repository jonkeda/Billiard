using Utilities;
namespace Physics
{
    namespace Colliders
    {
        class CircleCollider : Collider
        {
            public double r;
            public CircleCollider(Vector2D _center, double _r, ColliderMode _mode = ColliderMode.Union, double _k = 0.0f) : base(_mode, _center, _k)
            {
                center = _center;
                r = _r;
            }

            public override double MinDistance(Vector2D p)
            {
                return (p - center).Length - r;
            }
        }
    }
}
