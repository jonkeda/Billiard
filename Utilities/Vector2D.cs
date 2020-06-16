using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Utilities
{
    class Vector2D
    {
        public double x, y;

        public Vector2D(double _x, double _y)
        {
            x = _x;
            y = _y;
        }

        public Vector2D(double _xy)
        {
            x = _xy;
            y = _xy;
        }

        public Vector2D()
        {
            x = 0;
            y = 0;
        }

        public void Deconstruct(out double _x, out double _y)
        {
            _x = x;
            _y = y;
        }

        public override string ToString()
        {
            return $"x: {x}, y: {y}";
        }

        public Vector2D Normalize() => this / Length;

        public double Length
        {
            get { return Math.Sqrt(x * x + y * y); }
        }

        public double SquaredLength
        {
            get { return x * x + y * y; }
        }

        public bool Zero
        {
            get { return x == 0 && y == 0; }
        }

        // +- Vector2D
        public static Vector2D operator +(Vector2D a) => a;
        public static Vector2D operator -(Vector2D a) => new Vector2D(-a.x, -a.y);

        // Vector2D +- Vector2D
        public static Vector2D operator +(Vector2D a, Vector2D b) => new Vector2D(a.x + b.x, a.y + b.y);
        public static Vector2D operator -(Vector2D a, Vector2D b) => a + (-b);

        // Vector2D */ Vector2D
        public static Vector2D operator *(Vector2D a, Vector2D b) => new Vector2D(a.x * b.x, a.y * b.y);
        public static Vector2D operator /(Vector2D a, Vector2D b) => new Vector2D(a.x / b.x, a.y / b.y);

        // Vector2D */ double
        public static Vector2D operator *(Vector2D a, double s) => new Vector2D(a.x * s, a.y * s);
        public static Vector2D operator /(Vector2D a, double s) => a * (1 / s);

        // double */ Vector2D
        public static Vector2D operator *(double s, Vector2D a) => a * s;
        public static Vector2D operator /(double s, Vector2D a) => a / s;

        // bool operators componentwise
        public static bool operator <(Vector2D a, Vector2D b) => a.x < b.x && a.y < b.y;
        public static bool operator >(Vector2D a, Vector2D b) => b < a;

        // cast Point to Vector2D vice versa
        public static implicit operator Vector2D(Point p) => new Vector2D(p.X, p.Y);

        public static implicit operator Point(Vector2D v) => new Point(v.x, v.y);

        // cast Point3D to Vector2D vice versa

        public static implicit operator Vector2D(Point3D p) => new Point3D(p.X, p.Y, 0);

        public static implicit operator Point3D(Vector2D v) => new Point3D(v.x, v.y, 0);
    }
}
