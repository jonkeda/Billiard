using Physics.Colliders;
using Utilities;

namespace Physics.Prefabs
{
    class PTable : PStaticObject
    {
        public PTable()
        {
            LodGroup Lod0 = new LodGroup();

            Lod0.AddCollider(new BoxCollider(new Vector2D(485, 547 / 2), new Vector2D(424, 212), ColliderMode.Negate));

            LodGroup Lod1 = new LodGroup();

            Lod1.AddCollider(new BoxCollider(new Vector2D(485, 547 / 2), new Vector2D(424, 212), ColliderMode.Negate));

            Lod1.AddCollider(new StadiumCollider(new Vector2D(44, 44), new Vector2D(80, 80), 17, ColliderMode.Subtract));
            Lod1.AddCollider(new StadiumCollider(new Vector2D(44, 547 - 44), new Vector2D(80, -80), 17, ColliderMode.Subtract));

            Lod1.AddCollider(new StadiumCollider(new Vector2D(970 / 2, 44), new Vector2D(0, 80), 17, ColliderMode.Subtract));
            Lod1.AddCollider(new IsoscelesTriangleCollider(new Vector2D(970 / 2, 33), new Vector2D(70, 70), ColliderMode.Subtract));

            Lod1.AddCollider(new StadiumCollider(new Vector2D(970 / 2, 547 - 44), new Vector2D(0, -80), 17, ColliderMode.Subtract));
            Lod1.AddCollider(new IsoscelesTriangleCollider(new Vector2D(970 / 2, 547 - 33), new Vector2D(70, -70), ColliderMode.Subtract));

            Lod1.AddCollider(new StadiumCollider(new Vector2D(970 - 44, 44), new Vector2D(-80, 80), 17, ColliderMode.Subtract));
            Lod1.AddCollider(new StadiumCollider(new Vector2D(970 - 44, 547 - 44), new Vector2D(-80, -80), 17, ColliderMode.Subtract));

            AddLodGroup(Lod0);
            AddLodGroup(Lod1);
        }
    }
}
