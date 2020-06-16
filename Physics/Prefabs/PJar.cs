using Physics.Colliders;
using Utilities;

namespace Physics
{
    namespace Prefabs
    {
        class PJar : PStaticObject
        {
            public PJar()
            {
                LodGroup Lod0 = new LodGroup();

                Lod0.AddCollider(new StadiumCollider(new Vector2D(100, 50), new Vector2D(0, 445), 23, ColliderMode.Negate, 20));

                AddLodGroup(Lod0);
            }
        }
    }
}