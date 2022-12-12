using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using Billiard.Camera.vision.algorithms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;
using System.Linq;
using Billiard.Camera.vision.Geometries;
using System.Numerics;
using Emgu.CV.Util;

namespace Billiard.Camera.vision
{
    internal class TableDetector
    {
        public Mat originMat = new();
        public Mat floodFillMat = new();
        public Mat inRangeMat = new();
        public Mat tableMat = new();

        public Mat grayTableMat = new();
        public Mat cannyTableMat = new();
        public Mat hsvTableMat = new();
        public Mat hTableMat = new();
        public Mat sTableMat = new();
        public Mat vTableMat = new();

        public List<PointF> points = new();

        public void Detect(Mat image)
        {
            originMat = image;

            FindContours();

            Rectangle bounds = FloodFill();

            FindInRange();

            points = FindLines(bounds);

            FindHsv();
        }

        private List<PointF> FindLines(Rectangle bounds)
        {
            Size size = bounds.Size;

            // top
            int width4 = size.Width / 4;
            int length4 = size.Height / 3;

            Point? top1 = FindWhiteOnLine(inRangeMat,
                bounds.X + width4, bounds.Y,
                bounds.X + width4, bounds.Y + bounds.Height, false);
            Point? top2 = FindWhiteOnLine(inRangeMat,
                bounds.X + width4 * 3, bounds.Y,
                bounds.X + width4 * 3, bounds.Y + bounds.Height, false);

            Point? bottom1 = FindWhiteOnLine(inRangeMat,
                bounds.X + width4, bounds.Y,
                bounds.X + width4, bounds.Y + bounds.Height, true);
            Point? bottom2 = FindWhiteOnLine(inRangeMat, bounds.X + width4 * 3, bounds.Y, bounds.X + width4 * 3,
                bounds.Y + bounds.Height, true);

            Point? left1 = FindWhiteOnLine(inRangeMat, bounds.X, bounds.Y + length4,
                bounds.X + bounds.Width, bounds.Y + length4, false);
            Point? left2 = FindWhiteOnLine(inRangeMat, bounds.X, bounds.Y + length4 * 2,
                bounds.X + bounds.Width, bounds.Y + length4 * 2, false);

            Point? right1 = FindWhiteOnLine(inRangeMat, bounds.X, bounds.Y + length4,
                bounds.X + bounds.Width, bounds.Y + length4, true);
            Point? right2 = FindWhiteOnLine(inRangeMat, bounds.X, bounds.Y + length4 * 2,
                bounds.X + bounds.Width, bounds.Y + length4 * 2, true);

            List<PointF> pointsf = AddPoints(top1, top2, bottom1, bottom2, left1, left2, right1, right2);
            if (pointsf.Count != 8)
            {
                return new List<PointF>();
            }
            //return pointsf;  

            //RotatedRect rect = CvInvoke.MinAreaRect(pointsf.Select(i => new PointF(i.X, i.Y)).ToArray());

            Vector2 eLeft1 = PointFExtensions.Extend(left1.Value, left2.Value).AsVector2();
            Vector2 eLeft2 = PointFExtensions.Extend(left2.Value, left1.Value).AsVector2();

            Vector2 eTop1 = PointFExtensions.Extend(top1.Value, top2.Value).AsVector2();
            Vector2 eTop2 = PointFExtensions.Extend(top2.Value, top1.Value).AsVector2();

            Vector2 eRight1 = PointFExtensions.Extend(right1.Value, right2.Value).AsVector2();
            Vector2 eRight2 = PointFExtensions.Extend(right2.Value, right1.Value).AsVector2();

            Vector2 eBottom1 = PointFExtensions.Extend(bottom1.Value, bottom2.Value).AsVector2();
            Vector2 eBottom2 = PointFExtensions.Extend(bottom2.Value, bottom1.Value).AsVector2();

            PointF topLeft = PointF.Empty;
            PointF topRight = PointF.Empty;
            PointF bottomLeft = PointF.Empty;
            PointF bottomRight = PointF.Empty;
            List<PointF> rectPoints = new List<PointF>();
            Vector2 intersection;
            if (LineSegmentsIntersect(eTop1, eTop2, eLeft1, eLeft2, out intersection))
            {
                topLeft = intersection.AsPoint();
                rectPoints.Add(topLeft);
            }
            if (LineSegmentsIntersect(eTop1, eTop2, eRight1, eRight2, out intersection))
            {
                topRight = intersection.AsPoint();

                rectPoints.Add(topRight);
            }
            if (LineSegmentsIntersect(eBottom1, eBottom2, eLeft1, eLeft2, out intersection))
            {
                bottomLeft = intersection.AsPoint();
                rectPoints.Add(bottomLeft);
            }
            if (LineSegmentsIntersect(eBottom1, eBottom2, eRight1, eRight2, out intersection))
            {
                bottomRight = intersection.AsPoint();
                rectPoints.Add(bottomRight);
            }

            WarpTablePerspective(topLeft, topRight, bottomLeft, bottomRight);

            return rectPoints;

            // return rect.GetVertices().ToList(); 

        }

