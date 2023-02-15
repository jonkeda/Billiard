using System.Numerics;
using Billiards.Base.Extensions;

namespace Billiards.Base.Physics.Engine3
{

    public class Ball
    {
        // friction values
        private float muS = 0.2f; // sliding friction
        private float muSP = 0.022f; // spinning friction
        private float muR = 0.008f; // rolling friction
        private float cushion_height = 0.010f; // measured from center ball

        // ball properties
        private static float R = 0.031f; //3,1 cm, ballradius
        private static float Rkw = R * R; // R^2
        private static float m = 0.21f; // 210 gram, ball mass
        private static float ballMOI = 0.4f * m * Rkw; // moment of intertia of a ball
        private static float g = 9.81f; // good old gravity

        // motion variables
        private Vector3 LinVel = new Vector3(0, 0, 0); // Lineair/Translational Velocity
        private Vector3 AngVel = new Vector3(0, 0, 0); // Angular/Rotational Velocity
        private Vector3 RelVel = new Vector3(0, 0, 0); // Marlow's Relative Velocity
        private Vector3 RelVelNorm = Vector3.Normalize(new(0, 0, 0));
        private Vector2 PosT; // Position of ball within tableframe
        private Vector2 PosB = new Vector2(0, 0); // Position of ball within ballframe
        private float psi = 0; //

        // others
        private static float keu_m = 0.5f; // gewicht van een normale keu

        // motion states
        public static byte CUESTRIKE = 0;
        public static byte SLIDING = 1; // relative velocity!=0, lin vel!=0
        public static byte ROLLING = 2; // relative velocity=0, lin vel!=0
        public static byte SPINNING = 3; // lin vel=0; ang.vel.Z!=0
        public static byte STATIONAIRY = 4; // linvel=0;ang.vel.Z=0
        private byte currentMotionState = STATIONAIRY;
        private byte previousMotionState = STATIONAIRY;

        public static string[] motionStates = new string[]
            {
            "cuestrike","sliding","rolling","spinning","stationary"
            };

        // events
        public static byte NO_EVENT = 0;
        public static byte SLIDINGEND = 1;
        public static byte ROLLINGEND = 2;
        public static byte SPINNINGEND = 3;
        public static byte CC = 4; // cushion collision
        public static byte UPPERCC = 5;
        public static byte RIGHTCC = 6;
        public static byte LOWERCC = 7;
        public static byte LEFTCC = 8;
        public static byte BC = 9; // ball collision
        public static byte MEE = 10;
        public static byte CONTRA = 11;
        public static string[] events = new string[]
               {"no event","end of sliding","end of rolling","end of spinning",
           "cushion collision","upper cushion","right cushion","lower cushion","left cushion","ball collision","mee effect","contra effect"};
        public static byte nOfCushionColisions = 0;

        // method of railcolision resolving
        private static byte HAN = 0;
        private static byte LB = 1;
        private static byte IDEALIZED = 2;
        private static byte OWN = 3;
        private byte brcMethod = HAN;

        private byte detectedEvent = NO_EVENT;
        private byte detectedCushion = NO_EVENT;
        float last_t;

        private float pi = (float)Math.PI;
        private List<string> log;
        private List<Byte> eventList;

        private bool checkBallColision = false;
        private List<Ball> otherBalls = null;

        private Ball lastDetectedBallColision = null;
        private byte lastDetectedBallNr;
        private byte lastCushionMeeEffect;

        //to store initial values
        private float o_keuVelocity;
        private float o_a;
        private float o_b;
        private float o_x;
        private float o_y;
        private float o_psi;
        private List<Ball> o_otherBalls = null;
        //(float keuVelocity, float a, float b, float x, float y, float psi, List<Ball> otherBalls)

        public bool debug = false;

        /*
         * @keuVelocity the velocity of the cue at the time of impact, in m/s
         * @a left/right spin, in mm (0=heart, >0=left english, <0=right english)
         * @b height of impact point, in mm (0=heart, <0=draw, >0=spin)
         * @x,y ball position in tableframe
         * @psi angle between ball direction and line parallel to long cushion
         */
        public Ball(float keuVelocity, float a, float b, float x, float y, float psi)
        {
            initialize(keuVelocity, a, b, x, y, psi);
        }

        public Ball(float keuVelocity, float a, float b, float x, float y, float psi, List<Ball> otherBalls)
        {
            this.otherBalls = otherBalls;
            checkBallColision = true;
            initialize(keuVelocity, a, b, x, y, psi);
        }

        public Ball(StootParameters sp, float x, float y, List<Ball> otherBalls)
        {
            this.otherBalls = otherBalls;
            o_otherBalls = (List<Ball>)otherBalls.clone();
            checkBallColision = true;
            initialize(sp.v, sp.a, sp.b, x, y, sp.psi);
        }

