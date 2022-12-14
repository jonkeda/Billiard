using System.Numerics;
using Billiard.Physics.Colliders;

namespace Billiard.Physics.Prefabs
{
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

    public class BilliartTable : PTable
    {
        public float top;
        public float bottom;
        public float left;
        public float right;


        public BilliartTable()
        {
            // 970 424 848 132 
            // 547 212 424 123 

            LodGroup Lod0 = new LodGroup();

            Lod0.AddCollider(new BoxCollider(new Vector2(Length / 2, Width / 2),
                new Vector2(Length / 2 - BallRadius, Width / 2 - BallRadius), ColliderMode.Negate));
            AddLodGroup(Lod0);

            top = BallRadius * 2;
            left = BallRadius * 2;
            right = Length - BallRadius * 2;
            bottom = Width - BallRadius * 2;
        }

        public override bool Collides(Vector2 p, float r = 0, bool ignoreLodGroups = false)
        {
            float x = p.X;
            float y = p.Y;
            return x < left || x > right || y < top || y > bottom;
//            return MinDistance(p, r, true) < r;
        }

    }

    public class PTable : PStaticObject
    {
        public float Length { get; } = 2100 * 0.9f; // 1800; // 1200;
        public float Width { get; } = 1050 * 0.9f; // 900;
        public float BallRadius { get; } = BallRadiusConst;

        public const float BallRadiusConst = 30 * 0.9f;
    }
}
