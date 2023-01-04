using System.Collections.ObjectModel;

namespace Billiards.Base.Physics;

public class CollisionCollection : Collection<Collision>
{
    public bool TwoDifferentBallsHit()
    {
        int? index = null;

        foreach (Collision collision in this)
        {
            if (collision.Ball != null)
            {
                if (!index.HasValue)
                {
                    index = collision.Ball.index;
                }
                else if (index.Value != collision.Ball.index)
                {
                    return true;
                }
            }
        }

        return false;
    }
}