        public Ball getCopy()
        {
            return new Ball(o_keuVelocity, o_a, o_b, o_x, o_y, o_psi, o_otherBalls);
        }

        public Object Clone()
        {
            return new Ball(o_keuVelocity, o_a, o_b, o_x, o_y, o_psi, o_otherBalls);
        }

        private void initialize(float keuVelocity, float a, float b, float x, float y, float psi)
        {
            foundEventTime = -1f;
            log = new List<string>();
            eventList = new List<Byte>();
            o_keuVelocity = keuVelocity;
            o_a = a;
            o_b = b;
            o_x = x;
            o_y = y;
            o_psi = psi;
            float theta = 0f;
            float sintheta = (float)Math.Sin(theta);
            float costheta = (float)Math.Cos(theta);
            float stkw = sintheta * sintheta;
            float ctkw = costheta * costheta;
            float c = (float)Math.Abs(Math.Sqrt(R * R + a * a + b * b));
            float factor = 0f;
            float F = (2f * m * keuVelocity) /
              (1f + (m / keu_m) + ((5f / (2f * Rkw)) * factor *
              (a * a + b * b * ctkw + c * c * stkw - (2 * b * c * sintheta * costheta))));
            LinVel = new Vector3((F / m) * costheta, 0f, 0f);
            AngVel = new Vector3((a * F * sintheta) / ballMOI, (-c * F * sintheta + b * F * costheta) / ballMOI, (-a * F) / ballMOI);
            RelVel = LinVel.add(new Vector3(-AngVel.Y * R, AngVel.X * R, 0f));
            RelVelNorm = RelVel.normalize();
            currentMotionState = SLIDING;
            PosT = new Vector2(x, y);
            this.psi = psi;
            //log.Add("Init:\nF: "+F);
            //log.Add("a: "+a);
            //log.Add("b: "+b);
            last_t = 0;
            addVelToLog();
            nOfCushionColisions = 0;
        }

        public void addImpact(float velocity, float aai, float bai, float xai, float yai, float psiai)
        {
            foundEventTime = -1f;
            log = new List<string>();
            eventList = new List<Byte>();
            LinVel = new Vector3(velocity, 0f, 0f);
            AngVel = new Vector3(0f, (bai * velocity) / (0.4f * Rkw), (-aai * velocity) / (0.4f * Rkw));
            RelVel = LinVel.add(new Vector3(-AngVel.Y * R, AngVel.X * R, 0f));
            RelVelNorm = RelVel.normalize();
            currentMotionState = SLIDING;
            this.psi = psiai;
            last_t = 0;
            addVelToLog();
            nOfCushionColisions = 0;
        }

        private void addVelToLog()
        {
            //log.Add("t="+last_t);
            //log.Add("  LinVel: ("+LinVel.X+","+LinVel.Y+","+LinVel.Z+")");
            //log.Add("  AngVel: ("+AngVel.X+","+AngVel.Y+","+AngVel.Z+")");
            //log.Add("  RelVel: ("+RelVel.X+","+RelVel.Y+","+RelVel.Z+")");
            //log.Add("  RelVelNorm: ("+RelVelNorm.X+","+RelVelNorm.Y+","+RelVelNorm.Z+")");
            //log.Add("  psi: "+psi);
        }

        public void setPosTX(float x)
        {
            PosT.X = x;
        }
        public void setPosTY(float y)
        {
            PosT.Y = y;
        }

        public List<string> get_log()
        {
            return log;
        }

        public void setMu(float s, float r, float sp)
        {
            muS = s;
            muR = r;
            muSP = sp;
        }

        public float[] getMu()
        {
            return new float[] { muS, muR, muSP };
        }

        public void resetMu()
        {
            muS = 0.2f; // sliding friction
            muSP = 0.044f; // spinning friction
            muR = 0.016f; // rolling friction
        }

        public void setZdim(float zd)
        {
            zdim = zd;
        }

        public float getZdim()
        {
            return zdim;
        }

        public void resetZdim()
        {
            zdim = 1f;
        }

        public int getnOfCushionColisions()
        {
            return nOfCushionColisions;
        }

        public byte getDetectedEvent()
        {
            return detectedEvent;
        }

        public List<Byte> getEventList()
        {
            return eventList;
        }

        public Vector3 getLinVel()
        {
            return LinVel;
        }

        public bool isShotValid3c()
        {
            bool valid = false;
            byte ballCount = 0;
            byte cushionCount = 0;
            bool finished = false;
            foreach (Byte b in eventList)
            {
                if (b == BC)
                    ballCount++;
                if (b == CC)
                    cushionCount++;
                if (!finished && ballCount == 2)
                {
                    finished = true;
                    if (cushionCount >= 3) valid = true;
                }
            }
            return valid;
        }

