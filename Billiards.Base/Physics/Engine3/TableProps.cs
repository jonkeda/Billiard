namespace Billiards.Base.Physics.Engine3
{
    public sealed class TableProps
    {
        //const double CLOTH_WIDTH = 2820; //in mm., afmetingen spelvlak matchbiljart volgens KNBB SAR
        //const double CLOTH_HEIGHT = 1410;

        public const double CLOTH_WIDTH = 2300; //in mm., afmetingen spelvlak 'klein' biljart volgens KNBB SAR
        public const double CLOTH_HEIGHT = 1150;

        public const double DIKTE_BAND = 50;
        public const double DIKTE_LIJST = 80;
        public const double DLDB = DIKTE_BAND + DIKTE_LIJST;

        public const double TABLE_HEIGHT = CLOTH_HEIGHT + 2 * DIKTE_BAND + 2 * DIKTE_LIJST;
        public const double TABLE_WIDTH = CLOTH_WIDTH + 2 * DIKTE_BAND + 2 * DIKTE_LIJST;

        public const byte WHITE = 0;
        public const byte YELLOW = 1;
        public const byte RED = 2;

        public const double BALL_DIAM = 62;
        public const double BALL_RADIUS = 31;
    }
}
