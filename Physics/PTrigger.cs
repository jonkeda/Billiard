using Physics.Colliders;
using Utilities;

namespace Physics.Triggers
{
    class PTrigger : PStaticObject
    {
        public PTrigger()
        {
            LodGroup Lod0 = new LodGroup();

            Lod0.AddCollider(new BoxCollider(new Vector2D(485, 547 / 2), new Vector2D(424, 212), ColliderMode.Negate));

            LodGroup Lod1 = new LodGroup();

            Lod1.AddCollider(new CircleCollider(new Vector2D(44, 44), 17));
            Lod1.AddCollider(new CircleCollider(new Vector2D(44, 547 - 44), 17));

            Lod1.AddCollider(new CircleCollider(new Vector2D(970 / 2, 44), 16));
            Lod1.AddCollider(new CircleCollider(new Vector2D(970 / 2, 547 - 44), 17));

            Lod1.AddCollider(new CircleCollider(new Vector2D(970 - 44, 44), 16));
            Lod1.AddCollider(new CircleCollider(new Vector2D(970 - 44, 547 - 44), 17));

            AddLodGroup(Lod0);
            AddLodGroup(Lod1);
        }
        public bool CheckTrigger(PBall ball)
        {
            if (MinDistance(ball.position, 0) < 0) return true;
            return false;
        }
    }
}
