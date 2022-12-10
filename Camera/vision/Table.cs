using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Billiard.Camera.vision.algorithms;

namespace Billiard.Camera.vision
{
    internal class Table
    {
        public InferredTableCornerPoint backRightCornerPoint = new();

        public InferredTableCornerPoint backLeftCornerPoint = new();

        public InferredTableCornerPoint frontRightCornerPoint = new();

        public InferredTableCornerPoint frontLeftCornerPoint = new();

        private List<InferredBall> balls = new();

        private List<ShortTermSpot> shortTermSpots = new();


        public void inferBalls(List<PointF> detectedBallsInFrame)
        {
            foreach (InferredBall ball in balls)
            {
                ball.isStillInPreviousFrame = ball.isStill();
            }
            updateInferredBalls(detectedBallsInFrame);
            foreach (InferredBall ball in balls)
            {
                ball.isStillInCurrentFrame = ball.isStill();
            }
            removeNonExistingBalls();
            fadeShortTermSpots();
        }

        void updateInferredBalls(List<PointF> detectedBallsInFrame)
        {
            List<InferredBall> matchedBalls = new List<InferredBall>();
            // todo
/*            foreach (PointF detectedBallInFrame in detectedBallsInFrame)
            {
                InferredBall nearestBall = (balls - matchedBalls).min { GeometricMath.distance(detectedBallInFrame, it.lastValidPoint) }
                if (isBallMovementPlausible(nearestBall, detectedBallInFrame))
                {
                    nearestBall.lastPoints.elements.Add(detectedBallInFrame);
                    matchedBalls.Add(nearestBall);
                }
                else
                {
                    InferredBall newBall = new InferredBall();
                    newBall.lastPoints.elements.Add(detectedBallInFrame);
                    balls.Add(newBall);
                }
            }
            (balls - matchedBalls).each { it.lastPoints << null }
*/        }

        void removeNonExistingBalls()
        {
            balls.Clear();
        }

        void fadeShortTermSpots()
        {
            foreach (InferredBall ball in balls)
            {
                if (ball.startMovingNow())
                {
                    shortTermSpots.Add(new ShortTermSpot(ball.getAveragePoint()));
                }
            }
            shortTermSpots.ForEach(it => it.decreaseLivingTime());
            shortTermSpots.Clear();
        }

        private bool isBallMovementPlausible(InferredBall nearestBall, PointF detectedBallInFrame)
        {
            if (nearestBall == null)
                return false;

            else
            {
                float distanceToNearest = GeometricMath.distance(nearestBall.getLastValidPoint(), detectedBallInFrame);

                return distanceToNearest < InferredBall.MOVEMENT_MAX_DISTANCE;

            }
        }

        bool isOngoingTurn()
        {
            return balls.All(it => it.isStill());
            ;

        }

        public bool tableNeedSideReverse()
        {
            return
                GeometricMath.distance(backLeftCornerPoint.AsPoint(), backRightCornerPoint.AsPoint()) <
                GeometricMath.distance(backLeftCornerPoint.AsPoint(), frontLeftCornerPoint.AsPoint());

        }
    }
}