        /* update motion state variables
         * @t time in seconds
         */
        public void updateMV(float t)
        {
            //log.Add("------------\nupdate at "+t);
            last_t = t;
            //		addVelToLog();
            //log.Add("Event detected for this ball: "+(t==foundEventTime));
            previousMotionState = currentMotionState;
            if (currentMotionState != STATIONAIRY)
            {
                //first update relative velocity, lineair velocity, angular velocity, and position within ballframe 
                if (currentMotionState == SLIDING)
                {
                    PosB.X = LinVel.X * t - 0.5f * muS * g * t * t * RelVelNorm.X;
                    PosB.Y = LinVel.Y * t - 0.5f * muS * g * t * t * RelVelNorm.Y;
                    LinVel = LinVel.subtract(RelVelNorm.mult(muS * g * t));
                    float scal = -((5 * muS * g) / (2 * R)) * t;
                    Vector3 ku = (new Vector3(0, 0, 1)).cross(RelVelNorm);
                    float ztemp = AngVel.Z;
                    AngVel = AngVel.subtract(ku.mult(scal));
                    float zs = getSign(ztemp);
                    AngVel.Z = ztemp - getSign(ztemp) * ((5f * muSP * g) / (2f * R)) * t;
                    if (getSign(AngVel.Z) != zs) AngVel.Z = 0;
                    //				Vector3 ka = (new Vector3(0,0,R)).cross(AngVel);
                    RelVel = RelVel.subtract(RelVelNorm.mult(3.5f * muS * g * t));
                    RelVelNorm = RelVel.normalize();
                    //				System.out.println("RELVEL: "+RelVel.X+","+RelVel.Y+","+RelVel.Z);
                    if (RelVel.Length() <= 0.00001f)
                    {
                        currentMotionState = ROLLING;
                        RelVel = RelVel.mult(0f);
                        RelVelNorm = RelVelNorm.mult(0f);
                    }
                }

                else if (currentMotionState == ROLLING)
                {
                    Vector3 lvnorm = LinVel.normalize();
				// out("lvnorm y: " + lvnorm.Y);
                    PosB.X = LinVel.X * t - 0.5f * muR * g * t * t * lvnorm.X;
                    PosB.Y = LinVel.Y * t - 0.5f * muR * g * t * t * lvnorm.Y;
                    LinVel = LinVel.subtract((LinVel.normalize()).mult(muR * g * t));
                    float ztemp = AngVel.Z;
                    AngVel.X = 0;
                    AngVel.Y = LinVel.X / R;
                    float zs = getSign(ztemp);
                    AngVel.Z = ztemp - getSign(ztemp) * ((5f * muSP * g) / (2f * R)) * t;
                    if (getSign(AngVel.Z) != zs) AngVel.Z = 0;

                    if (LinVel.Length() == 0)
                    { //to SPINNING or STATIONAIRY
                        if (AngVel.Z > 0)
                        {
                            //						System.out.println("to spinning!");
                            currentMotionState = SPINNING;
                        }
                        else
                        {
                            currentMotionState = STATIONAIRY;
                            //						System.out.println("to stationary!");
                        }
                    }
                }

                else if (currentMotionState == SPINNING)
                {
                    AngVel.Z = AngVel.Z - ((5 * muSP * g) / (2 * R)) * t;
                    if (AngVel.Z == 0) currentMotionState = STATIONAIRY;
                }
            }

            //calculate displacement within table frame
            float cospsi = (float)Math.Cos(psi);
            float sinpsi = (float)Math.Sin(psi);
            PosT.X += cospsi * PosB.X - sinpsi * PosB.Y;
            PosT.Y += sinpsi * PosB.X + cospsi * PosB.Y;
            PosB.X = 0;
            PosB.Y = 0;

            //update the following if one of these event occured at t
            if (currentMotionState != STATIONAIRY)
            {
                if (foundEventTime == t)
                {
                    //resolve colision
                    if (detectedEvent == CC)
                    {
                        nOfCushionColisions++;
                        resolveCushionColision();
                    }

                    if (detectedEvent == BC)
                    {
                        resolveBallColision();
                    }

                    if ((previousMotionState == SLIDING)
                        && (currentMotionState == ROLLING))
                    {
                        psi += (float)Math.Atan2((double)LinVel.Y, (double)LinVel.X);
                        LinVel.X = (float)Math.Sqrt(LinVel.X * LinVel.X + LinVel.Y * LinVel.Y);
                        LinVel.Y = 0;
                    }
                }
            }
            //log.Add("motion state after update: "+currentMotionState);
            //log.Add("end of update\n------------");
            detectedEvent = NO_EVENT;
        }

