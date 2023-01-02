using System.Numerics;
using Billiard.Utilities;
using Billiards.Base.Extensions;
using Billiards.Base.Filters;


namespace Billiard.Physics
{
    public class PBall 
    {
        public BallColor BallColor { get; }
        public Vector2 position;
        public Vector2 velocity;

        public PBall(Vector2 _position, Vector2 _velocity)
        {
            position = _position;
            velocity = _velocity;
        }


        public Vector2Collection Positions { get; } = new();
        public CollisionCollection Collisions { get; private set; } = new();
        private Vector2 lastPosition = new Vector2();
        private Vector2 lastCollision = new Vector2();

        public bool DrawTrajectory { get; set; }

        public float r;
        public int index;
        public Vector2 phi = new Vector2(0, 0);

        private readonly float cr, u0;
        private static readonly Vector2 rest_velocity = new Vector2(0.7f);
        private const float g = 9.81f;

        public PBall(int _index, float _r, Vector2 _position, Vector2 _velocity, BallColor ballColor, float _u0 = 0.2f, float _cr = 0.02f) 
            : this(_position, _velocity)
        {
            BallColor = ballColor;
            u0 = _u0;
            cr = _cr;
            r = _r;
            index = _index;
        }

        public PBall(PBall clone) : this(clone.position, clone.velocity)
        {
            u0 = clone.u0;
            cr = clone.cr;
            r = clone.r;
            index = clone.index;
            Positions = clone.Positions;
            Collisions = clone.Collisions;
            DrawTrajectory = clone.DrawTrajectory;
        }

        public void Deconstruct(out Vector2 _position, out float _r)
        {
            _position = position;
            _r = r;
        }

        public bool Simulate(float dt)
        {
            if (velocity.X == float.NaN
                || velocity.Y == float.NaN
                || Vector2.Abs(velocity).SmallerThen(rest_velocity))
            {
                return true;
            }

            Vector2 normalizedVelocity = Vector2.Normalize(velocity);

            Vector2 lag = normalizedVelocity * (5.0f / 2.0f * r * g * (-cr - u0));

            // Leapfrog integration scheme
            position += velocity * dt + lag * (dt * dt / 2.0f);
            velocity += dt * lag;

            return false;
        }

        public void SavePosition()
        {
            if (!DrawTrajectory)
            {
                return;
            }
            if (!position.Equals(lastPosition))
            {
                Positions.Add(position);
                lastPosition = position;
            }
        }

        public void SaveCollision(PBall? pBall, CollisionType collisionType)
        {
            if (!position.Equals(lastCollision))
            {
                Collisions.Add(new Collision(position, pBall, collisionType));
                lastCollision = position;
            }
        }

        public void ClearPositions()
        {
            Collisions = new ();
            Positions.Clear();
            Collisions.Add(new Collision(position, null, CollisionType.Start));
        }
        

    }
}
