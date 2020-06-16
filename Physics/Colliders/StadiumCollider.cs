using Utilities;
namespace Physics
{
    namespace Colliders
    {
        class StadiumCollider : Collider
        {
            public Vector2D b;
            public double r;
            public StadiumCollider(Vector2D _center, Vector2D _b, double _r, ColliderMode _mode = ColliderMode.Union, double _k = 0.0f) : base(_mode, _center, _k)
            {
                center = _center;
                b = _b;
                r = _r;
            }
            public override double MinDistance(Vector2D p)
            {
                p = (p - center);
                double h = MathV.Clamp(MathV.Dot(p, b) / MathV.Dot(b, b), 0.0f, 1.0f);
                return (p - b * h).Length - r;
            }
        }
    }
}
