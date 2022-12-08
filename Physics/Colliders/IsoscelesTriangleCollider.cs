using System;
using System.Numerics;
using Utilities;
namespace Physics
{
    namespace Colliders
    {
        class IsoscelesTriangleCollider : Collider
        {
            public Vector2 size;
            public IsoscelesTriangleCollider(Vector2 _center, Vector2 _size, ColliderMode _mode = ColliderMode.Union, float _k = 0.0f) : base(_mode, _center, _k)
            {
                size = _size;
            }

            public override float MinDistance(Vector2 ip)
            {
                ip -= center;
                Vector2 p = new Vector2(Math.Abs(ip.X), ip.Y);

                Vector2 a = p - size * MathV.Clamp(Vector2.Dot(p, size) / Vector2.Dot(size, size), 0, 1);
                Vector2 b = p - size * new Vector2(MathV.Clamp(p.X / size.X, 0, 1), 1);

                float s = -Math.Sign(size.Y);

                Vector2 d = Vector2.Min(
                    new Vector2(Vector2.Dot(a, a), s * (p.X * size.Y - p.Y * size.X)),
                    new Vector2(Vector2.Dot(b, b), s * (p.Y - size.Y))
                );

                return -(float)(Math.Sqrt(d.X) * Math.Sign(d.Y));
            }
        }
    }
}
