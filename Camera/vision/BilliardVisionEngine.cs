using System.Collections.Generic;
using System.Drawing;
using Billiard.Camera.vision.algorithms;
using Billiard.Camera.vision.Geometries;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace Billiard.Camera.vision;

public class BilliardVisionEngine
{
    internal BilliardVisionEngineState engineState; //= new BilliardVisionEngineState(this);

    private Size VIDEO_IMAGE_SIZE;

    private Size CORNER_DETECTION_IMAGE_SIZE;

    private Size TABLE_DETECTION_IMAGE_SIZE;


    private long frameStartedEpoch;


    public BilliardVisionEngine(
        /*            Size VIDEO_IMAGE_SIZE = new Size(ConfigurationProperties.readInt('Input_video_width'), 
                        600 ConfigurationProperties.readInt('Input_video_height'));
                    Size CORNER_DETECTION_IMAGE_SIZE = new Size(ConfigurationProperties.readInt('Corner_detection_image_width'),
                                                                 * ConfigurationProperties.readInt('Corner_detection_image_height')),
                    Size TABLE_DETECTION_IMAGE_SIZE = new Size(ConfigurationProperties.readInt('Table_detection_image_width'),
                        ConfigurationProperties.readInt('Table_detection_image_height'))*/
        )
    {

        VIDEO_IMAGE_SIZE = new Size(1200, 600);

        CORNER_DETECTION_IMAGE_SIZE = new Size(400, 200);

        TABLE_DETECTION_IMAGE_SIZE = new Size(400, 200);

        engineState = new BilliardVisionEngineState(this);

    }


    public void processFrame(Mat image)
    {
        //startFrameTiming();

        // CvInvoke.Resize(engineState.cameraMat, engineState.processMat, VIDEO_IMAGE_SIZE);

        CvInvoke.Resize(image, engineState.processMat, image.Size);
        //CvInvoke.Resize(image, engineState.processMat, VIDEO_IMAGE_SIZE);

        findCorners();

        warpTablePerspective();

        detectBalls2();
        /*
                engineState.table.inferBalls(engineState.detectedBallsInFrame);
        */
        //stopFrameTiming();

    }

    void findCorners()
    {
        if (engineState.frameCounter % 25 == 0)
        {

            CvInvoke.Resize(engineState.processMat, engineState.floodfillMat, engineState.processMat.Size);

            PointF pointOnTable = VisionAlgorithms.findSimilarPointOnCenterSpiral(engineState.floodfillMat);

            Mat mask = new Mat();

            MCvScalar newColor = Color.WHITE.AsMCvScalar();

            MCvScalar diff = new MCvScalar(engineState.floodFillDiff, engineState.floodFillDiff, engineState.floodFillDiff);

            Rectangle boundingRect;
            CvInvoke.FloodFill(engineState.floodfillMat, mask, pointOnTable.AsWindowsPoint(), newColor,
                out boundingRect, diff, diff, Connectivity.EightConnected
                    /* 8 | (255 << 8)*/ );


            PointF leftCornerSmall = VisionAlgorithms.findColorOnLine(engineState.floodfillMat, newColor, boundingRect.Height,
                boundingRect.X, boundingRect.Y, 0, 1);

            PointF rightCornerSmall = VisionAlgorithms.findColorOnLine(engineState.floodfillMat, newColor, boundingRect.Height,
                boundingRect.X + boundingRect.Width - 1, boundingRect.Y, 0, 1);

            PointF topCornerSmall = VisionAlgorithms.findColorOnLine(engineState.floodfillMat, newColor, boundingRect.Width,
                boundingRect.X, boundingRect.Y, 1, 0);

            PointF bottomCornerSmall = VisionAlgorithms.findColorOnLine(engineState.floodfillMat, newColor, boundingRect.Width,
                boundingRect.X, boundingRect.Y + boundingRect.Height - 1, 1, 0);


            if (!engineState.isForcedCorners)
            {
                engineState.table.backLeftCornerPoint.recordPoint(transformCoordinate(bottomCornerSmall,
                    engineState.floodfillMat, engineState.processMat));

                engineState.table.backRightCornerPoint.recordPoint(transformCoordinate(rightCornerSmall,
                    engineState.floodfillMat, engineState.processMat));

                engineState.table.frontLeftCornerPoint.recordPoint(transformCoordinate(leftCornerSmall,
                    engineState.floodfillMat, engineState.processMat));

                engineState.table.frontRightCornerPoint.recordPoint(transformCoordinate(topCornerSmall,
                    engineState.floodfillMat, engineState.processMat));

            }
        }
    }

    void detectBalls2()
    {
        CvInvoke.CvtColor(engineState.tableMat, engineState.grayTableMat, ColorConversion.Bgr2Gray);

        float cannyThreshold = 180.0f;
        float cannyThresholdLinking = 120.0f;
        CvInvoke.GaussianBlur(engineState.grayTableMat, engineState.grayTableMat, new Size(3, 3), 1);
        CvInvoke.Canny(engineState.grayTableMat, engineState.binaryTableMat, cannyThreshold, cannyThresholdLinking);

        CvInvoke.CvtColor(engineState.tableMat, engineState.hsvTableMat, ColorConversion.Bgr2Hsv);
        CvInvoke.ExtractChannel(engineState.hsvTableMat, engineState.hTableMat, 0);
        CvInvoke.ExtractChannel(engineState.hsvTableMat, engineState.sTableMat, 1);
        CvInvoke.ExtractChannel(engineState.hsvTableMat, engineState.vTableMat, 2);

        CircleF[] circles = detectCircles(engineState.binaryTableMat);
    }

