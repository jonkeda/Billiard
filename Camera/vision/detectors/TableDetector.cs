using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Billiard.Camera.vision.algorithms;
using Billiard.Camera.vision.Geometries;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace Billiard.Camera.vision.detectors
{
    public class TableDetector
    {
        public Mat originMat = new();
        public Mat floodFillMat = new();
        public Mat floodFillMaskMat = new();
        public Mat inRangeMat = new();
        public Mat sameColorMat = new();
        public Mat tableMat = new();
        public Mat grayTableMat = new();
        public Mat cannyTableMat = new();
        public Mat hsvTableMat = new();
        public Mat hTableMat = new();
        public Mat sTableMat = new();
        public Mat vTableMat = new();

        public List<PointF> floodFillPoints = new();
        public List<PointF> floodFillMPoints = new ();

        public List<PointF> sameColorPoints = new();
        public List<PointF> sameColorMPoints = new ();

        public List<PointF> inRangePoints = new();
        public List<PointF> inRangeMPoints = new();

        public (List<PointF> sameColorPoints, List<PointF> sameColorMPoints) DetectFast(Mat image)
        {
/*            int floodFillColor = 100;

            originMat = image;

            // FindContours();

            *//*            Rectangle bounds = FloodFill(floodFillColor);
                        (floodFillPoints, floodFillMPoints) = FindLines(bounds, floodFillMat, floodFillColor);
            *//*
            Rectangle bounds = FindSameColor(floodFillColor);
            (sameColorPoints, sameColorMPoints) = FindLines(bounds, sameColorMat, floodFillColor);

            //FindInRange(floodFillColor);


            //FindHsv();

            return (sameColorPoints, sameColorMPoints);*/

            Detect(image);
            return (inRangePoints, inRangeMPoints);
        }

        public void Detect(Mat image)
        {
            int floodFillColor = 100;

            originMat = image;

            FindContours();

            Rectangle bounds = FloodFill(floodFillColor);
            (floodFillPoints, floodFillMPoints) = FindLines(bounds, floodFillMat, floodFillColor);

            bounds = FindInRange(floodFillColor);
            (inRangePoints, inRangeMPoints) = FindMultiLines(bounds, inRangeMat, floodFillColor);

            FloodFillMatHoughLines = FloodFill2(floodFillColor);

            bounds = FindSameColor(floodFillColor);
            (sameColorPoints, sameColorMPoints) = FindLines(bounds, sameColorMat, floodFillColor);

            FindHoughLines();

            FindHsv();

            WarpTablePerspective(inRangeMPoints.ToArray());

        }

        public LineSegment2D[] HoughLines;

        public LineSegment2D[] FloodFillMatHoughLines;
        
        private void FindHoughLines()
        {
            Mat cannyEdges = new Mat();

            float cannyThreshold = 180.0f;
            float cannyThresholdLinking = 120.0f;
            CvInvoke.Canny(inRangeMat, cannyEdges, cannyThreshold, cannyThresholdLinking);

            HoughLines = CvInvoke.HoughLinesP(
                cannyEdges,
                1, //Distance resolution in pixel-related units
                System.Math.PI / 45.0, //Angle resolution measured in radians.
                20, //threshold
                30, //min Line width
                10); //gap between lines
        }

        private (List<PointF> rectPoints, List<PointF> pointsf) FindLines(Rectangle bounds, Mat image, int findColor)
        {
            Size size = bounds.Size;

            // top
            int width4 = (int) (size.Width * 0.45);
            int length4 = (int) (size.Height * 0.45);

            Point? top1 = FindColorOnLine(image,
                bounds.X + width4, bounds.Y,
                bounds.X + width4, bounds.Y + bounds.Height, false, findColor);
            Point? top2 = FindColorOnLine(image,
                bounds.X + bounds.Width - width4, bounds.Y,
                bounds.X + bounds.Width - width4, bounds.Y + bounds.Height, false, findColor);

            Point? bottom1 = FindColorOnLine(image,
                bounds.X + width4, bounds.Y,
                bounds.X + width4, bounds.Y + bounds.Height, true, findColor);
            Point? bottom2 = FindColorOnLine(image, bounds.X + bounds.Width - width4, bounds.Y,
                bounds.X + bounds.Width - width4,
                bounds.Y + bounds.Height, true, findColor);

            Point? left1 = FindColorOnLine(image, bounds.X, bounds.Y + length4,
                bounds.X + bounds.Width, bounds.Y + length4, false, findColor);
            Point? left2 = FindColorOnLine(image, bounds.X, bounds.Y + bounds.Height - length4,
                bounds.X + bounds.Width, bounds.Y + bounds.Height - length4, false, findColor);

            Point? right1 = FindColorOnLine(image, bounds.X, bounds.Y + length4,
                bounds.X + bounds.Width, bounds.Y + length4, true, findColor);
            Point? right2 = FindColorOnLine(image, bounds.X, bounds.Y + bounds.Height - length4,
                bounds.X + bounds.Width, bounds.Y + bounds.Height - length4, true, findColor);

            List<PointF> pointsf = AddPoints(top1, top2, bottom1, bottom2, left1, left2, right1, right2);
            if (pointsf.Count != 8)
            {
                return (new List<PointF>(), new List<PointF>());
            }
            return (pointsf, FindCorners(left1, left2, top1, top2, bottom1, bottom2, right1, right2));  
        }

        private (List<PointF> rectPoints, List<PointF> pointsf) FindMultiLines(Rectangle bounds, Mat image, int findColor)
        {
            Size size = bounds.Size;

            // top
            int steps = 8;
            int start = 2;
            int end = 6;
            int widthStep = (int)(size.Width / steps);
            int lengthStep = (int)(size.Height / steps);
            int sideX = size.Width / 20;
            int sideY = size.Height / 20;
            int foundStep = 0;
            int sign = 1;

            // top
            foundStep = start;
            Point? top1 = FindColorOnLine(image,
                bounds.X + (start *  widthStep), bounds.Y,
                bounds.X + (start * widthStep), bounds.Y + bounds.Height, false, findColor);
            for (int i = start + 1; i < end; i++)
            {
                Point? p = FindColorOnLine(image,
                    bounds.X + (i * widthStep), bounds.Y,
                    bounds.X + (i * widthStep), bounds.Y + bounds.Height, false, findColor);
                if (!top1.HasValue || 
                     (p.HasValue 
                      && p.Value.Y < top1.Value.Y
                      && p.Value.Y > 5))
                {
                    top1 = p;
                    foundStep = i;
                }
            }
            sign = (foundStep > 8 / 2) ? -1 : 1; 
            Point? top2 = FindColorOnLine(image,
                top1.Value.X + sign * sideX, bounds.Y,
                top1.Value.X + sign * sideX, bounds.Y + bounds.Height, false, findColor);

            // bottom
            foundStep = start;
            Point? bottom1 = FindColorOnLine(image,
                bounds.X + (start * widthStep), bounds.Y,
                bounds.X + (start * widthStep), bounds.Y + bounds.Height, true, findColor);
            for (int i = start + 1; i < end; i++)
            {
                Point? p = FindColorOnLine(image,
                    bounds.X + (i * widthStep), bounds.Y,
                    bounds.X + (i * widthStep), bounds.Y + bounds.Height, true, findColor);
                if (!bottom1.HasValue ||
                    (p.HasValue
                     && p.Value.Y > bottom1.Value.Y
                     && p.Value.Y < size.Height - 5))
                {
                    bottom1 = p;
                    foundStep = i;
                }
            }
            sign = (foundStep > 8 / 2) ? -1 : 1;
            Point? bottom2 = FindColorOnLine(image,
                bottom1.Value.X + sign * sideX, bounds.Y,
                bottom1.Value.X + sign * sideX, bounds.Y + bounds.Height, true, findColor);

            // left
            foundStep = start;
            Point? left1 = FindColorOnLine(image, bounds.X, bounds.Y + start * lengthStep,
                bounds.X + bounds.Width, bounds.Y + start * lengthStep, false, findColor);
            for (int i = start + 1; i < end; i++)
            {
                Point? p = FindColorOnLine(image, bounds.X, bounds.Y + i * lengthStep,
                    bounds.X + bounds.Width, bounds.Y + i * lengthStep, false, findColor);
                if (!left1.HasValue ||
                    (p.HasValue
                     && p.Value.X < left1.Value.X
                     && p.Value.X > 5))
                {
                    left1 = p;
                    foundStep = i;
                }
            }
            sign = (foundStep > 8 / 2) ? -1 : 1;
            Point? left2 = FindColorOnLine(image, bounds.X, left1.Value.Y + sign * sideY,
                bounds.X + bounds.Width, left1.Value.Y + sign * sideY, false, findColor);

            // right
            foundStep = start;
            Point? right1 = FindColorOnLine(image, bounds.X, bounds.Y + start * lengthStep,
                bounds.X + bounds.Width, bounds.Y + start * lengthStep, true, findColor);
            for (int i = start + 1; i < end; i++)
            {
                Point? p = FindColorOnLine(image, bounds.X, bounds.Y + i * lengthStep,
                    bounds.X + bounds.Width, bounds.Y + i * lengthStep, true, findColor);
                if (!right1.HasValue ||
                    (p.HasValue
                     && p.Value.X > right1.Value.X
                     && p.Value.X < size.Width - 5))
                {
                    right1 = p;
                    foundStep = i;
                }
            }
            sign = (foundStep > 8 / 2) ? -1 : 1;
            Point? right2 = FindColorOnLine(image, bounds.X, right1.Value.Y + sign * sideY,
                bounds.X + bounds.Width, right1.Value.Y + sign * sideY, true, findColor);


            List<PointF> pointsf = AddPoints(top1, top2, bottom1, bottom2, left1, left2, right1, right2);
            if (pointsf.Count != 8)
            {
                return (new List<PointF>(), new List<PointF>());
            }
            return (pointsf, FindCorners(left1, left2, top1, top2, bottom1, bottom2, right1, right2));
        }


        private List<PointF> FindCorners(Point? left1, Point? left2, Point? top1, Point? top2, Point? bottom1, Point? bottom2, Point? right1, Point? right2) 
        {
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
                topLeft = intersection.AsPointF();
                rectPoints.Add(topLeft);
            }
            if (LineSegmentsIntersect(eTop1, eTop2, eRight1, eRight2, out intersection))
            {
                topRight = intersection.AsPointF();

                rectPoints.Add(topRight);
            }
            if (LineSegmentsIntersect(eBottom1, eBottom2, eRight1, eRight2, out intersection))
            {
                bottomRight = intersection.AsPointF();
                rectPoints.Add(bottomRight);
            }
            if (LineSegmentsIntersect(eBottom1, eBottom2, eLeft1, eLeft2, out intersection))
            {
                bottomLeft = intersection.AsPointF();
                rectPoints.Add(bottomLeft);
            }

            return rectPoints;
        }

        void WarpTablePerspective(PointF topLeft, PointF topRight, PointF bottomLeft, PointF bottomRight)
        {
            WarpTablePerspective(new[]
            {
                topLeft,
                topRight,
                bottomRight,
                bottomLeft
            });
        }

        void WarpTablePerspective(PointF[] pointArray)
        {
            if (pointArray.Length < 4)
            {
                return;
            }

            bool needSideReverse = true; // table.tableNeedSideReverse();
            VectorOfPointF
                src = new VectorOfPointF(pointArray);

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

        public void WarpTablePerspective(Mat mat, List<PointF> tableCornerPoints, ref PointF whiteBallPoint, ref PointF yellowBallPoint, ref PointF redBallPoint)
        {
            bool needSideReverse = true; // table.tableNeedSideReverse();
            VectorOfPointF
                dest = new VectorOfPointF(tableCornerPoints.ToArray());

            VectorOfPointF src = needSideReverse
                ? new VectorOfPointF(new[]
                    {
                        new PointF(0, 0),
                        new PointF(mat.Width, 0),
                        new PointF(mat.Width, mat.Height),
                        new PointF(0, mat.Height)
                    }
                )
                : new VectorOfPointF(
                    new[]
                    {
                        new PointF(mat.Width, mat.Height),
                        new PointF(mat.Width, 0),
                        new PointF(0, 0),
                        new PointF(0, mat.Height)
                    }
                );
            Mat warpingMat = CvInvoke.GetPerspectiveTransform(src, dest);

            PointF[] points = CvInvoke.PerspectiveTransform(new[] { whiteBallPoint, yellowBallPoint, redBallPoint }, warpingMat);
            whiteBallPoint = points[0];
            yellowBallPoint = points[1];
            redBallPoint = points[2];
        }

/*        public static bool ExtendLineSegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2,
            out Vector2 intersection, bool considerCollinearOverlapAsIntersect = false)
        {
            p1 = (p1 - p2) * 1000 + p1;

            return LineSegmentsIntersect(p1, p2, q1, q2,
                out intersection);
        }*/

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

        private Point? FindColorOnLine(Mat mat, int startX, int startY, int endX, int endY, bool reverse, int findColor)
        {
            if (reverse)
            {
                for (int x = endX; x >= startX; x--)
                {
                    for (int y = endY; y >= startY; y--)
                    {
                        int color = GetColorByte(mat, x, y);
                        if (color == findColor)
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
                        if (color == findColor)
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
            if (x < 0
                || y < 0
                || y >= image.Rows
                || x >= image.Cols)
            {
                return -1;
            }


            var rawData = image.GetRawData(y, x);
            return rawData[0];
        }

        private Rectangle FindInRange(int floodFillColor)
        {
            CvInvoke.Resize(floodFillMat, inRangeMat, floodFillMat.Size);
            CvInvoke.ExtractChannel(inRangeMat, inRangeMat, 0);
            CvInvoke.InRange(inRangeMat, new ScalarArray(floodFillColor), new ScalarArray(floodFillColor), inRangeMat);

            PointF pointOnTable = VisionAlgorithms.findMidPoint(inRangeMat);


            MCvScalar newColor = new MCvScalar(floodFillColor);
            Mat mask = new Mat();
            MCvScalar diff = new MCvScalar(0, 0, 0);
            CvInvoke.FloodFill(inRangeMat, mask, pointOnTable.AsWindowsPoint(), newColor,
                out var boundingRect, diff, diff, Connectivity.EightConnected);

            CvInvoke.InRange(inRangeMat, new ScalarArray(floodFillColor), new ScalarArray(floodFillColor), inRangeMat);

            mask = new Mat();
            CvInvoke.FloodFill(inRangeMat, mask, pointOnTable.AsWindowsPoint(), newColor,
                out _, diff, diff, Connectivity.EightConnected);

            return boundingRect;
        }

        private MCvScalar GetColorScalarArray(Mat image, int x, int y)
        {
            var rawData = image.GetRawData(y, x);
            return new MCvScalar(rawData[0], rawData[1], rawData[2]);
            //return rawData[0];
        }


        private Rectangle FindSameColor(int floodFillColor)
        {
            CvInvoke.CvtColor(originMat, sameColorMat, ColorConversion.Bgr2Hsv);
            CvInvoke.ExtractChannel(sameColorMat, sameColorMat, 0);
            PointF pointOnTable = VisionAlgorithms.findSimilarPointOnCenterSpiral(sameColorMat);

            /*            MCvScalar color = GetColorScalarArray(sameColorMat, (int)pointOnTable.X, (int)pointOnTable.Y);
                        CvInvoke.InRange(sameColorMat, new ScalarArray(color), new ScalarArray(color), sameColorMat);
            */
            int colorInt = GetColorByte(sameColorMat, (int)pointOnTable.X, (int)pointOnTable.Y);
            CvInvoke.InRange(sameColorMat,
                new ScalarArray(System.Math.Max(colorInt - 5, 0)),
                new ScalarArray(System.Math.Min(colorInt + 5, 180)), sameColorMat);


            CvInvoke.BitwiseNot(sameColorMat, sameColorMat);

            MCvScalar newColor = new MCvScalar(floodFillColor);
            Mat mask = new Mat();
            MCvScalar diff = new MCvScalar(0,0,0);
            CvInvoke.FloodFill(sameColorMat, mask, pointOnTable.AsWindowsPoint(), newColor,
                out var boundingRect, diff, diff, Connectivity.EightConnected);
            return boundingRect;
            //return Rectangle.Empty;
            
        }

        private Rectangle FloodFill(int floodFillColor)
        {
            CvInvoke.CvtColor(originMat, floodFillMat, ColorConversion.Bgr2Hsv);
            CvInvoke.ExtractChannel(floodFillMat, floodFillMat, 0);

            CvInvoke.GaussianBlur(floodFillMat, floodFillMat, new Size(5, 5), 1);
            // get rid of small objects
            /*            Mat kernelOp = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
                        CvInvoke.MorphologyEx(floodFillMat, floodFillMat, MorphOp.Open, kernelOp, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
                        Mat kernelCl = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(11, 11), new Point(-1, -1));
                        CvInvoke.MorphologyEx(floodFillMat, floodFillMat, MorphOp.Close, kernelCl, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
            */
            // PointF pointOnTable = VisionAlgorithms.findSimilarPointOnCenterSpiral(floodFillMat);
            PointF pointOnTable = VisionAlgorithms.findMidPoint(floodFillMat);
            
            Mat mask = new Mat();
            MCvScalar newColor = new MCvScalar(floodFillColor);
            float floodFillDiff = 1.5f;
            MCvScalar diff = new MCvScalar(floodFillDiff, floodFillDiff, floodFillDiff);
            Rectangle boundingRect;
            CvInvoke.FloodFill(floodFillMat, mask, pointOnTable.AsWindowsPoint(), newColor,
                out boundingRect, diff, diff, Connectivity.EightConnected);
            

            return boundingRect;
        }

        private LineSegment2D[] FloodFill2(int floodFillColor)
        {
            CvInvoke.Resize(inRangeMat, floodFillMaskMat, inRangeMat.Size);

            PointF pointOnTable = VisionAlgorithms.findMidPoint(floodFillMaskMat);
            Mat mask = new Mat();
            MCvScalar newColor = new MCvScalar(floodFillColor);

            float floodFillDiff = 1.5f;

            MCvScalar diff = new MCvScalar(floodFillDiff, floodFillDiff, floodFillDiff);
            Rectangle boundingRect;
            CvInvoke.FloodFill(floodFillMaskMat, mask, pointOnTable.AsWindowsPoint(), newColor,
                out boundingRect, diff, diff, Connectivity.EightConnected);

            MCvScalar color = new MCvScalar(floodFillColor, floodFillColor, floodFillColor);
            CvInvoke.InRange(floodFillMaskMat, new ScalarArray(color), new ScalarArray(color), floodFillMaskMat);


            float cannyThreshold = 180.0f;
            float cannyThresholdLinking = 120.0f;
            CvInvoke.GaussianBlur(floodFillMaskMat, floodFillMaskMat, new Size(3, 3), 1);
            CvInvoke.Canny(floodFillMaskMat, floodFillMaskMat, cannyThreshold, cannyThresholdLinking);

            return CvInvoke.HoughLinesP(
                floodFillMaskMat,
                1, //Distance resolution in pixel-related units
                System.Math.PI / 45.0, //Angle resolution measured in radians.
                20, //threshold
                30, //min Line width
                10); //gap between lines
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
            CvInvoke.GaussianBlur(grayTableMat, grayTableMat, new Size(3, 3), 1);
            CvInvoke.Canny(grayTableMat, cannyTableMat, cannyThreshold, cannyThresholdLinking);

        }
    }
}
