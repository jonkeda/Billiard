using System.Numerics;

namespace Billiards.Base.Physics;

public class Collision
{
    public Collision(Vector2 position, PBall? ball, CollisionType collisionType)
    {
        CollisionType = collisionType;
        Position = position;
        Ball = ball;
    }

    public Vector2 Position { get; }
    public PBall? Ball { get; }
    public CollisionType CollisionType { get; }
}