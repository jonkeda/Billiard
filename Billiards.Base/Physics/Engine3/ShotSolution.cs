namespace Billiards.Base.Physics.Engine3
{

    public class ShotSolution : IComparable
    {
        // shot parameters
        public float psi { get; }
        public float v { get; }
        public float a { get; }
        public float b { get; }

        // heuristics scores
        public int nInGroup;
        public float distanceToOb1;
        public bool cushionFirst;
        float distanceToCushion; //if cushionfirst
        public float thickness; //1 = half, 0 = extreme thin/thick
        public float thicknessabs; //absoluut, handig voor visualisatie
        public bool contraFirstCushion;
        //nearest distance between OB1 and CB/OB2 to predict kiss danger
        public float shortestDistance;
        public Byte[] first6Cushions;
        int clusterScore;
        /*
         * ob1 is to indicate which ball is hit first, or 0 for a cushion first shot
         * ob1 = 1 is yellow first
         * ob1 = 2 is red first
         */
        public byte ob1;


        public ShotSolution(float v, float a, float b, float psi)
        {
            this.v = v;
            this.a = a;
            this.b = b;
            this.psi = psi;

            nInGroup = 0;
            distanceToOb1 = 0;
            cushionFirst = false;
            distanceToCushion = 0f;
            thickness = 0;
            contraFirstCushion = false;
            shortestDistance = 0f;
            first6Cushions = new Byte[6];
            for (int i = 0; i < 6; i++) first6Cushions[i] = 9;
            ob1 = 0;
            clusterScore = 0;
        }

        public bool isLKL()
        {
            if (first6Cushions.Length < 3) return false;
            else return ((isL(first6Cushions[0])) &&
                        (isK(first6Cushions[1])) &&
                        (isL(first6Cushions[2])));
        }

        public bool isL(byte cushion)
        {
            return (cushion == Ball.UPPERCC || cushion == Ball.LOWERCC);
        }
        public bool isK(byte cushion)
        {
            return (cushion == Ball.RIGHTCC || cushion == Ball.LEFTCC);
        }

        public float calcConfidence()
        {
            return 0f;
        }

        public string tostring()
        {
            string ret = "";
            if (!cushionFirst) ret = "a: " + a + " b: " + b + " v: " + v + " psi: " + psi +
            " nInCluster: " + nInGroup + " distancetoOB1: " + distanceToOb1 + " thickness: " + thickness + " contra1st: " + contraFirstCushion + " shortestD: " + shortestDistance + "\n score:" + this.getScore() + "c1: " + Ball.events[first6Cushions[0]] + " c2: " + Ball.events[first6Cushions[1]] + " c3: " + Ball.events[first6Cushions[2]] + " ob1: " + ob1 + "absthick: " + thicknessabs + "clusterScore: " + clusterScore + "isLKL: " + isLKL();
            if (cushionFirst) ret = "a: " + a + " b: " + b + " v: " + v + " psi: " + psi +
            " nInCluster: " + nInGroup + " distancetoCushion: " + distanceToOb1 + " thickness: " + thickness + " contra1st: " + contraFirstCushion + " shortestD: " + shortestDistance + "\n score:" + this.getScore() + "c1: " + Ball.events[first6Cushions[0]] + " c2: " + Ball.events[first6Cushions[1]] + " c3: " + Ball.events[first6Cushions[2]] + " ob1: " + ob1 + "absthick: " + thicknessabs + "clusterScore: " + clusterScore + "isLKL: " + isLKL();

            return ret;
        }

        public float getScore()
        {
            float ret = 0;
            ret += 2f * thickness;
            if (contraFirstCushion) ret -= 2.0f;
            float dOB1val = 1f - Math.Abs(0.3f - distanceToOb1);
            if (dOB1val > 0)
                ret += dOB1val;
            ret += nInGroup / 10f;
            ret += (5f - v);
            return ret;
        }

        public int CompareTo(object? o)
        {
            int ret = 0;
            ShotSolution ob = (ShotSolution)o;
            float d = this.getScore() - ob.getScore();
            if (d > 0) ret = -1;
            if (d < 0) ret = 1;
            return ret;
        }

    }
}
