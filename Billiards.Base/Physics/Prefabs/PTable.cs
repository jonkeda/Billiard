namespace Billiards.Base.Physics.Prefabs
{
    public class PTable : PStaticObject
    {
        public float Length { get; } = 2000; // * 0.9f; // 1800; // 1200;
        public float Width { get; } = 1000;// * 0.9f; // 900;
        public float BallRadius { get; } = BallRadiusConst;

        public const float BallRadiusConst = 30 * 0.9f;
    }
}
