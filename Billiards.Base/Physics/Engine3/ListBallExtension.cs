namespace Billiards.Base.Physics.Engine3
{
    public static class ListBallExtension
    {
        public static List<Ball> clone(this List<Ball> balls)
        {
            List<Ball> newBalls = new List<Ball>();
            newBalls.AddRange(balls);
            return newBalls;
        }
    }
}
