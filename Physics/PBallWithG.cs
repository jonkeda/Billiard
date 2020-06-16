using Utilities;
namespace Physics
{
    class PBallWithG : PBall
    {
        private readonly Vector2D g = new Vector2D(0, 98.1 * 4);

        public PBallWithG(int _index, double _r, Vector2D _position, Vector2D _velocity = null) : base(_index, _r, _position, _velocity) { }

        public override void Simulate(double dt)
        {
            position += velocity * dt + g * dt * dt / 2;
            velocity += dt * g;
        }
    }
}