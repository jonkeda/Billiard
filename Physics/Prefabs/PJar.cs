using System.Numerics;
using Billiard.Physics.Colliders.Colliders;

namespace Billiard.Physics.Prefabs
{
    namespace Prefabs
    {
        class PJar : PStaticObject
        {
            public PJar()
            {
                LodGroup Lod0 = new LodGroup();

                Lod0.AddCollider(new StadiumCollider(new Vector2(100, 50), new Vector2(0, 445), 23, ColliderMode.Negate, 20));

                AddLodGroup(Lod0);
            }
        }
    }
}