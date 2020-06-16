using Utilities;
namespace Physics
{
    struct Trajectory
    {
        public Trajectory(Vector2D origin, Vector2D hit, Vector2D normal)
        {
            Origin = origin;
            Hit = hit;
            Normal = normal;
        }

        public Vector2D Origin { get; }
        public Vector2D Hit { get; }
        public Vector2D Normal { get; }
    }
}
