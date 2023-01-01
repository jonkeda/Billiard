using System.Drawing;

namespace Billiard.Camera.vision
{
    public class ShortTermSpot
    {
        private const int DEFAULT_LIVE_TIME = 500;
        private PointF tablePoint;
        private int framesToLive;

        public ShortTermSpot(PointF tablePoint, int timeToLive = DEFAULT_LIVE_TIME)
        {
/*            if (tablePoint == null)
            {
                tablePoint = new PointF(200, 100);
            }
*/            framesToLive = timeToLive;
            this.tablePoint = tablePoint;
        }

        public void decreaseLivingTime()
        {
            if (framesToLive > 0)
                framesToLive--;
        }

        bool isAlive()
        {
            return framesToLive > 0;
        }
    }
}
