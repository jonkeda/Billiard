using System;
using Utilities;
namespace Physics
{
    namespace Colliders
    {
        class IsoscelesTriangleCollider : Collider
        {
            public Vector2D size;
            public IsoscelesTriangleCollider(Vector2D _center, Vector2D _size, ColliderMode _mode = ColliderMode.Union, double _k = 0.0) : base(_mode, _center, _k)
            {
                size = _size;
            }

            public override double MinDistance(Vector2D p)
            {
                p -= center;
                p.x = Math.Abs(p.x);

                Vector2D a = p - size * MathV.Clamp(MathV.Dot(p, size) / MathV.Dot(size, size), 0, 1);
                Vector2D b = p - size * new Vector2D(MathV.Clamp(p.x / size.x, 0, 1), 1);

                double s = -Math.Sign(size.y);

                Vector2D d = MathV.Min(
                    new Vector2D(MathV.Dot(a, a), s * (p.x * size.y - p.y * size.x)),
                    new Vector2D(MathV.Dot(b, b), s * (p.y - size.y))
                );

                return -Math.Sqrt(d.x) * Math.Sign(d.y);
            }
        }
    }
}
