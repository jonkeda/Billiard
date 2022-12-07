using Physics.Colliders;
using Utilities;

namespace Physics.Prefabs
{
    class PoolTable : PTable
    {
        public PoolTable()
        {
            // 970 424 848 132 
            // 547 212 424 123 

            LodGroup Lod0 = new LodGroup();

            Lod0.AddCollider(new BoxCollider(new Vector2D(Length / 2, Width / 2),
                new Vector2D((Length - 132) / 2, (Width - 123) / 2), ColliderMode.Negate));
            AddLodGroup(Lod0);

            LodGroup Lod1 = new LodGroup();

            Lod1.AddCollider(new BoxCollider(new Vector2D(Length / 2, Width / 2),
                new Vector2D((Length - 132) / 2, (Width - 123) / 2), ColliderMode.Negate));

            Lod1.AddCollider(new StadiumCollider(new Vector2D(44, 44), new Vector2D(80, 80), 17,
                ColliderMode.Subtract));
            Lod1.AddCollider(new StadiumCollider(new Vector2D(44, Width - 44), new Vector2D(80, -80), 17,
                ColliderMode.Subtract));

            Lod1.AddCollider(new StadiumCollider(new Vector2D(Length / 2, 44), new Vector2D(0, 80), 17,
                ColliderMode.Subtract));
            Lod1.AddCollider(new IsoscelesTriangleCollider(new Vector2D(Length / 2, 33), new Vector2D(70, 70),
                ColliderMode.Subtract));

            Lod1.AddCollider(new StadiumCollider(new Vector2D(Length / 2, Width - 44), new Vector2D(0, -80), 17,
                ColliderMode.Subtract));
            Lod1.AddCollider(new IsoscelesTriangleCollider(new Vector2D(Length / 2, Width - 33),
                new Vector2D(70, -70),
                ColliderMode.Subtract));

            Lod1.AddCollider(new StadiumCollider(new Vector2D(Length - 44, 44), new Vector2D(-80, 80), 17,
                ColliderMode.Subtract));
            Lod1.AddCollider(new StadiumCollider(new Vector2D(Length - 44, Width - 44), new Vector2D(-80, -80), 17,
                ColliderMode.Subtract));
            AddLodGroup(Lod1);
        }
    }

    class BilliartTable : PTable
    {
        public double top;
        public double bottom;
        public double left;
        public double right;


        public BilliartTable()
        {
            // 970 424 848 132 
            // 547 212 424 123 

            LodGroup Lod0 = new LodGroup();

            Lod0.AddCollider(new BoxCollider(new Vector2D(Length / 2, Width / 2),
                new Vector2D(Length / 2 - BallRadius, Width / 2 - BallRadius), ColliderMode.Negate));
            AddLodGroup(Lod0);

            top = BallRadius * 2;
            left = BallRadius * 2;
            right = Length - BallRadius * 2;
            bottom = Width - BallRadius * 2;
        }

        public override bool Collides(Vector2D p, double r = 0, bool ignoreLodGroups = false)
        {
            double x = p.x;
            double y = p.y;
            return x < left || x > right || y < top || y > bottom;
//            return MinDistance(p, r, true) < r;
        }

    }

    class PTable : PStaticObject
    {
        public double Length { get; } = 1800; // 1200;
        public double Width { get; } = 1000;
        public double BallRadius { get; } = 16;
    }
}
