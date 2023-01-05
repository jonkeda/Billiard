namespace Billiards.Web.Shared;

public class Collision
{
    public Collision(Point position, BallColor? ball, CollisionType collisionType)
    {
        CollisionType = collisionType;
        Position = position;
        Ball = ball;
    }

    public Point Position { get; }
    public BallColor? Ball { get; }
    public CollisionType CollisionType { get; }
}