using Utilities;

namespace Physics
{
    namespace Collisions
    {
        class CollisionEvent
        {
            public CollisionEvent(Vector2D _position, double _force, int _surface)
            {
                position = _position;
                force = _force;
                surface = _surface;
            }

            public Vector2D position;
            public double force;
            public int surface;
        }
    }
}
