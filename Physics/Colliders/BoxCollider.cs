using System;
using Utilities;
namespace Physics
{
    namespace Colliders
    {
        class BoxCollider : Collider
        {
            public Vector2D b;
            public BoxCollider(Vector2D _center, Vector2D _b, ColliderMode _mode = ColliderMode.Union, double _k = 0.0f) : base(_mode, _center, _k)
            {
                center = _center;
                b = _b;
            }
            public override double MinDistance(Vector2D p)
            {
                Vector2D d = MathV.Abs(p - center) - b;
                return MathV.Max(d, 0.0).Length + Math.Min(Math.Max(d.x, d.y), 0.0);
            }
        }
    }
}
