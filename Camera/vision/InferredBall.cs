using Emgu.CV.CvEnum;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Billiard.Camera.vision.algorithms;
using Billiard.Camera.vision.Geometries;

namespace Billiard.Camera.vision
{
    internal class InferredBall
    {

        private const int SAMPLE_FRAME_RATE = 3 * 25;

        public const int MOVEMENT_MAX_DISTANCE = (int)(2100 * 0.9f);
        // (ConfigurationProperties.read('Table_detection_image_width') as int) / 10;

        private const float STILL_MAX_DISTANCE = MOVEMENT_MAX_DISTANCE / 1.5f;

        private const int MIN_FRAMES_FOR_RECOGNIZATION = 25;

        private const float FRAME_CONFIRMATION_PERCENTAGE = 0.25f;


        public CappedQueue<PointF> lastPoints = new CappedQueue<PointF>(InferredBall.SAMPLE_FRAME_RATE);


        public bool isStillInPreviousFrame = false;

        public bool isStillInCurrentFrame = false;


        bool isLikelyRecognizedAsBall(float frameConfirmationPercentage = FRAME_CONFIRMATION_PERCENTAGE)
        {
            return lastPoints.size() > MIN_FRAMES_FOR_RECOGNIZATION &&
                   getValidPoints().Count / (lastPoints.size()) > frameConfirmationPercentage;
        }

        public bool isStill()
        {
            List<PointF> p = getValidPoints();

            for (int i = 0; i < p.Count; i++)
            {
                for (int j = i + 1; j < p.Count; j++)
                {
                    if (GeometricMath.distance(p[i], p[j]) > STILL_MAX_DISTANCE)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool startMovingNow()
        {
            return !isStillInCurrentFrame && isStillInPreviousFrame; //&& isLikelyRecognizedAsBall()

        }

        public bool isNewPointPossible(PointF newPoint)
        {
            return GeometricMath.distance(newPoint, getLastValidPoint()) < InferredBall.MOVEMENT_MAX_DISTANCE;

        }

        public bool exists()
        {
            return lastPoints.elements.Any();
        }

        public PointF getAveragePoint()
        {
            List<PointF> points = getValidPoints();

            return points.size() > 0 ? GeometricMath.getGeometricAverage(points) : new PointF(0, 0);

        }

        public PointF getLastValidPoint()
        {
            if (lastPoints.elements.Count == 0)
            {
                return new PointF(0, 0);
            }
            return lastPoints.elements[^1];

        }

        List<PointF> getValidPoints()
        {
            return lastPoints.elements;
            ;
        }


    }
}
