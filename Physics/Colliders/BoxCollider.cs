using System;
using System.Numerics;
using Billiard.Utilities;

namespace Billiard.Physics.Colliders
{
    namespace Colliders
    {
        class BoxCollider : Collider
        {
            public Vector2 b;
            public BoxCollider(Vector2 _center, Vector2 _b, ColliderMode _mode = ColliderMode.Union, float _k = 0.0f) : base(_mode, _center, _k)
            {
                center = _center;
                b = _b;
            }
            public override float MinDistance(Vector2 p)
            {
                Vector2 d = Vector2.Abs(p - center) - b;
                return MathV.Max(d, 0.0f).Length() + Math.Min(Math.Max(d.X, d.Y), 0.0f);
            }
        }
    }
}
