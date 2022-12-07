using Utilities;

namespace Billiard.Physics
{
    class Force
    {
        public double Direction { get; set; }

        public double Power { get; set; } = 150;
        public Vector2D Position { get; set; }

        public Vector2D Vector
        {
            get
            {
                return MathV.GetVector(Direction).Normalize();
            }
        }

        public Vector2D VectorPower
        {
            get
            {
                return MathV.GetVector(Direction).Normalize() * Power;
            }
        }

        public void ClockWise(double speed)
        {
            Direction += speed;
        }

        public void CounterClockWise(double speed)
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