    CircleF[] detectCircles(Mat gray)
    {
        float cannyThreshold = 180.0f;
        /*
            Mat cannyEdges = new Mat();
            float cannyThresholdLinking = 120.0f;
            CvInvoke.GaussianBlur(gray, gray, new System.Drawing.Size(3, 3), 1);
            CvInvoke.Canny(gray, cannyEdges, cannyThreshold, cannyThresholdLinking);
        */
        // Mat gray = new Mat();
        float circleAccumulatorThreshold = 1;
        CircleF[] circles = CvInvoke.HoughCircles(gray, HoughModes.GradientAlt, 2.0, 20.0, cannyThreshold,
            circleAccumulatorThreshold, 0);

        Mat hierarchy = null; // new Mat();

        VectorOfVectorOfPoint immutableContours = new VectorOfVectorOfPoint();
        CvInvoke.FindContours(engineState.binaryTableMat, immutableContours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxNone);

        return circles;
    }


    void detectBalls()
    {
        CvInvoke.CvtColor(engineState.tableMat, engineState.binaryTableMat, ColorConversion.Bgr2Gray);

        CvInvoke.Canny(engineState.binaryTableMat, engineState.binaryTableMat, 50.0, 150.0, 3, true);


        Mat hierarchy = null; // new Mat();

        VectorOfVectorOfPoint immutableContours = new VectorOfVectorOfPoint();


        CvInvoke.FindContours(engineState.binaryTableMat, immutableContours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxNone);

        List<VectorOfPoint> contours = new List<VectorOfPoint>();
        for (int i = 0; i < immutableContours.Size; i++)
        {
            if (immutableContours[i] != null)
                contours.Add(immutableContours[i]);
        }
        engineState.detectedBallsInFrame = new List<Point>();

        bool continueWithSameObject = false;

        VectorOfPoint contour = null;

        while (contours.Count > 0)
        {
            if (!continueWithSameObject)
            {
                contour = contours[^1];
            }

            continueWithSameObject = false;
            // Optimization: if this contour is already too big ditch it
            //if (VisionAlgorithms.getRadiusOptimizedWithMaxValue(contour, engineState.ballMaxRadius) >= engineState.ballMaxRadius) continue
            for (int contourIndex = 0; contourIndex < contours.Count; contourIndex++)
            {
                VectorOfPoint comparsionContour = contours.ToArray()[contourIndex];
                // Optimization: if this contour is already too big ditch it 
                //if (VisionAlgorithms.getRadiusOptimizedWithMaxValue(contour, engineState.ballMaxRadius) >= engineState.ballMaxRadius) continue
                if (VisionAlgorithms.isContoursOfSameObject(contour.AsList(), comparsionContour.AsList(), engineState.maxContourDistanceWithSameObject))
                {
                    contour = comparsionContour;
                    //contour.AddRange(comparsionContour);

                    continueWithSameObject = true;

                    contours.Remove(comparsionContour);

                    contourIndex--;

                }
            }
            if (!continueWithSameObject || contours.size() == 0)
            {
                float contourRadius = GeometricMath.getRadiusOptimizedWithMaxValue(contour, engineState.ballMaxRadius);

                if (contourRadius > engineState.ballMinRadius && contourRadius < engineState.ballMaxRadius)
                    engineState.detectedBallsInFrame.Add(GeometricMath.getGeometricAverage(contour.AsList()));

            }
        }
    }


    PointF transformCoordinate(PointF sourceCordinate, Mat sourceCordinateSystem, Mat targetCordinateSystem)
    {
        float x = sourceCordinate.X * ((float)targetCordinateSystem.Rows) /
                   ((float)sourceCordinateSystem.Rows);

        float y = sourceCordinate.Y * ((float)targetCordinateSystem.Cols) /
                   ((float)sourceCordinateSystem.Cols);

        return new PointF(x, y);

    }

    void warpTablePerspective()
    {
        Table table = engineState.table;
        bool needSideReverse = table.tableNeedSideReverse();
        VectorOfPointF
            src = new VectorOfPointF(new[]
                {
                    table.frontRightCornerPoint.AsPoint(),
                    table.backRightCornerPoint.AsPoint(),
                    table.backLeftCornerPoint.AsPoint(),
                    table.frontLeftCornerPoint.AsPoint()
                });
        VectorOfPointF dest = needSideReverse
                ? new VectorOfPointF(new[]
                    {
                        new PointF(0, 0),
                        new PointF(TABLE_DETECTION_IMAGE_SIZE.Width, 0),
                        new PointF(TABLE_DETECTION_IMAGE_SIZE.Width, TABLE_DETECTION_IMAGE_SIZE.Height),
                        new PointF(0, TABLE_DETECTION_IMAGE_SIZE.Height)
                    }
                )
                : new VectorOfPointF(
                    new[]
                    {
                        new PointF(TABLE_DETECTION_IMAGE_SIZE.Width, TABLE_DETECTION_IMAGE_SIZE.Height),
                        new PointF(TABLE_DETECTION_IMAGE_SIZE.Width, 0),
                        new PointF(0, 0),
                        new PointF(0, TABLE_DETECTION_IMAGE_SIZE.Height)
                    }
                );
        Mat warpingMat = CvInvoke.GetPerspectiveTransform(src, dest);
        CvInvoke.WarpPerspective(engineState.processMat, engineState.tableMat, warpingMat, TABLE_DETECTION_IMAGE_SIZE);
    }


    /*    private void startFrameTiming()
        {
            long frameStartedEpoch = System.currentTimeMillis();

        }

        private void stopFrameTiming()
        {
            engineState.frameProcessingTimeInMillis = System.currentTimeMillis() - frameStartedEpoch;

            engineState.frameCounter++;

        }*/
}