/*using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Utilities
{
    struct Vector2
    {
        public readonly float X, Y;

        public Vector2(float _x, float _y)
        {
            X = _x;
            Y = _y;
        }

        public Vector2(float _xy)
        {
            X = _xy;
            Y = _xy;
        }

        public Vector2()
        {
            X = 0;
            Y = 0;
        }

        public void Deconstruct(out float _x, out float _y)
        {
            _x = X;
            _y = Y;
        }

        public override string ToString()
        {
            return $"X: {X}, Y: {Y}";
        }

        public Vector2 Normalize() => this / Length;

        public float Length
        {
            get { return Math.Sqrt(X * X + Y * Y); }
        }

        public float SquaredLength
        {
            get { return X * X + Y * Y; }
        }

        public bool Zero
        {
            get { return X == 0 && Y == 0; }
        }

        public bool Equals(Vector2 to)
        {
            return this.X == to.X && this.Y == to.Y;
        }

        // +- Vector2
        public static Vector2 operator +(Vector2 a) => a;
        public static Vector2 operator -(Vector2 a) => new(-a.X, -a.Y);

        // Vector2 +- Vector2
        public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.X + b.X, a.Y + b.Y);
        public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.X - b.X, a.Y - b.Y);

        // Vector2 */ //Vector2
        //public static Vector2 operator *(Vector2 a, Vector2 b) => new(a.X * b.X, a.Y * b.Y);
        //public static Vector2 operator /(Vector2 a, Vector2 b) => new(a.X / b.X, a.Y / b.Y);

        // Vector2 */ float
        //public static Vector2 operator *(Vector2 a, float s) => new(a.X * s, a.Y * s);
        //public static Vector2 operator /(Vector2 a, float s) => new(a.X / s, a.Y / s); // a * (1 / s);

        // float */ Vector2
        //public static Vector2 operator *(float s, Vector2 a) => new(a.X * s, a.Y * s);
        //public static Vector2 operator /(float s, Vector2 a) => new(s / a.X, s / a.Y); // a / s;

        // bool operators componentwise
        //public static bool operator <(Vector2 a, Vector2 b) => a.X < b.X && a.Y < b.Y;
        //public static bool operator >(Vector2 a, Vector2 b) => b < a;

        // cast PointF to Vector2 vice versa
        //public static implicit operator Vector2(PointF p) => new(p.X, p.Y);

        //public static implicit operator PointF(Vector2 v) => new(v.X, v.Y);

        // cast Point3D to Vector2 vice versa

       // public static implicit operator Vector2(Point3D p) => new Point3D(p.X, p.Y, 0);

        //public static implicit operator Point3D(Vector2 v) => new(v.X, v.Y, 0);
    //}
//}
