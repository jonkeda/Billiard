using System.Collections.Generic;
using System.Drawing;
using Billiard.Camera.vision.algorithms;
using Billiard.Camera.vision.Geometries;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;

namespace Billiard.Camera.vision;

public class BilliardVisionEngine
{
    private BilliardVisionEngineState engineState; //= new BilliardVisionEngineState(this);

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

        this.VIDEO_IMAGE_SIZE = new Size(1200, 600);

        this.CORNER_DETECTION_IMAGE_SIZE = new Size(400, 200);

        this.TABLE_DETECTION_IMAGE_SIZE = new Size(400, 200);

        engineState = new BilliardVisionEngineState(this);

    }


    public void processFrame()
    {
        //startFrameTiming();

        CvInvoke.Resize(engineState.cameraMat, engineState.processMat, VIDEO_IMAGE_SIZE);

        findCorners();

        warpTablePerspective();

        detectBalls(engineState);

        engineState.table.inferBalls(engineState.detectedBallsInFrame);

        //stopFrameTiming();

    }

    void findCorners()
    {
        if (engineState.frameCounter % 25 == 0)
        {
            CvInvoke.Resize(engineState.processMat, engineState.smallMat, CORNER_DETECTION_IMAGE_SIZE);

            PointF pointOnTable = VisionAlgorithms.findSimilarPointOnCenterSpiral(engineState.smallMat);

            Mat mask = new Mat();

            Scalar newColor = Color.WHITE;

            Scalar diff = new Scalar(engineState.floodFillDiff, engineState.floodFillDiff, engineState.floodFillDiff);

            System.Drawing.Rectangle boundingRect = new Rectangle();
            //Rect boundingRect = new Rect();

            CvInvoke.FloodFill(engineState.smallMat, mask, pointOnTable.AsPoint(), newColor.AsMCvScalar(),
                out boundingRect, diff.AsMCvScalar(), diff.AsMCvScalar(), Connectivity.EightConnected
                    /* 8 | (255 << 8)*/ );


            PointF leftCornerSmall = VisionAlgorithms.findColorOnLine(engineState.smallMat, Color.WHITE, boundingRect.Height,
                boundingRect.X, boundingRect.Y, 0, 1);

            PointF rightCornerSmall = VisionAlgorithms.findColorOnLine(engineState.smallMat, Color.WHITE, boundingRect.Height,
                boundingRect.X + boundingRect.Width - 1, boundingRect.Y, 0, 1);

            PointF topCornerSmall = VisionAlgorithms.findColorOnLine(engineState.smallMat, Color.WHITE, boundingRect.Width,
                boundingRect.X, boundingRect.Y, 1, 0);

            PointF bottomCornerSmall = VisionAlgorithms.findColorOnLine(engineState.smallMat, Color.WHITE, boundingRect.Width,
                boundingRect.X, boundingRect.Y + boundingRect.Height - 1, 1, 0);


            if (!engineState.isForcedCorners)
            {
                engineState.table.backLeftCornerPoint.recordPoint(transformCoordinate(bottomCornerSmall,
                    engineState.smallMat, engineState.processMat));

                engineState.table.backRightCornerPoint.recordPoint(transformCoordinate(rightCornerSmall,
                    engineState.smallMat, engineState.processMat));

                engineState.table.frontLeftCornerPoint.recordPoint(transformCoordinate(leftCornerSmall,
                    engineState.smallMat, engineState.processMat));

                engineState.table.frontRightCornerPoint.recordPoint(transformCoordinate(topCornerSmall,
                    engineState.smallMat, engineState.processMat));

            }
        }
    }

    void detectBalls(BilliardVisionEngineState state)
    {
        CvInvoke.CvtColor(engineState.tableMat, engineState.binaryTableMat, ColorConversion.Bgr2Gray);

        CvInvoke.Canny(engineState.binaryTableMat, engineState.binaryTableMat, 50.0, 150.0, 3, true);


        Mat hierarchy = new Mat();

        VectorOfVectorOfPointF immutableContours = new VectorOfVectorOfPointF();


        CvInvoke.FindContours(engineState.binaryTableMat, immutableContours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxNone);

        List<VectorOfPointF> contours = new List<VectorOfPointF>();
        for (int i = 0; i < contours.Count; i++)
        {
            contours.Add(immutableContours[i]);
        }
        engineState.detectedBallsInFrame = new List<PointF>();

        bool continueWithSameObject = false;

        VectorOfPointF contour = null;

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
                VectorOfPointF comparsionContour = contours.ToArray()[contourIndex];
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


        MatOfPoint src = new MatOfPoint(
            table.frontRightCornerPoint.AsPoint(), 
            table.backRightCornerPoint.AsPoint(),
            table.backLeftCornerPoint.AsPoint(), 
            table.frontLeftCornerPoint.AsPoint()
        );


        MatOfPoint dest = needSideReverse
            ? new MatOfPoint(
                new PointF(0, 0), new PointF(TABLE_DETECTION_IMAGE_SIZE.Width, 0),
                new PointF(TABLE_DETECTION_IMAGE_SIZE.Width, TABLE_DETECTION_IMAGE_SIZE.Height),
                new PointF(0, TABLE_DETECTION_IMAGE_SIZE.Height)
            )
            : new MatOfPoint(
                new PointF(TABLE_DETECTION_IMAGE_SIZE.Width, TABLE_DETECTION_IMAGE_SIZE.Height),
                new PointF(TABLE_DETECTION_IMAGE_SIZE.Width, 0),
                new PointF(0, 0), new PointF(0, TABLE_DETECTION_IMAGE_SIZE.Height)
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