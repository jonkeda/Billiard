using System;
using System.Collections.ObjectModel;

using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Utilities;
using Brush = System.Windows.Media.Brush;
using Pen = System.Windows.Media.Pen;



namespace Physics
{
    enum CollisionType
    {
        Start,
        End,
        Ball,
        Cushion
    }

    class CollisionCollection : Collection<Collision>
    {
        public bool TwoDifferentBallsHit()
        {
            int? index = null;

            foreach (Collision collision in this)
            {
                if (collision.Ball != null)
                {
                    if (!index.HasValue)
                    {
                        index = collision.Ball.index;
                    }
                    else if (index.Value != collision.Ball.index)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public Geometry AsGeometry()
        {
            if (this.Count <= 2)
            {
                return null;
            }

            StreamGeometry geometry = new StreamGeometry();

            using var ctx = geometry.Open();

            Collision s = this[0];
            ctx.BeginFigure(new Point(s.Position.x, s.Position.y), false, false);
            foreach (Collision v in this.Skip(1))
            {
                ctx.LineTo(new Point(v.Position.x, v.Position.y), true, false);
            }

            return geometry;
        }

    }

    class Collision
    {
        public Collision(Vector2D position, PBall ball, CollisionType collisionType)
        {
            this.collisionType = collisionType;
            Position = position;
            Ball = ball;
        }

        public Vector2D Position { get; }
        public PBall Ball { get; }
        private readonly CollisionType collisionType;
    }

    class PBall 
    {
        public Vector2D position;
        public Vector2D velocity;
        public bool Resting { get; set; } = false;

        public PBall(Vector2D _position, Vector2D _velocity)
        {
            position = _position;
            velocity = _velocity;
        }


        public Vector2DCollection Positions { get; } = new();
        public CollisionCollection Collisions { get; private set; } = new();
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
        private static readonly Vector2D rest_velocity = new Vector2D(0.7);
        private const double g = 9.81;

        public PBall(int _index, double _r, Vector2D _position, Vector2D _velocity, double _u0 = 0.2, double _cr = 0.02, Brush color = null) : this(_position, _velocity)
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

        public PBall(PBall clone) : this(clone.position, clone.velocity)
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

        public void Simulate(double dt, bool animate)
        {
            if (MathV.Abs(velocity) < rest_velocity)
            {
                Resting = true;
                return;
            }

            Resting = false;

            Vector2D normalizedVelocity = velocity.Normalize();

            Vector2D lag = normalizedVelocity * (5.0 / 2.0 * r * g * (-cr - u0));

            // Leapfrog integration scheme
            position += velocity * dt + lag * (dt * dt / 2.0);
            velocity += dt * lag;

            if (animate)
            {
            //Calculate new rotation matrix
                Vector2D tangentVelocity = new Vector2D(-velocity.y, velocity.x).Normalize();

                phi = velocity * dt + lag * dt * dt / 2.0 / r;

                rotation.RotateAroundAxisWithAngle(tangentVelocity, phi.Length / r);
            }
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

        public void SaveCollision(PBall pBall, CollisionType collisionType)
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
            geometry = null;
            positionsGeometry = null;
            Collisions.Add(new Collision(position, null, CollisionType.Start));
        }

        private Geometry geometry;

        public Geometry AsGeometry()
        {
            if (geometry == null)
            {
                geometry = Collisions.AsGeometry();
                //geometry = Positions.AsGeometry();
            }
           
            return geometry;
        }

        private Geometry positionsGeometry;
        public Geometry PositionsAsGeometry()
        {
            if (positionsGeometry == null)
            {
                positionsGeometry = Positions.AsGeometry();
            }

            return positionsGeometry;
        }

    }
}