        void WarpTablePerspective(PointF topLeft, PointF topRight, PointF bottomLeft, PointF bottomRight)
        {
            bool needSideReverse = false; // table.tableNeedSideReverse();
            VectorOfPointF
                src = new VectorOfPointF(new[]
                {
                    bottomRight,
                    topRight,
                    topLeft,
                    bottomLeft
                });

            VectorOfPointF dest = needSideReverse
                ? new VectorOfPointF(new[]
                    {
                        new PointF(0, 0),
                        new PointF(originMat.Width, 0),
                        new PointF(originMat.Width, originMat.Height),
                        new PointF(0, originMat.Height)
                    }
                )
                : new VectorOfPointF(
                    new[]
                    {
                        new PointF(originMat.Width, originMat.Height),
                        new PointF(originMat.Width, 0),
                        new PointF(0, 0),
                        new PointF(0, originMat.Height)
                    }
                );
            Mat warpingMat = CvInvoke.GetPerspectiveTransform(src, dest);
            CvInvoke.WarpPerspective(originMat, tableMat, warpingMat, originMat.Size);
        }

        public static bool ExtendLineSegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2,
            out Vector2 intersection, bool considerCollinearOverlapAsIntersect = false)
        {
            p1 = (p1 - p2) * 1000 + p1;

            return LineSegmentsIntersect(p1, p2, q1, q2,
                out intersection);
        }

        public static bool LineSegmentsIntersect(Vector2 p, Vector2 p2, Vector2 q, Vector2 q2,
            out Vector2 intersection, bool considerCollinearOverlapAsIntersect = false)
        {
            intersection = new Vector2();

            Vector2 r = p2 - p;
            Vector2 s = q2 - q;
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
                intersection = p + t * r;

                // An intersection was found.
                return true;
            }

            // 5. Otherwise, the two line segments are not parallel but do not intersect.
            return false;
        }

        private List<PointF> AddPoints(params Point?[] points)
        {
            return points.Where(p => p.HasValue).Select(i => new PointF(i.Value.X, i.Value.Y)).ToList();
        }

        private Point? FindWhiteOnLine(Mat mat, int startX, int startY, int endX, int endY, bool reverse)
        {
            if (reverse)
            {
                for (int x = endX; x >= startX; x--)
                {
                    for (int y = endY; y >= startY; y--)
                    {
                        int color = GetColorByte(mat, x, y);
                        if (color == 255)
                        {
                            return new Point(x, y);
                        }
                    }
                }
            }
            else
            {
                for (int x = startX; x <= endX; x++)
                {
                    for (int y = startY; y <= endY; y++)
                    {
                        int color = GetColorByte(mat, x, y);
                        if (color == 255)
                        {
                            return new Point(x, y);
                        }
                    }
                }
            }

            return null;
        }

        private int GetColorByte(Mat image, int x, int y)
        {
            var rawData = image.GetRawData(y, x);
            return rawData[0];
        }

        private void FindInRange()
        {
            CvInvoke.Resize(floodFillMat, inRangeMat, floodFillMat.Size);
            CvInvoke.ExtractChannel(inRangeMat, inRangeMat, 0);
            CvInvoke.InRange(inRangeMat, new ScalarArray(255), new ScalarArray(255), inRangeMat);

        }

        private Rectangle FloodFill()
        {
            CvInvoke.CvtColor(originMat, floodFillMat, ColorConversion.Bgr2Hsv);
            CvInvoke.ExtractChannel(floodFillMat, floodFillMat, 0);
            PointF pointOnTable = VisionAlgorithms.findSimilarPointOnCenterSpiral(floodFillMat);
            Mat mask = new Mat();

            MCvScalar newColor = Color.WHITE.AsMCvScalar();

            float floodFillDiff = 1.5f;

            MCvScalar diff = new MCvScalar(floodFillDiff, floodFillDiff, floodFillDiff);
            Rectangle boundingRect;
            CvInvoke.FloodFill(floodFillMat, mask, pointOnTable.AsWindowsPoint(), newColor,
                out boundingRect, diff, diff, Connectivity.EightConnected
                /* 8 | (255 << 8)*/ );
            return boundingRect;
        }

        private void FindHsv()
        {
            CvInvoke.CvtColor(originMat, hsvTableMat, ColorConversion.Bgr2Hsv);
            CvInvoke.ExtractChannel(hsvTableMat, hTableMat, 0);
            CvInvoke.ExtractChannel(hsvTableMat, sTableMat, 1);
            CvInvoke.ExtractChannel(hsvTableMat, vTableMat, 2);

        }

        private void FindContours()
        {
            CvInvoke.CvtColor(originMat, grayTableMat, ColorConversion.Bgr2Gray);

            float cannyThreshold = 180.0f;
            float cannyThresholdLinking = 120.0f;
            CvInvoke.GaussianBlur(grayTableMat, grayTableMat, new System.Drawing.Size(3, 3), 1);
            CvInvoke.Canny(grayTableMat, cannyTableMat, cannyThreshold, cannyThresholdLinking);

        }
    }
}
