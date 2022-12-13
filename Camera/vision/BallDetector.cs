using System.Collections.Generic;
using Billiard.Camera.vision.algorithms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;

using Billiard.Camera.vision.Geometries;
using Billiard.UI.Converters;
using MaterialDesignColors.ColorManipulation;
using System.Threading.Tasks;
using Emgu.CV.Util;


namespace Billiard.Camera.vision
{
    internal class BallDetector
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

        public Mat whiteBallMat = new();
        public Mat yellowBallMat = new();
        public Mat redBallMat = new();


        public List<PointF> points = new();
        public System.Windows.Point whiteBallPoint;
        public System.Windows.Point redBallPoint;
        public System.Windows.Point yellowBallPoint;

        public void Detect(Mat image)
        {
            originMat = image;

            FindContours();

            FindHsv();

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
            CvInvoke.GaussianBlur(image, image, new System.Drawing.Size(5, 5), 1);

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(image, contours, null, RetrType.External, ChainApproxMethod.ChainApproxNone);

            List<VectorOfPoint> foundContours = new List<VectorOfPoint>();
            for (int i = 0; i < contours.Size; i++)
            {
                if (contours[i] != null)
                    foundContours.Add(contours[i]);
            }

            if (foundContours.Count == 1)
            {
                 var M = CvInvoke.Moments(foundContours[0]);
                
                 var cX = (int) (M.M10 / M.M00);

                 var cY = (int) (M.M01 / M.M00);
                return new System.Windows.Point(cX, cY);
            }
            return new System.Windows.Point(0,0);
        }

        private void FindWhiteBall()
        {
            ScalarArray lower = new ScalarArray(new MCvScalar(0, 0, 128));
            ScalarArray upper = new ScalarArray(new MCvScalar(0, 0, 255));

            CvInvoke.InRange(hsvTableMat, lower, upper, whiteBallMat);
            whiteBallPoint = FindBall(whiteBallMat);
        }
        
        private void FindRedBall()
        {
            ScalarArray lower = new ScalarArray(new MCvScalar(160, 100, 50));
            ScalarArray upper = new ScalarArray(new MCvScalar(180, 255, 255));

            CvInvoke.InRange(hsvTableMat, lower, upper, redBallMat);
            redBallPoint = FindBall(redBallMat);
        }

        private void FindYellowBall()
        {
            ScalarArray lower = new ScalarArray(new MCvScalar(22, 93, 128));
            ScalarArray upper = new ScalarArray(new MCvScalar(45, 255, 255));

            CvInvoke.InRange(hsvTableMat, lower, upper, yellowBallMat);
            yellowBallPoint = FindBall(yellowBallMat);
        }

        private void FindHsv()
        {
            CvInvoke.GaussianBlur(originMat, originMat, new System.Drawing.Size(3, 3), 1);
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
