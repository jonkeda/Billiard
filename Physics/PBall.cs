using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Utilities;

namespace Physics
{
    class CollisionCollection : Collection<Collision>
    {

    }
    class Collision
    {
        public Collision(Vector2D position, PBall ball)
        {
            Position = position;
            Ball = ball;
        }

        public Vector2D Position { get; }
        public PBall Ball { get; }
    }

    class PBall : PDynamicObject
    {
        public Vector2DCollection Positions { get; } = new();
        public CollisionCollection Collisions { get; } = new();
        private Vector2D lastPosition = new Vector2D();
        private Vector2D lastCollision = new Vector2D();

        public Brush Color { get; }
        public Pen Pen { get; }
        public bool DrawTrajectory { get; set; }

        public double r;
        public BitmapImage texture;
        public int index;
        public Vector2D phi = new Vector2D(0, 0);
        public RotationMatrix rotation = new RotationMatrix();

        private readonly double cr, u0;
        private readonly Vector2D rest_velocity = new Vector2D(0.7);
        private readonly double g = 9.81;

        public PBall(int _index, double _r, Vector2D _position = null, Vector2D _velocity = null, double _u0 = 0.2, double _cr = 0.02, Brush color = null) : base(_position, _velocity)
        {
            u0 = _u0;
            cr = _cr;
            r = _r;
            index = _index;

            texture = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/" + _index + ".jpg"));
            Color = color;

            Pen = new Pen(Color, 2)
            {
                DashStyle = DashStyles.Dash
            };

        }

        public PBall(PBall clone) : base(clone.position, clone.velocity)
        {
            u0 = clone.u0;
            cr = clone.cr;
            r = clone.r;
            index = clone.index;
            texture = clone.texture;
            Color = clone.Color;
            Pen = clone.Pen;
            Positions = clone.Positions;
            Collisions = clone.Collisions;
            DrawTrajectory = clone.DrawTrajectory;
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
            position += velocity * dt + normalizedVelocity * 5.0 / 2.0 * r * g * (-cr - u0) * dt * dt / 2.0;
            velocity += dt * (normalizedVelocity * 5.0 / 2.0 * r * g * (-cr - u0));

            //Calculate new rotation matrix
            Vector2D tangentVelocity = new Vector2D(-velocity.y, velocity.x).Normalize();

            phi = velocity * dt + normalizedVelocity * 5.0 / 2.0 * r * g * (-cr - u0) * dt * dt / 2.0 / r;

            rotation.RotateAroundAxisWithAngle(tangentVelocity, phi.Length / r);
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

        public void SaveCollision(PBall pBall)
        {
            if (!DrawTrajectory)
            {
                return;
            }
            if (!position.Equals(lastCollision))
            {
                Collisions.Add(new Collision(position, pBall));
                lastCollision = position;
            }
        }

        public void ClearPositions()
        {
            Collisions.Clear();
            Positions.Clear();
            geometry = null;
        }

        private Geometry geometry;

        public Geometry AsGeometry()
        {
            if (geometry == null)
            {
                geometry = Positions.AsGeometry();
            }
           
            return geometry;
        }
    }
}