        private float calc_e(float vx)
        {
            float e = a + b * vx - c * vx * vx;
            //log.Add("e!:"+e);
            return e;
        }

        private float calc_mu(float theta)
        {
            return 0.471f - 0.241f * theta;
        }

        private byte colMode; //for debugging
        public byte getColMode() { return colMode; }

        float zdim = 1f;

        private void resolveHAN()
        {
            //		System.out.println("han collision");
            //log.Add("HAN collision\n-pre");
            //		addVelToLog();
            Vector2 v0 = new Vector2(0, 0);
            Vector2 new_v = new Vector2(0, 0);
            float spinV = AngVel.Z;
            float spinX = 0;
            float spinY = 0;
            float sinThA = cushion_height / 0.031f;
            float cosThA = (float)Math.Cos(Math.Asin(sinThA));
            float cospsi = (float)Math.Cos(psi);
            float sinpsi = (float)Math.Sin(psi);
            //ontleed volgens han, x = normal (always positive), y tangential to cushion (from left to right)
            if (detectedCushion == UPPERCC)
            {
                v0.X = -(sinpsi * LinVel.X + cospsi * LinVel.Y);
                v0.Y = (cospsi * LinVel.X - sinpsi * LinVel.Y);
                spinX = sinpsi * AngVel.X + cospsi * AngVel.Y;
                spinY = cospsi * AngVel.X - sinpsi * AngVel.Y;
            }
            else if (detectedCushion == LOWERCC)
            {
                v0.X = sinpsi * LinVel.X + cospsi * LinVel.Y;
                v0.Y = -(cospsi * LinVel.X - sinpsi * LinVel.Y);
                spinX = -(sinpsi * AngVel.X + cospsi * AngVel.Y);
                spinY = -(cospsi * AngVel.X - sinpsi * AngVel.Y);
            }
            else if (detectedCushion == LEFTCC)
            {
                v0.Y = -(sinpsi * LinVel.X + cospsi * LinVel.Y);
                v0.X = -(cospsi * LinVel.X - sinpsi * LinVel.Y);
                spinY = -(sinpsi * AngVel.X + cospsi * AngVel.Y);
                spinX = cospsi * AngVel.X - sinpsi * AngVel.Y;
            }
            else if (detectedCushion == RIGHTCC)
            {
                v0.Y = sinpsi * LinVel.X + cospsi * LinVel.Y;
                v0.X = cospsi * LinVel.X - sinpsi * LinVel.Y;
                spinY = sinpsi * AngVel.X + cospsi * AngVel.Y;
                spinX = -(cospsi * AngVel.X - sinpsi * AngVel.Y);
            }

            if (v0.Y * spinV > 0) lastCushionMeeEffect = CONTRA;
            else lastCushionMeeEffect = MEE;

            //geen idee waarom, maar mee effect werkt beter wanneer spinv en v0y
            //gespiegeld worden
            if (lastCushionMeeEffect == MEE)
            {
                v0.Y = -v0.Y;
                spinV = -spinV;
            }

            float e = calc_e(Math.Abs(v0.X));
            //			log.Add("pre-han converted:");
            //			log.Add("vy (tangential): "+v0.Y);
            //			log.Add("vx (tangential): "+v0.X);
            //			log.Add("spinx: "+spinX);
            //			log.Add("spiny: "+spinY);
            //			log.Add("spinz: "+spinV);
            //log.Add("lastCushionMeeEffect: "+events[lastCushionMeeEffect]);
            //log.Add("v0.Y*spinV: "+(v0.Y*spinV));

            //first determine case
            float sx0 = v0.X * sinThA + R * spinY;
            float sy0 = -v0.Y - R * spinV * cosThA + R * spinX * sinThA;
            float s0 = (new Vector2(sx0, sy0)).Length();
            float A = 7 / (2 * m);
            float Pzs = s0 / A;

            float c0 = v0.X * cosThA;
            float B = 1 / m;
            float Pze = (1 + e) * (Math.Abs(c0) / B);
            float Px = 0;
            float Py = 0;
            float Pz = 0;
            //log.Add("Pzs: "+Pzs);
            //log.Add("Pze: "+Pze);
            //log.Add("sx0,sy0: "+sx0+","+sy0);
            //log.Add("c0"+c0);
            if (Pzs <= Pze)
            { //case 1-1, sliding and sticking
                Px = -(sx0 / A) * sinThA - (1 + e) * (c0 / B) * cosThA;
                Py = sy0 / A;
                Pz = (sx0 / A) * cosThA - (1 + e) * (c0 / B) * sinThA;
                colMode = 0;
                //log.Add("Px: "+Px);
                //log.Add("Py: "+Py);
                //log.Add("Pz: "+Pz);
            }
            if (Pzs > Pze)
            {
                float theta = (float)Math.Atan2(v0.Y, v0.X);
                float mu = calc_mu(theta);
                Px = -mu * (1 + e) * (c0 / B) * (float)Math.Cos(theta) * sinThA - (1 + e) * (c0 / B) * cosThA;
                Py = mu * (1 + e) * (c0 / B) * (float)Math.Sin(theta);
                Pz = mu * (1 + e) * (c0 / B) * (float)Math.Cos(theta) * cosThA - (1 + e) * (c0 / B) * sinThA;
                colMode = 1;
                //log.Add("Px: "+Px);
                //log.Add("Py: "+Py);
                //log.Add("Pz: "+Pz);
            }
            new_v.X = v0.X + Px / m;
            new_v.Y = v0.Y + Py / m;

            spinV = spinV + zdim * ((R / ballMOI) * Py * cosThA);
            spinX = spinX - (R / ballMOI) * Py * sinThA;
            spinY = spinY + (R / ballMOI) * (Px * sinThA - Pz * cosThA);

            //			new_v.Y = -new_v.Y;
            //			new_v.X = -new_v.X;
            //			spinV = -spinV;
            //			spinX = -spinX;
            //			spinY = -spinY;

            if (lastCushionMeeEffect == MEE)
            {
                new_v.Y = -new_v.Y;
                spinV = -spinV;
            }

            //log.Add("post-han not yet converted:");
            //			log.Add("newy (tangential): "+new_v.Y);
            //			log.Add("newx (tangential): "+new_v.X);
            //			log.Add("spinx: "+spinX);
            //			log.Add("spiny: "+spinY);
            //			log.Add("spinz: "+spinV);

            //now transform v0.X and v0.Y into direction (psi) and magnitude(LinVel.X)
            LinVel.X = (float)Math.Sqrt(new_v.X * new_v.X + new_v.Y * new_v.Y);
            LinVel.Y = 0;
            if (detectedCushion == UPPERCC)
            {
                psi = (float)Math.Atan2(-(double)new_v.X, (double)new_v.Y);
                AngVel.X = (float)(Math.Sin(psi) * spinX + Math.Cos(psi) * spinY);
                AngVel.Y = (float)(Math.Cos(psi) * spinX - Math.Sin(psi) * spinY);
                AngVel.Z = spinV;
            }
            if (detectedCushion == LOWERCC)
            {
                psi = (float)Math.Atan2((double)new_v.X, -(double)new_v.Y);
                AngVel.X = -((float)(Math.Sin(psi) * spinX + Math.Cos(psi) * spinY));
                AngVel.Y = -((float)(Math.Cos(psi) * spinX - Math.Sin(psi) * spinY));
                AngVel.Z = spinV;
            }
            if (detectedCushion == LEFTCC)
            {
                psi = (float)Math.Atan2(-(double)new_v.Y, -(double)new_v.X);
                AngVel.Y = -((float)(Math.Sin(psi) * spinX + Math.Cos(psi) * spinY));
                AngVel.X = (float)(Math.Cos(psi) * spinX - Math.Sin(psi) * spinY);
                AngVel.Z = spinV;
            }
            if (detectedCushion == RIGHTCC)
            {
                psi = (float)Math.Atan2((double)new_v.Y, (double)new_v.X);
                AngVel.Y = (float)(Math.Sin(psi) * spinX + Math.Cos(psi) * spinY);
                AngVel.X = -((float)(Math.Cos(psi) * spinX - Math.Sin(psi) * spinY));
                AngVel.Z = spinV;
            }
            //log.Add("HAN collision\n-post");
            //			addVelToLog();
            RelVel = LinVel.add(new Vector3(-AngVel.Y * R, AngVel.X * R, 0f));
            RelVelNorm = RelVel.normalize();
            if (RelVel.Length() != 0) currentMotionState = SLIDING;
        }

