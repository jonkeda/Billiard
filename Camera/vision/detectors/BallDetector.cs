using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Billiard.Camera.vision.Geometries;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace Billiard.Camera.vision.detectors
{
    public class BallDetector
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

        public Mat hlsTableMat = new();
        public Mat h2TableMat = new();
        public Mat l2TableMat = new();
        public Mat s2TableMat = new();

        public Mat hueMat = new();
        
        public Mat whiteBallMat = new();
        public Mat yellowBallMat = new();
        public Mat redBallMat = new();


        public List<PointF> points = new();
        private System.Windows.Point whiteBallPoint;
        private System.Windows.Point redBallPoint;
        private System.Windows.Point yellowBallPoint;

        public System.Windows.Point WhiteBallPoint
        {
            get { return whiteBallPoint; }
            set
            {
                whiteBallPoint = value;
                WhiteBallRelativePoint = ToRelativePoint(WhiteBallPoint);
            }
        }

        public System.Windows.Point WhiteBallRelativePoint
        {
            get;
            set;
        }

        public System.Windows.Point RedBallPoint
        {
            get { return redBallPoint; }
            set
            {
                redBallPoint = value;
                RedBallRelativePoint = ToRelativePoint(RedBallPoint);
            }
        }

        public System.Windows.Point RedBallRelativePoint
        {
            get;
            set;
        }

        public System.Windows.Point YellowBallPoint
        {
            get { return yellowBallPoint; }
            set
            {
                yellowBallPoint = value;
                YellowBallRelativePoint = ToRelativePoint(YellowBallPoint);
            }
        }

        public System.Windows.Point YellowBallRelativePoint
        {
            get;
            set;
        }

        public System.Windows.Point ToRelativePoint(System.Windows.Point p)
        {
            if (p.X == 0 && p.Y == 0)
            {
                return p;
            }

            if (originMat.Height > originMat.Width)
            {
                return new System.Windows.Point(p.Y / originMat.Height, 1 - (p.X / originMat.Width));
            }
            return new System.Windows.Point(p.X / originMat.Width, p.Y / originMat.Height);
        }

        public (System.Windows.Point whiteBallPoint, System.Windows.Point yellowBallPoint, System.Windows.Point redBallPoint) DetectFast(Mat image)
        {
            originMat = image;

            CvInvoke.CvtColor(originMat, hsvTableMat, ColorConversion.Bgr2Hsv);
            FindWhiteBall();
            FindYellowBall();
            FindRedBall();


            return (whiteBallPoint, yellowBallPoint, redBallPoint);
        }

        public void Detect(Mat image)
        {
            originMat = image;

            FindContours();

            FindHsv();

            FindHls();

            HsvToRgb();

            FindWhiteBall();
            FindYellowBall();
            FindRedBall();
        }

        private void FindInRange()
        {
            CvInvoke.Resize(floodFillMat, inRangeMat, floodFillMat.Size);
            CvInvoke.ExtractChannel(inRangeMat, inRangeMat, 0);
            CvInvoke.InRange(inRangeMat, new ScalarArray(255), new ScalarArray(255), inRangeMat);
        }

        private System.Windows.Point FindBall(Mat image)
        {
            CvInvoke.GaussianBlur(image, image, new Size(5, 5), 1);
            // get rid of small objects
            Mat kernelOp = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
            CvInvoke.MorphologyEx(image, image, MorphOp.Open, kernelOp, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
            Mat kernelCl = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(11, 11), new Point(-1, -1));
            CvInvoke.MorphologyEx(image, image, MorphOp.Close, kernelCl, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(image, contours, null, RetrType.List, ChainApproxMethod.ChainApproxNone);

            List<VectorOfPoint> foundContours = new List<VectorOfPoint>();
            for (int i = 0; i < contours.Size; i++)
            {
                if (contours[i] != null)
                    foundContours.Add(contours[i]);
            }

            if (foundContours.Count > 0)
            {
                VectorOfPoint foundContour = foundContours[0];
                double foundContourArea = CvInvoke.ContourArea(foundContours[0]);
                foreach (VectorOfPoint contour in foundContours.Skip(1))
                {
                    double area = CvInvoke.ContourArea(contour);
                    if (area > foundContourArea)
                    {
                        foundContourArea = area;
                        foundContour = contour;
                    }
                }
                Moments m = CvInvoke.Moments(foundContour);
                int cX = (int)(m.M10 / m.M00);
                int cY = (int)(m.M01 / m.M00);
                return new System.Windows.Point(cX, cY);
            }
            return new System.Windows.Point(0, 0);
        }

        private void FindWhiteBall()
        {
            int sensitivity = 50;
            ScalarArray lower = new ScalarArray(new MCvScalar(0, 0, 255 - sensitivity));
            ScalarArray upper = new ScalarArray(new MCvScalar(255, sensitivity, 255));

/*            ScalarArray lower = new ScalarArray(new MCvScalar(110, 100, 100));
            ScalarArray upper = new ScalarArray(new MCvScalar(130, 255, 255));
*/
/*            ScalarArray lower = new ScalarArray(new MCvScalar(0, 0, 128));
            ScalarArray upper = new ScalarArray(new MCvScalar(0, 0, 255));
*/
            CvInvoke.InRange(hsvTableMat, lower, upper, whiteBallMat);
            WhiteBallPoint = FindBall(whiteBallMat);
        }

        private void FindRedBall()
        {
            ScalarArray lower = new ScalarArray(new MCvScalar(160, 100, 100));
            ScalarArray upper = new ScalarArray(new MCvScalar(180, 255, 255));

            CvInvoke.InRange(hsvTableMat, lower, upper, redBallMat);
            RedBallPoint = FindBall(redBallMat);
        }

        private void FindYellowBall()
        {
            ScalarArray lower = new ScalarArray(new MCvScalar(22, 93, 128));
            ScalarArray upper = new ScalarArray(new MCvScalar(45, 255, 255));

            CvInvoke.InRange(hsvTableMat, lower, upper, yellowBallMat);
            YellowBallPoint = FindBall(yellowBallMat);
        }

        private void FindHsv()
        {
            CvInvoke.CvtColor(originMat, hsvTableMat, ColorConversion.Bgr2Hsv);
            CvInvoke.ExtractChannel(hsvTableMat, hTableMat, 0);

            CvInvoke.ExtractChannel(hsvTableMat, sTableMat, 1);
            CvInvoke.ExtractChannel(hsvTableMat, vTableMat, 2);
        }

        private void HsvToRgb()
        {
            //CvInvoke.GaussianBlur(originMat, originMat, new Size(3, 3), 1);
            CvInvoke.CvtColor(originMat, hsvTableMat, ColorConversion.Bgr2Hsv);

            Mat hMat = new Mat();
            CvInvoke.ExtractChannel(hsvTableMat, hMat, 0);
            
            Mat sMat = new Mat();
            CvInvoke.ExtractChannel(hsvTableMat, sMat, 1);
            sMat.SetTo(new MCvScalar(255));

            Mat vMat = new Mat();
            CvInvoke.ExtractChannel(hsvTableMat, vMat, 2);
            vMat.SetTo(new MCvScalar(255));

            CvInvoke.Merge(new VectorOfMat(hMat, sMat, vMat), hueMat);

            CvInvoke.CvtColor(hueMat, hueMat, ColorConversion.Hsv2Bgr);
        }


        private void FindHls()
        {
            CvInvoke.GaussianBlur(originMat, originMat, new Size(3, 3), 1);
            CvInvoke.CvtColor(originMat, hlsTableMat, ColorConversion.Bgr2Hls);
            CvInvoke.ExtractChannel(hlsTableMat, h2TableMat, 0);
            CvInvoke.ExtractChannel(hlsTableMat, l2TableMat, 1);
            CvInvoke.ExtractChannel(hlsTableMat, s2TableMat, 2);
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
