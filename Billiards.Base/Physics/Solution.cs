namespace Billiard.Physics;

public class Solution
{
    public CollisionCollection Collisions { get; }

    public Solution(CollisionCollection collisions)
    {
        Collisions = collisions;
    }
}