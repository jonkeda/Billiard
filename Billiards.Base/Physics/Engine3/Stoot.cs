using System.Numerics;
using Billiards.Base.Extensions;

namespace Billiards.Base.Physics.Engine3
{

    public class Stoot
    {
        static byte LINKSOM = 0;
        static byte RECHTSOM = 1;

        public Vector2 cb { get; set; }
        public Vector2 c1 { get; set; }
        public Vector2 c2 { get; set; }
        public Vector2 c3 { get; set; }
        public Vector2 c4 { get; set; }
        public Vector2 c5 { get; set; }
        public Vector2 hoekc1 { get; set; }
        public Vector2 hoekc2 { get; set; }
        public Vector2 hoekc3 { get; set; }
        public Vector2 hoekc4 { get; set; }
        public string naam { get; set; }
        byte richting;

        public Stoot()
        {
            cb = new Vector2(0f, 0f);
            c1 = new Vector2(0f, 0f);
            c2 = new Vector2(0f, 0f);
            c3 = new Vector2(0f, 0f);
            c4 = new Vector2(0f, 0f);
            c5 = new Vector2(0f, 0f);
            hoekc1 = new Vector2(0f, 0f);
            hoekc2 = new Vector2(0f, 0f);
            hoekc3 = new Vector2(0f, 0f);
            hoekc4 = new Vector2(0f, 0f);
            naam = "";
            richting = 0;
        }

        public void calcAngles()
        {
            Vector2 L1 = new Vector2(0, -1);
            Vector2 L2 = new Vector2(0, 1);
            Vector2 K1 = new Vector2(1, 0);
            Vector2 K2 = new Vector2(-1, 0);

            Vector2 inV = new Vector2(c1.X - cb.X, c1.Y - cb.Y);
            Vector2 outV = new Vector2(c1.X - c2.X, c1.Y - c2.Y);

            hoekc1 = new Vector2((float)MathF2.toDegrees(
                    Math.Acos(inV.dot(L1) / (inV.Length() * L1.Length()))),
             (float)MathF2.toDegrees(
                    Math.Acos(outV.dot(L1) / (outV.Length() * L1.Length()))));

            inV = new Vector2(c2.X - c1.X, c2.Y - c1.Y);
            outV = new Vector2(c2.X - c3.X, c2.Y - c3.Y);

            hoekc2 = new Vector2(
             (float)MathF2.toDegrees(
                    Math.Acos(inV.dot(K2) / (inV.Length() * K2.Length()))),
             (float)MathF2.toDegrees(
                    Math.Acos(outV.dot(K2) / (outV.Length() * K2.Length()))));

            inV = new Vector2(c3.X - c2.X, c3.Y - c2.Y);
            outV = new Vector2(c3.X - c4.X, c3.Y - c4.Y);

            hoekc3 = new Vector2(
             (float)MathF2.toDegrees(
                    Math.Acos(inV.dot(L2) / (inV.Length() * L2.Length()))),
             (float)MathF2.toDegrees(
                    Math.Acos(outV.dot(L2) / (outV.Length() * L2.Length()))));

            Vector2 B = new Vector2();
            //welke band?
            if (c4.Y == 0.031f)
                B = L1;
            else
                B = K1;
            inV = new Vector2(c4.X - c3.X, c4.Y - c3.Y);
            outV = new Vector2(c4.X - c5.X, c4.Y - c5.Y);

            hoekc4 = new Vector2(
             (float)MathF2.toDegrees(
                    Math.Acos(inV.dot(B) / (inV.Length() * B.Length()))),
             (float)MathF2.toDegrees(
                    Math.Acos(outV.dot(B) / (outV.Length() * B.Length()))));
        }

        public string tostring()
        {
            return naam;
        }
    }
}