        private void resolveOWN()
        {

        }

        private void resolveCushionColision()
        {
            if ((brcMethod == HAN))
            {
                resolveHAN();
            }
            else if (brcMethod == OWN)
            {
                resolveOWN();
            }
            else // perfect reflection
            {
                if ((detectedCushion == UPPERCC) || (detectedCushion == LOWERCC))
                {
                    psi = -psi;
                }
                if ((detectedCushion == LEFTCC) || (detectedCushion == RIGHTCC))
                {
                    psi = (float)Math.PI - psi;
                }
            }
        }

        public void setOtherBalls(List<Ball> ob)
        {
            otherBalls = ob;
        }

        private void resolveBallColision()
        {
            float x1 = PosT.X;
            float y1 = PosT.Y;
            log.Add("" + AngVel);
            Vector2 v1 = new Vector2((float)Math.Cos(psi) * LinVel.X, (float)Math.Sin(psi) * LinVel.X);
            float oldpsi = psi;

            Ball b = lastDetectedBallColision;
            //log.Add("---collided with ball---");

            float x2 = b.getPosT().X;
            float y2 = b.getPosT().Y;

            //Vector2 normal = new Vector2(x2-x1,y2-y1);
            Vector2 tangent = new Vector2(-(y2 - y1), x2 - x1).normalize();
            Vector2 ortho = new Vector2(x2 - x1, y2 - y1).normalize();

            LinVel.X = v1.dot(tangent);
            LinVel.X *= 0.98f; //coefficient of restitution...is this correct?
            psi = (float)Math.Atan2(tangent.Y, tangent.X);


            //update otherball
            float ox = b.getPosT().X;
            float oy = b.getPosT().Y;
            float ov = Math.Abs(v1.dot(ortho) * 0.98f);
            float opsi = (float)Math.Atan2(ortho.Y, ortho.X);
            b.addImpact(ov, 0f, 0f, ox, oy, opsi);

            //even de gecolleerde bal weghalen...
            otherBalls.Remove(b);

            //convert angular velocities
            float d_psi = oldpsi - psi;

            float cos_d_psi = (float)Math.Cos(d_psi);
            float sin_d_psi = (float)Math.Sin(d_psi);

            float old_av_x = AngVel.X;
            float old_av_y = AngVel.Y;

            AngVel.X = cos_d_psi * old_av_x - sin_d_psi * old_av_y;
            AngVel.Y = sin_d_psi * old_av_x + cos_d_psi * old_av_y;

            RelVel = LinVel.add(new Vector3(-AngVel.Y * R, AngVel.X * R, 0f));
            RelVelNorm = RelVel.normalize();
            if (RelVel.Length() != 0) currentMotionState = SLIDING;
            log.Add("" + AngVel + "---");
        }

