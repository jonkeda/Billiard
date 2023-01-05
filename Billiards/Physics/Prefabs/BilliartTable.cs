using System.Numerics;
using Billiard.Physics.Colliders;

namespace Billiard.Physics.Prefabs;

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