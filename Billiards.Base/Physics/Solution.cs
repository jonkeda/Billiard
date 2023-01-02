namespace Billiard.Physics;

public class Solution
{
    public CollisionCollection Collections { get; }

    public Solution(CollisionCollection collections)
    {
        Collections = collections;
    }
}