        public Vector2 getPosT()
        {
            return PosT;
        }

        private float foundEventTime;

        /*
         * Find the time of the first occurring event
         */
        public float getTimeOfFirstEvent()
        {
            if (currentMotionState != STATIONAIRY)
            {
                foundEventTime = getFirstMotionTransition();
                if (currentMotionState != SPINNING)
                {
                    float t = getCushionColisionTime();
                    if ((t < foundEventTime) && (t > 0))
                    {
                        foundEventTime = t;
                        detectedEvent = CC;
                    }
                    if (checkBallColision)
                    {
                        t = getBallColisionTime();
                        if ((t < foundEventTime) && (t > 0))
                        {
                            foundEventTime = t;
                            detectedEvent = BC;
                        }
                    }
                }
            }
            else
            {
                foundEventTime = -1;
            }
            eventList.Add(detectedEvent);
            if (detectedEvent == CC)
            {
                eventList.Add(lastCushionMeeEffect);
                eventList.Add(detectedCushion);
            }
            if (detectedEvent == BC)
            {
                eventList.Add(lastDetectedBallNr);
            }
            return foundEventTime;
        }

/*        private void out(string s) {
		////System.out.println(s);	
	}
*/
    /*
	 * This only works during rolling state, or in sliding state,
	 * when the ball travels along a rectilinear path.
	 * This is done to prevent the need of a quartic solver.
	 * 
	 * 
	 * 
	 */
    private float getBallColisionTime()
    {
        float t = 1000f;
        float cospsi = (float)Math.Cos(psi);
        float sinpsi = (float)Math.Sin(psi);
        float mu = 0;
        float nrmx = 0;
        float nrmy = 0;
        Vector3 lvnorm = LinVel.normalize();

        if (currentMotionState == ROLLING)
        {
            mu = muR;
            nrmx = lvnorm.X;
            nrmy = lvnorm.Y;
        }

        if (currentMotionState == SLIDING)
        {
            mu = muS;
            nrmx = RelVelNorm.X;
            nrmy = RelVelNorm.Y;
        }
        byte ballcnt = 0;
        if (otherBalls != null)
            foreach(Ball b in otherBalls)
            {
                ballcnt++;
                //calculate distance to point of impact
                float d = (float)calculateDistanceToPOI(PosT.X, PosT.Y, (b.getPosT()).X, (b.getPosT()).Y);
                //			System.out.println("d to POI: "+d);
                if (d > 0)
                {
                    //PosB.X = LinVel.X*t-0.5f*muR*g*t*t*lvnorm.X;

                    float tmp = solveCubic(
                            -0.5f * mu * g * nrmx,
                            LinVel.X,
                            -d
                        );
                    if ((tmp > 0.00001f) && (tmp < t))
                    {
                        t = tmp;
                        lastDetectedBallColision = b;
                        lastDetectedBallNr = ballcnt;
                        calc_poi(d);
                    }
                }
            }
        return t;
    }

