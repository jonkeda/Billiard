using System.Numerics;
using Billiards.Base.Physics.Colliders;

namespace Billiards.Base.Physics.Prefabs;

public class PoolTable : PTable
{
    public PoolTable()
    {
        // 970 424 848 132 
        // 547 212 424 123 

        LodGroup Lod0 = new LodGroup();

        Lod0.AddCollider(new BoxCollider(new Vector2(Length / 2, Width / 2),
            new Vector2((Length - 132) / 2, (Width - 123) / 2), ColliderMode.Negate));
        AddLodGroup(Lod0);

        LodGroup Lod1 = new LodGroup();

        Lod1.AddCollider(new BoxCollider(new Vector2(Length / 2, Width / 2),
            new Vector2((Length - 132) / 2, (Width - 123) / 2), ColliderMode.Negate));

        Lod1.AddCollider(new StadiumCollider(new Vector2(44, 44), new Vector2(80, 80), 17,
            ColliderMode.Subtract));
        Lod1.AddCollider(new StadiumCollider(new Vector2(44, Width - 44), new Vector2(80, -80), 17,
            ColliderMode.Subtract));

        Lod1.AddCollider(new StadiumCollider(new Vector2(Length / 2, 44), new Vector2(0, 80), 17,
            ColliderMode.Subtract));
        Lod1.AddCollider(new IsoscelesTriangleCollider(new Vector2(Length / 2, 33), new Vector2(70, 70),
            ColliderMode.Subtract));

        Lod1.AddCollider(new StadiumCollider(new Vector2(Length / 2, Width - 44), new Vector2(0, -80), 17,
            ColliderMode.Subtract));
        Lod1.AddCollider(new IsoscelesTriangleCollider(new Vector2(Length / 2, Width - 33),
            new Vector2(70, -70),
            ColliderMode.Subtract));

        Lod1.AddCollider(new StadiumCollider(new Vector2(Length - 44, 44), new Vector2(-80, 80), 17,
            ColliderMode.Subtract));
        Lod1.AddCollider(new StadiumCollider(new Vector2(Length - 44, Width - 44), new Vector2(-80, -80), 17,
            ColliderMode.Subtract));
        AddLodGroup(Lod1);
    }
}