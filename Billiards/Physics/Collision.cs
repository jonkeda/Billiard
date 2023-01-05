using System.Numerics;

namespace Billiard.Physics;

public class Collision
{
    public Collision(Vector2 position, PBall ball, CollisionType collisionType)
    {
        this.collisionType = collisionType;
        Position = position;
        Ball = ball;
    }

    public Vector2 Position { get; }
    public PBall Ball { get; }
    private readonly CollisionType collisionType;
}