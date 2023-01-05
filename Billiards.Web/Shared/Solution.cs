using System.Text;

namespace Billiards.Web.Shared;

public class Solution
{
    public CollisionCollection Collisions { get; }

    public Solution(CollisionCollection collisions)
    {
        Collisions = collisions;
    }

    public string PointsAsString()
    {
        StringBuilder points = new StringBuilder();
        foreach (Collision collision in Collisions)
        {
            points.Append(collision.Position);
            points.Append(" ");
        }

        return points.ToString();
    }
}