    /*point of impact coordinates for debugging*/
    private void calc_poi(float d)
    {
        poi = new Vector2(PosT.X + ((float)Math.Cos(psi)) * d,
                PosT.Y + ((float)Math.Sin(psi)) * d);
    }
    public Vector2 poi = new Vector2(0, 0);

    private double calculateDistanceToPOI(float x1, float y1, float x2, float y2)
    {
        double dist = -1;
        double a = x2 - x1;
        double b = y2 - y1;
        double c = Math.Sqrt(a * a + b * b);
        double alpha = Math.Atan2(b, a);
        double beta = Math.Abs(psi - alpha);
        double pi = Math.PI;
        if (beta > pi) beta = 2 * pi - beta;
        if (!((beta < -Math.PI / 2) || (beta > Math.PI / 2)))
        {
            double d = Math.Abs(c * (Math.Cos(beta)));
            double e = Math.Sqrt(c * c - d * d);
            double diam = TableProps.BALL_DIAM / 1000f;
            double f = 0;
            if (e <= diam)
            {
                f = (double)Math.Sqrt(diam * diam - e * e);
                dist = d - f;
            }
        }
        return dist;
    }

    private float getCushionColisionTime()
    {
        float t = 100f;
        float cospsi = (float)Math.Cos(psi);
        float sinpsi = (float)Math.Sin(psi);
        float mu = 0;
        float nrmx = 0;
        float nrmy = 0;
        Vector3 lvnorm = LinVel.normalize();

        if (currentMotionState == ROLLING)
        {
            mu = muR;
            nrmx = lvnorm.X;
            nrmy = lvnorm.Y;
        }

        if (currentMotionState == SLIDING)
        {
            mu = muS;
            nrmx = RelVelNorm.X;
            nrmy = RelVelNorm.Y;
        }

        //left cushion
        float tmp = solveCubic(
                -cospsi * 0.5f * mu * g * nrmx + sinpsi * 0.5f * mu * g * nrmy,
                cospsi * LinVel.X - sinpsi * LinVel.Y,
                PosT.X - R
            );
        //if (debug) System.out.println("left cushion at: " + tmp);
        if (tmp > 0.00001f)
        {
            t = tmp;
            detectedCushion = LEFTCC;
        }
        //right cushion
        tmp = solveCubic(
                -cospsi * 0.5f * mu * g * nrmx + sinpsi * 0.5f * mu * g * nrmy,
        cospsi * LinVel.X - sinpsi * LinVel.Y,
                PosT.X - ((float)TableProps.CLOTH_WIDTH) / 1000f + R
            );
        //if (debug) System.out.println("right cushion at: " + tmp);
        if ((tmp > 0.00001f) && (tmp < t))
        {
            t = tmp;
            detectedCushion = RIGHTCC;
        }
        //upper cushion
        tmp = solveCubic(
                -cospsi * 0.5f * mu * g * nrmy - sinpsi * 0.5f * mu * g * nrmx,
                cospsi * LinVel.Y + sinpsi * LinVel.X,
                PosT.Y - R
            );
        //if (debug) System.out.println("upper cushion at: " + tmp);
        if ((tmp > 0.00001f) && (tmp < t))
        {
            t = tmp;
            detectedCushion = UPPERCC;
        }
        //lower cushion
        tmp = solveCubic(
                -cospsi * 0.5f * mu * g * nrmy - sinpsi * 0.5f * mu * g * nrmx,
                cospsi * LinVel.Y + sinpsi * LinVel.X,
                PosT.Y - ((float)TableProps.CLOTH_HEIGHT) / 1000f + R
            );
        //if (debug) System.out.println("lower cushion at: " + tmp);
        if ((tmp > 0.00001f) && (tmp < t))
        {
            t = tmp;
            detectedCushion = LOWERCC;
        }
        return t;
    }

    public float solveCubic(float a, float b, float c)
    {
        float t = -1f;

        float discr = b * b - 4 * a * c;
        float discr_wortel = (float)Math.Sqrt(discr);
        if (discr == 0)
        {
            t = (-b + discr_wortel) / (2f * a);
        }
        else if (discr > 0)
        {
            float t1 = (-b + discr_wortel) / (2f * a);
            float t2 = (-b - discr_wortel) / (2f * a);
            if (t2 > 0)
                if ((t1 < t2) && (t1 > 0)) t = t1; else t = t2;
            else t = t1;
        }
        else
        {
            //System.out.println("Cubic solver: no real root!");
            t = -2.0f;
        }

        return t;
    }

