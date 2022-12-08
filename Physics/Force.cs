using System.Numerics;
using Utilities;

namespace Billiard.Physics
{
    class Force
    {
        public float Direction { get; set; }

        public float Power { get; set; } = 150;
        public Vector2 Position { get; set; }

        public Vector2 Vector
        {
            get
            {
                return Vector2.Normalize(MathV.GetVector(Direction));
            }
        }

        public Vector2 VectorPower
        {
            get
            {
                return Vector2.Normalize(MathV.GetVector(Direction)) * Power;
            }
        }

        public void ClockWise(float speed)
        {
            Direction += speed;
        }

        public void CounterClockWise(float speed)
        {
            Direction -= speed;
        }

        public void PowerUp()
        {
            Power += 1;
            if (Power > 200)
            {
                Power = 200;
            }
        }

        public void PowerDown()
        {
            Power -= 1;
            if (Power < 1)
            {
                Power = 1;
            }
        }

    }
}
