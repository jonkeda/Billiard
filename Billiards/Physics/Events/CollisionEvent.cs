using System.Numerics;

namespace Billiard.Physics.Events
{
    namespace Collisions
    {
        class CollisionEvent
        {
            public CollisionEvent(Vector2 _position, float _force, int _surface)
            {
                position = _position;
                force = _force;
                surface = _surface;
            }

            public Vector2 position;
            public float force;
            public int surface;
        }
    }
}