    private float getSlidingDuration()
    {
        float t = 2f * RelVel.Length() /
            (7f * muS * g);
		//out("slidingD: " + t);
        return t;
    }

    private float getRollingDuration()
    {
        float t = LinVel.Length() /
            (muR * g);
        return t;
    }

    private float getSpinningDuration()
    {
        float t = 2f * R * Math.Abs(AngVel.Z) /
            (5f * muSP * g);
		//out("spd: " + t);
        return t;
    }

    private float getFirstMotionTransition()
    {
        getSpinningDuration();
        float t = float.PositiveInfinity;
        if (currentMotionState == SLIDING)
        { //kan naar rolling of stationairy
            float d = getSlidingDuration();
            if (d < t)
            {
                t = d;
                detectedEvent = SLIDINGEND;
            }
        }
        else if (currentMotionState == ROLLING)
        { //kan naar spinning of stationairy
            float d = getRollingDuration();
            if (d < t)
            {
                t = d;
                detectedEvent = ROLLINGEND;
            }
        }
        else if (currentMotionState == SPINNING)
        { //kan naar stationairy
            float d = getSpinningDuration();
            if (d < t)
            {
                t = d;
                detectedEvent = SPINNINGEND;
            }
        }
        else t = -1;
		// out("det: " + this.detectedEvent);
        return t;
    }

    float getSign(float f)
    {
        if (f < 0) return -1;
        if (f > 0) return 1;
        if (f == 0) return 0;
        return 0;
    }

    public byte getCurrentMotionState() { return this.currentMotionState; }

    /*
	 * predicts position according to previous motionstate 
	 */
    public Vector2 getPosAt(float t)
    {
        Vector2 retvec = new Vector2(0f, 0f);
        Vector2 PosTmp = new Vector2(0f, 0f);
        if (currentMotionState == SLIDING)
        {
            PosTmp.X = LinVel.X * t - 0.5f * muS * g * t * t * RelVelNorm.X;
            //			PosTmp.Y = LinVel.Y*t-0.5f*muS*g*t*t*RelVelNorm.Y;
            PosTmp.Y = LinVel.Y * t - 0.5f * muS * g * t * t * RelVelNorm.Y;
            float cospsi = (float)Math.Cos(psi);
            float sinpsi = (float)Math.Sin(psi);
            retvec.X = PosT.X + cospsi * PosTmp.X - sinpsi * PosTmp.Y;
            retvec.Y = PosT.Y + sinpsi * PosTmp.X + cospsi * PosTmp.Y;
            return retvec;
        }
        else if (currentMotionState == ROLLING)
        {
            Vector3 lvnorm = LinVel.normalize();
            Vector3 LinVelTmp = LinVel.clone();
            LinVelTmp = LinVelTmp.subtract((LinVelTmp.normalize()).mult(muR * g * t));
            PosTmp.X = LinVel.X * t - 0.5f * muR * g * t * t * lvnorm.X;
            PosTmp.Y = LinVel.Y * t - 0.5f * muR * g * t * t * lvnorm.Y;
            float cospsi = (float)Math.Cos(psi);
            float sinpsi = (float)Math.Sin(psi);
            retvec.X = PosT.X + cospsi * PosTmp.X - sinpsi * PosTmp.Y;
            retvec.Y = PosT.Y + sinpsi * PosTmp.X + cospsi * PosTmp.Y;
            return retvec;
        }
        else return PosT;
    }

    public float getZAt(float t)
    {
        float ztemp = AngVel.Z;
        float ret = 0;
        float zs = getSign(ztemp);
        ret = ztemp - getSign(ztemp) * ((5f * muSP * g) / (2f * R)) * t;
        if (getSign(ret) != zs) ret = 0;
        return ret;
    }

    public byte getMotionState()
    {
        return currentMotionState;
    }

    //  parameters according to [HAN05]:
    //	float a = 0.39f;
    //	float b = 0.257f;
    //	float c = 0.044f;

    float a = 0.85f;
    float b = 0.0f;
    float c = 0.0f;
    public void set_e(float a, float b, float c)
    {
        this.a = a; this.b = b; this.c = c;
    }
    public float[] get_e()
    {
        return new float[] { a, b, c };
    }

    public void reset_e()
    {
        a = 0.39f; b = 0.257f; c = 0.044f;
    }
    public void set_ch(float h)
    {
        cushion_height = h;
    }
    public void reset_ch()
    {
        cushion_height = 0.010f;
    }
    public float get_ch()
    {
        return cushion_height;
    }
    public void set_m(float m)
    {
        Ball.m = m;
    }
    public void reset_m()
    {
        m = 0.210f;
    }
    public float get_m()
    {
        return m;
    }
}
}
