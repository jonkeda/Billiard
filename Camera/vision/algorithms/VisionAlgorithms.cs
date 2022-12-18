using System;
using Emgu.CV;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Billiard.Camera.vision.Geometries;
using Emgu.CV.Structure;
using Math = Billiard.Camera.vision.Geometries.Math;

namespace Billiard.Camera.vision.algorithms
{
    public class VisionAlgorithms
    {

        /**
         * From given line finds center point of given target color.
         * 
         * @param img			picture
         * @param targetColor	color to look for
         * @param lineLength	length of line
         * @param X				line start X-coordinate
         * @param Y				line start Y-coordinate
         * @param xAdd			X increment, f.e. 1
         * @param yAdd			Y increment, f.e. 0
         * @return				average center point of found matching points
         */
        public static PointF findColorOnLine(Mat image, MCvScalar targetColor, int lineLength, int x, int y, int xAdd, int yAdd)
        {
            List<float> foundX = new List<float>();
            List<float> foundY = new List<float>();
            for (int i = 0; i < lineLength; i++)
            {
                var rawData = image.GetRawData(y, x);
                MCvScalar color = new MCvScalar(rawData[0], rawData[1], rawData[2]);
                if (color.Equals(targetColor))
                {
                    foundX.Add( x);
                    foundY.Add(y);
                }
                x += xAdd;
                y += yAdd;
            }
            return foundX.Any()
                ? new PointF(foundX.Sum() / foundX.size(), foundY.Sum() / foundY.size())
                : new PointF(0, 0);
        }

        public static bool isContoursOfSameObject(List<Point> contour1, List<Point> contour2, int maxDistance)
        {
            Rectangle boundingRectangle1 = getBoundingRectangle(contour1, maxDistance);
            Rectangle boundingRectangle2 = getBoundingRectangle(contour2, maxDistance);
            List<Point> restrictedContour1 = contour1.FindAll(it => boundingRectangle2.Contains(it));
            List<Point> restrictedContour2 = contour2.FindAll(it => boundingRectangle1.Contains(it));
            foreach (Point p1 in restrictedContour1)
                foreach (Point p2 in restrictedContour2)
                    if (GeometricMath.rectilinearDistance(p1, p2) <= maxDistance)
                        return true;
            return false;
        }

        public static PointF findSimilarPointOnCenterSpiral(Mat image, float diff = 20, int angleSteps = 100, float radiusIncrement = 0.5f, float xFix = -1f, float yFix = 0.33f)
        {
            if (xFix < 0)
            {
                if (image.Height > image.Width)
                {
                    xFix = 0.33f;
                    yFix = 0.57f;
                }
                else
                {
                    xFix = 0.57f;
                    yFix = 0.33f;
                }
            }

            float angleIncrement = (MathF.PI * 2) / angleSteps;
            Scalar lastColor = new Scalar(-diff, -diff, -diff);
            int x = 0, y = 0, countSimilarLastOnes = 0;
            float a = 0, radius = 0;
            float firstX = 0, firstY = 0;
            while (countSimilarLastOnes <= 30 
                   && radius < (image.height() / 2)
                   && radius < (image.width() / 2))
            {
                x = (int)(Math.cos(a) * radius + (image.width() * xFix));
                y = (int)(Math.sin(a) * radius + (image.height() * yFix));
                Scalar color = averageNeighbourColor(image, x, y);
                float diffToLast = Math.abs(color.x - lastColor.x) + Math.abs(color.y - lastColor.y) +
                                    Math.abs(color.z - lastColor.z);

                if (diffToLast <= diff)
                    countSimilarLastOnes++;

                lastColor = color;
                a += angleIncrement;
                radius += radiusIncrement;
            }
            return new PointF(x, y);
        }

        static Scalar averageNeighbourColor(Mat image, int xCoordinate, int yCoordinate, int radius = 1)
        {
            int pixelCounter = 0;
            float b = 0;
            float g = 0;
            float r = 0;
            for (int x = Math.max(xCoordinate - radius, 0); x <= Math.min(xCoordinate + radius, image.width() - 1); x++)
            {
                for (int y = Math.max(yCoordinate - radius, 0); y <= Math.min(yCoordinate + radius, image.height() - 1); y++)
                {
                    // TODO
/*                    var color = image.Get(y, x);
                    if (color)
                    {
                        b += color[0];

                        g += color[1];

                        r += color[2];

                        pixelCounter++;

                    }
*/                }
            }

            return pixelCounter != 0
                ? new Scalar(b / pixelCounter, g / pixelCounter, r / pixelCounter)
                : new Scalar(0, 0, 0);
        }

        static Rectangle getBoundingRectangle(List<Point> points, int padding = 0)
        {
            int minX = 0, maxX = 0, minY = 0, maxY = 0;
            if (points.size() > 0)
            {
                minX = maxX = points[0].X;
                minY = maxY = points[0].Y;
            }
            foreach (Point point in points)
            {
                if (point.X < minX) minX = point.X;
                if (point.X > maxX) maxX = point.X;
                if (point.Y < minY) minY = point.Y;
                if (point.Y > maxY) maxY = point.Y;
            }
            return new Rectangle(new Point(minX - padding, minY - padding), new Size(maxX + padding, maxY + padding));
        }
    }
}
