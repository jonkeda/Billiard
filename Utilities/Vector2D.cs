/*using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Utilities
{
    struct Vector2
    {
        public readonly float x, y;

        public Vector2(float _x, float _y)
        {
            x = _x;
            y = _y;
        }

        public Vector2(float _xy)
        {
            x = _xy;
            y = _xy;
        }

        public Vector2()
        {
            x = 0;
            y = 0;
        }

        public void Deconstruct(out float _x, out float _y)
        {
            _x = x;
            _y = y;
        }

        public override string ToString()
        {
            return $"x: {x}, y: {y}";
        }

        public Vector2 Normalize() => this / Length;

        public float Length
        {
            get { return Math.Sqrt(x * x + y * y); }
        }

        public float SquaredLength
        {
            get { return x * x + y * y; }
        }

        public bool Zero
        {
            get { return x == 0 && y == 0; }
        }

        public bool Equals(Vector2 to)
        {
            return this.x == to.x && this.y == to.y;
        }

        // +- Vector2
        public static Vector2 operator +(Vector2 a) => a;
        public static Vector2 operator -(Vector2 a) => new(-a.x, -a.y);

        // Vector2 +- Vector2
        public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.x + b.x, a.y + b.y);
        public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.x - b.x, a.y - b.y);

        // Vector2 */ //Vector2
        //public static Vector2 operator *(Vector2 a, Vector2 b) => new(a.x * b.x, a.y * b.y);
        //public static Vector2 operator /(Vector2 a, Vector2 b) => new(a.x / b.x, a.y / b.y);

        // Vector2 */ float
        //public static Vector2 operator *(Vector2 a, float s) => new(a.x * s, a.y * s);
        //public static Vector2 operator /(Vector2 a, float s) => new(a.x / s, a.y / s); // a * (1 / s);

        // float */ Vector2
        //public static Vector2 operator *(float s, Vector2 a) => new(a.x * s, a.y * s);
        //public static Vector2 operator /(float s, Vector2 a) => new(s / a.x, s / a.y); // a / s;

        // bool operators componentwise
        //public static bool operator <(Vector2 a, Vector2 b) => a.x < b.x && a.y < b.y;
        //public static bool operator >(Vector2 a, Vector2 b) => b < a;

        // cast Point to Vector2 vice versa
        //public static implicit operator Vector2(Point p) => new(p.X, p.Y);

        //public static implicit operator Point(Vector2 v) => new(v.x, v.y);

        // cast Point3D to Vector2 vice versa

       // public static implicit operator Vector2(Point3D p) => new Point3D(p.X, p.Y, 0);

        //public static implicit operator Point3D(Vector2 v) => new(v.x, v.y, 0);
    //}
//}
