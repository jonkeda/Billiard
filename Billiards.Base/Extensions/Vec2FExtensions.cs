using System.Numerics;
using OpenCvSharp;

namespace Billiards.Base.Extensions
{
    public static class Vec2FExtensions
    {
        #region As

        public static Point2f AsPoint2f(this Vec2f p)
        {
            return new Point2f(p.Item0, p.Item1);
        }

        public static Vec2f AsVec2f(this Point2f p)
        {
            return new Vec2f(p.X, p.Y);
        }

        public static Vec2f AsVec2f(this Point p)
        {
            return new Vec2f(p.X, p.Y);
        }

        public static float X(this Vec2f v)
        {
            return v.Item0;
        }

        public static float Y(this Vec2f v)
        {
            return v.Item1;
        }

        public static Vec2f AsVec2f(this Vector2 p)
        {
            return new Vec2f(p.X, p.Y);
        }

        public static Vector2 AsVector2(this Vec2f p)
        {
            return new Vector2(p.X(), p.Y());
        }

        #endregion


        #region Calculations

        public static float Dot(Vec2f i1, Vec2f i2)
        {
            return Vector2.Dot(i1.AsVector2(), i2.AsVector2());
        }

        private static Vec2f vec1 = new Vec2f(1f, 0f);
        public static float GetAngle(this Vec2f a)
        {
            if (a.Y() > 0)
            {
                return MathF.Acos(Dot(a, vec1)) * 180f / MathF.PI;
            }
            return -MathF.Acos(Dot(a, vec1)) * 180f / MathF.PI;
        }

        public static float Length(this Vec2f u)
        {
            return u.AsVector2().Length();
        }

        public static Vec2f Normalize(this Vec2f u)
        {
            return Vector2.Normalize(u.AsVector2()).AsVec2f();
        }

        public static float Cross(this Vec2f u, Vec2f v)
        {
            return u.X() * v.Y() - u.Y() * v.X();
        }

        public static Vec2f Center(this Vec2f u, Vec2f v)
        {
            return u + (v - u) / 2;
        }

        #endregion


        #region Extend

        public static Vec2f? Extend(Point2f? p1, Point2f? p2)
        {
            return Extend(p1, p2, 1000);
        }

        public static Vec2f? Extend(Vec2f v1, Vec2f v2, int extender)
        {
            Vec2f x = (v1 - v2) * extender + v1;
            return x;
        }

        public static Vec2f? Extend(Point2f? p1, Point2f? p2, int extender)
        {
            if (!p1.HasValue
                || !p2.HasValue)
            {
                return null;
            }
            return Extend(p1.Value.AsVec2f(), p2.Value.AsVec2f(), extender);
        }

        public static Point2f Extend(Point2f p1, Point2f p2)
        {
            Vec2f v1 = p1.AsVec2f();
            Vec2f v2 = p2.AsVec2f();
            Vec2f x = (v1 - v2) * 1000 + v1;
            return x.AsPoint2f();
        }

        #endregion

        #region As

        public static bool LineSegmentsIntersect(Vec2f? pn, Vec2f? p2n, Vec2f? qn, Vec2f? q2n,
        out Vec2f intersection, bool considerCollinearOverlapAsIntersect = false)
        {
            intersection = new Vec2f();

            if (!pn.HasValue
                || !p2n.HasValue
                || !qn.HasValue
                || !q2n.HasValue)
            {
                return false;
            }
            Vec2f p = pn.Value;
            Vec2f p2 = p2n.Value;
            Vec2f q = qn.Value;
            Vec2f q2 = q2n.Value;

            Vec2f r = p2 - p;
            Vec2f s = q2 - q;
            float rxs = r.Cross(s);
            float qpxr = (q - p).Cross(r);

            // If r x s = 0 and (q - p) x r = 0, then the two lines are collinear.
            if (rxs.IsZero() && qpxr.IsZero())
            {
                // 1. If either  0 <= (q - p) * r <= r * r or 0 <= (p - q) * s <= * s
                // then the two lines are overlapping,
                /*                if (considerCollinearOverlapAsIntersect)
                                    if ((0 <= (q - p) * r 
                                         && (q - p) * r <= r * r) 
                                        || (0 <= (p - q) * s && (p - q) * s <= s * s))
                                        return true;
                */
                // 2. If neither 0 <= (q - p) * r = r * r nor 0 <= (p - q) * s <= s * s
                // then the two lines are collinear but disjoint.
                // No need to implement this expression, as it follows from the expression above.
                return false;
            }

            // 3. If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
            if (rxs.IsZero() && !qpxr.IsZero())
                return false;

            // t = (q - p) x s / (r x s)
            var t = (q - p).Cross(s) / rxs;

            // u = (q - p) x r / (r x s)

            var u = (q - p).Cross(r) / rxs;

            // 4. If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1
            // the two line segments meet at the point p + t r = q + u s.
            if (!rxs.IsZero() && (0 <= t && t <= 1) && (0 <= u && u <= 1))
            {
                // We can calculate the intersection point using either t or u.
                intersection = p + r * t;

                // An intersection was found.
                return true;
            }

            // 5. Otherwise, the two line segments are not parallel but do not intersect.
            return false;
        }

        public static bool LineIntersect(Point2f? left1, Point2f? left2,
            Point2f? top1,
            Point2f? top2, out Vec2f intersection)
        {
            Vec2f? eLeft1 = Extend(left1, left2, 10);
            Vec2f? eLeft2 = Extend(left2, left1, 10);

            Vec2f? eTop1 = Extend(top1, top2, 10);
            Vec2f? eTop2 = Extend(top2, top1, 10);

            return LineSegmentsIntersect(eTop1, eTop2, eLeft1, eLeft2, out intersection);
        }

        public static bool LineIntersect(Vec2f left1, Vec2f left2,
            Vec2f top1, Vec2f top2,
            out Vec2f intersection)
        {
            Vec2f? eLeft1 = Extend(left1, left2, 10);
            Vec2f? eLeft2 = Extend(left2, left1, 10);

            Vec2f? eTop1 = Extend(top1, top2, 10);
            Vec2f? eTop2 = Extend(top2, top1, 10);

            return LineSegmentsIntersect(eTop1, eTop2, eLeft1, eLeft2, out intersection);
        }

        #endregion
    }
}
