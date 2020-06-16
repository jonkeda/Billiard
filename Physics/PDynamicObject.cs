using Utilities;
namespace Physics
{
    abstract class PDynamicObject
    {
        public Vector2D position;
        public Vector2D velocity;
        public bool Resting { get; set; } = false;

        public PDynamicObject(Vector2D _position, Vector2D _velocity)
        {
            position = _position ?? new Vector2D(0, 0);
            velocity = _velocity ?? new Vector2D(0, 0);
        }

        public abstract void Simulate(double dt);
    }
}
