using System.Numerics;

namespace Billiards.Base.Physics
{
    struct Trajectory
    {
        public Trajectory(Vector2 origin, Vector2 hit, Vector2 normal)
        {
            Origin = origin;
            Hit = hit;
            Normal = normal;
        }

        public Vector2 Origin { get; }
        public Vector2 Hit { get; }
        public Vector2 Normal { get; }
    }
}
