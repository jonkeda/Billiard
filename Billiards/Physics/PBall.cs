using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Billiard.Utilities;
using Brush = System.Windows.Media.Brush;
using Pen = System.Windows.Media.Pen;



namespace Billiard.Physics
{
    public enum CollisionType
    {
        Start,
        End,
        Ball,
        Cushion
    }

    public class CollisionCollection : Collection<Collision>
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
            if (Count <= 2)
            {
                return null;
            }

            StreamGeometry geometry = new StreamGeometry();

            using var ctx = geometry.Open();

            Collision s = this[0];
            ctx.BeginFigure(new Point(s.Position.X, s.Position.Y), false, false);
            foreach (Collision v in this.Skip(1))
            {
                ctx.LineTo(new Point(v.Position.X, v.Position.Y), true, false);
            }

            return geometry;
        }

    }

    public class Collision
    {
        public Collision(Vector2 position, PBall ball, CollisionType collisionType)
        {
            this.collisionType = collisionType;
            Position = position;
            Ball = ball;
        }

        public Vector2 Position { get; }
        public PBall Ball { get; }
        private readonly CollisionType collisionType;
    }

    public class PBall 
    {
        public Vector2 position;
        public Vector2 velocity;
        //public bool Resting { get; set; } = false;

        public PBall(Vector2 _position, Vector2 _velocity)
        {
            position = _position;
            velocity = _velocity;
        }


        public Vector2Collection Positions { get; } = new();
        public CollisionCollection Collisions { get; private set; } = new();
        private Vector2 lastPosition = new Vector2();
        private Vector2 lastCollision = new Vector2();

        public Brush Color { get; }
        public Pen Pen { get; }
        public bool DrawTrajectory { get; set; }

        public float r;
        public BitmapImage texture;
        public int index;
        public Vector2 phi = new Vector2(0, 0);
        public RotationMatrix rotation = new RotationMatrix();

        private readonly float cr, u0;
        private static readonly Vector2 rest_velocity = new Vector2(0.7f);
        private const float g = 9.81f;

        public PBall(int _index, float _r, Vector2 _position, Vector2 _velocity, float _u0 = 0.2f, float _cr = 0.02f, Brush color = null) : this(_position, _velocity)
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

        public void Deconstruct(out Vector2 _position, out float _r)
        {
            _position = position;
            _r = r;
        }

        public bool Simulate(float dt, bool animate)
        {
            if (velocity.X == Double.NaN
                || velocity.Y == Double.NaN
                || Vector2.Abs(velocity).SmallerThen(rest_velocity))
            {
                return true;
            }

            Vector2 normalizedVelocity = Vector2.Normalize(velocity);

            Vector2 lag = normalizedVelocity * (5.0f / 2.0f * r * g * (-cr - u0));

            // Leapfrog integration scheme
            position += velocity * dt + lag * (dt * dt / 2.0f);
            velocity += dt * lag;

            if (animate)
            {
            //Calculate new rotation matrix
                Vector2 tangentVelocity = Vector2.Normalize(new Vector2(-velocity.Y, velocity.X));

                phi = velocity * dt + lag * dt * dt / 2.0f / r;

                rotation.RotateAroundAxisWithAngle(tangentVelocity.AsPoint3D(), phi.Length() / r);
            }

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
