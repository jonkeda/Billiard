using Physics.Colliders;
using Utilities;

namespace Physics.Prefabs
{
    class PTable : PStaticObject
    {
        public double Length { get; } = 1800; // 1200;
        public double Width { get; } = 1000;


        public PTable(GameType gameType)
        {
            // 970 424 848 132 
            // 547 212 424 123 

            LodGroup Lod0 = new LodGroup();

            Lod0.AddCollider(new BoxCollider(new Vector2D(Length / 2, Width / 2), new Vector2D((Length-132) / 2, (Width -123) / 2), ColliderMode.Negate));

            LodGroup Lod1 = new LodGroup();

            Lod1.AddCollider(new BoxCollider(new Vector2D(Length / 2, Width / 2), new Vector2D((Length - 132) / 2, (Width - 123) / 2), ColliderMode.Negate));

            if (gameType == GameType.Pool)
            {
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
                Lod1.AddCollider(new IsoscelesTriangleCollider(new Vector2D(Length / 2, Width - 33), new Vector2D(70, -70),
                    ColliderMode.Subtract));

                Lod1.AddCollider(new StadiumCollider(new Vector2D(Length - 44, 44), new Vector2D(-80, 80), 17,
                    ColliderMode.Subtract));
                Lod1.AddCollider(new StadiumCollider(new Vector2D(Length - 44, Width - 44), new Vector2D(-80, -80), 17,
                    ColliderMode.Subtract));
            }

            AddLodGroup(Lod0);
            AddLodGroup(Lod1);
        }
    }
}
