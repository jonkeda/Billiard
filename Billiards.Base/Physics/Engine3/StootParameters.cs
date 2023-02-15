namespace Billiards.Base.Physics.Engine3
{
    public class StootParameters
    {
        public float psi { get; }
        public float a { get; }
        public float b { get; }
        public float v { get; }
        public float theta { get; }

        public StootParameters(float v, float a, float b, float psi, float theta)
        {
            this.v = v;
            this.a = a;
            this.b = b;
            this.psi = psi;
            this.theta = theta;
        }
    }
}
