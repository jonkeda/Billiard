using System;
using System.Windows.Media.Imaging;
using Utilities;

namespace Physics
{
    class PBall : PDynamicObject
    {
        public double r;
        public BitmapImage texture;
        public int index;
        public Vector2D phi = new Vector2D(0, 0);
        public RotationMatrix rotation = new RotationMatrix();

        private readonly double cr, u0;
        private readonly Vector2D rest_velocity = new Vector2D(0.7);
        private readonly double g = 9.81;
        public PBall(int _index, double _r, Vector2D _position = null, Vector2D _velocity = null, double _u0 = 0.2, double _cr = 0.02) : base(_position, _velocity)
        {
            u0 = _u0;
            cr = _cr;
            r = _r;
            index = _index;

            texture = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/" + _index + ".jpg"));
        }

        public void Deconstruct(out Vector2D _position, out double _r)
        {
            _position = position;
            _r = r;
        }

        public override void Simulate(double dt)
        {
            if (MathV.Abs(velocity) < rest_velocity)
            {
                Resting = true;
                return;
            }

            Resting = false;

            Vector2D normalizedVelocity = velocity.Normalize();

            // Leapfrog integration scheme
            position += velocity * dt + normalizedVelocity * 5 / 2 * r * g * (-cr - u0) * dt * dt / 2;
            velocity += dt * (normalizedVelocity * 5 / 2 * r * g * (-cr - u0));

            //Calculate new rotation matrix
            Vector2D tangentVelocity = new Vector2D(-velocity.y, velocity.x).Normalize();

            phi = velocity * dt + normalizedVelocity * 5 / 2 * r * g * (-cr - u0) * dt * dt / 2 / r;

            rotation.RotateAroundAxisWithAngle(tangentVelocity, phi.Length / r);
        }
    }
}
