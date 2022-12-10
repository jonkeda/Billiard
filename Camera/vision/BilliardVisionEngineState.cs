using System;
using System.Collections.Generic;
using System.Drawing;
using Billiard.Camera.vision.Geometries;
using Emgu.CV;

namespace Billiard.Camera.vision
{
    internal class BilliardVisionEngineState
    {
        public BilliardVisionEngineState(BilliardVisionEngine billiardVisionEngine)
        {
            this.billiardVisionEngine = billiardVisionEngine;
        }

        public BilliardVisionEngine billiardVisionEngine;

        //long timeTotal = 0

        // OPTIONS / DEFAULT VALUES
        public float floodFillDiff = 7.5f;
        public int sleepBetweenFrames = 0;
        public float ballMinRadius = 10;
        public float ballMaxRadius = 40;

        public float maxContourDistanceWithSameObject = 5;
        public Mat cameraMat = new Mat();
        public Mat processMat = new Mat();
        public Mat smallMat = new Mat();
        public Mat tableMat = new Mat();
        public Mat binaryTableMat = new Mat();
        
        public bool isForcedCorners = false;	// TODO: when detection works (near) perfectly, we do not need this anymore

        public Table table = new Table();
        public List<PointF> detectedBallsInFrame = new List<PointF>();

        public int frameCounter = 0;
        public int frameProcessingTimeInMillis;
    }
}
