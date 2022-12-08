using System.Numerics;
using Physics.Colliders;

namespace Physics.Triggers
{
    class PTrigger : PStaticObject
    {
        public PTrigger(float length, float width)
        {
            LodGroup lod0 = new LodGroup();

            lod0.AddCollider(new BoxCollider(new Vector2(length / 2, width / 2), new Vector2((length - 132) / 2, (width - 123) / 2), ColliderMode.Negate));

            LodGroup lod1 = new LodGroup();

            lod1.AddCollider(new CircleCollider(new Vector2(44, 44), 17));
            lod1.AddCollider(new CircleCollider(new Vector2(44, width - 44), 17));

            lod1.AddCollider(new CircleCollider(new Vector2(970f / 2, 44), 16));
            lod1.AddCollider(new CircleCollider(new Vector2(970f / 2, width - 44), 17));

            lod1.AddCollider(new CircleCollider(new Vector2(970 - 44, 44), 16));
            lod1.AddCollider(new CircleCollider(new Vector2(970 - 44, width - 44), 17));

            AddLodGroup(lod0);
            AddLodGroup(lod1);
        }
        public bool CheckTrigger(PBall ball)
        {
            if (MinDistance(ball.position, 0) < 0) return true;
            return false;
        }
    }
}
