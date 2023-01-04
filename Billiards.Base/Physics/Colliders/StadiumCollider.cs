using System.Numerics;
using Billiards.Base.Utilities;

namespace Billiards.Base.Physics.Colliders
{

    public class StadiumCollider : Collider
    {
        public Vector2 b;
        public float r;
        public StadiumCollider(Vector2 _center, Vector2 _b, float _r, ColliderMode _mode = ColliderMode.Union, float _k = 0.0f) : base(_mode, _center, _k)
        {
            center = _center;
            b = _b;
            r = _r;
        }
        public override float MinDistance(Vector2 p)
        {
            p = (p - center);
            float h = MathV.Clamp(Vector2.Dot(p, b) / Vector2.Dot(b, b), 0.0f, 1.0f);
            return (p - b * h).Length() - r;
        }
    }

}
