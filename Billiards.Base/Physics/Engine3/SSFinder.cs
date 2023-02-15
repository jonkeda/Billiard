using System.Numerics;
using Billiards.Base.Extensions;

namespace Billiards.Base.Physics.Engine3
{

    public class SSFinder
    {
        private List<ShotSolution> allSolutions;

        private List<Cluster> clusters;

        //private Table table;
        private Ball white;
        private Ball yellow;
        private Ball red;

        private static bool showall = true;
        private static bool showcombined = true;

        private float muS = 0.2f;
        private float muR = 0.008f;
        private float muSP = 0.022f;

        private float ea = 0.85f;
        private float eb = 0f;
        private float ec = 0f;

        public SSFinder()
        {
            //table = new Table(500);
        }

        public SSFinder(float muS, float muR, float muSP)
        {
            //table = new Table(500);
            this.muS = muS;
            this.muR = muR;
            this.muSP = muSP;
        }



        public SSFinder(float muS, float muR, float muSP, float ea, float eb, float ec)
        {
            //table = new Table(500);
            this.muS = muS;
            this.muR = muR;
            this.muSP = muSP;

            this.ea = ea;
            this.eb = eb;
            this.ec = ec;
        }

        public void setMu(float muS, float muR, float muSP)
        {
            this.muS = muS;
            this.muR = muR;
            this.muSP = muSP;
        }

        public void set_e(float ea, float eb, float ec)
        {
            this.ea = ea;
            this.eb = eb;
            this.ec = ec;
        }

        private List<Ball> otherballs;
        //private HTMLMaker out;

        /*
         * Returns an List with solutions, in the form of shotsolutions
         */
        public List<ShotSolution> getSolutions(Ball white, Ball yellow, Ball red)
        {
            List<ShotSolution> solutionList = new List<ShotSolution>();

            this.white = white;
            this.yellow = yellow;
            this.red = red;

            clusters = new List<Cluster>();
            allSolutions = new List<ShotSolution>();

            int resolutionLos = 360;
            int resolutionVanOB1 = 90;

            //System.out.println("yellow: " + yellow.getPosT());
            //System.out.println("red: " + red.getPosT());
            //System.out.println("white: " + white.getPosT());

            otherballs = new List<Ball>();
            otherballs.Add(yellow);
            otherballs.Add(red);

            Vector2 pos = white.getPosT();
            float a = 0.01f;
            float b = 0f;
            float psi = 0f;
            float v = 0;
            float pi = (float)Math.PI;

            float default_v = 2f;
            float its = 3;

            //first if desirable look for cushion first solutions
            float dx = red.getPosT().X - yellow.getPosT().X;
            float dy = red.getPosT().Y - yellow.getPosT().Y;
            double d_ry = Math.Sqrt(dx * dx + dy * dy);
            byte ob1 = 0;
            v = default_v;
            if (d_ry < 0.25f)
            {
                for (int i = 0; i < its; i++)
                {
                    a = 0.005f;
                    searchShots(-pi, pi, resolutionLos, v, a, b, pos, true, 0, ob1);
                    a = -0.005f;
                    searchShots(-pi, pi, resolutionLos, v, a, b, pos, true, 0, ob1);
                    v += 0.5f;
                }
            }

            // next the yellow ball from one extreme to the other in 180 steps
            double bb = TableProps.BALL_DIAM / 1000;
            dx = white.getPosT().X - yellow.getPosT().X;
            dy = white.getPosT().Y - yellow.getPosT().Y;
            double d_wy = Math.Sqrt(dx * dx + dy * dy);
            float d_psi = Math.Abs((float)Math.Atan2(bb, d_wy));
            //float step = d_psi/(resolutionVanOB1/2);
            psi = (float)Math.Atan2(-dy, -dx);

            ob1 = 1;
            v = default_v;
            for (int i = 0; i < its; i++)
            {

                //rechts effect
                a = 0.005f;
                searchShots(psi - d_psi, psi + d_psi, resolutionVanOB1, v, a, b, pos, false, (float)d_wy, ob1);
                //links effect
                a = -0.005f;
                searchShots(psi - d_psi, psi + d_psi, resolutionVanOB1, v, a, b, pos, false, (float)d_wy, ob1);
                v += 0.5f;
            }

            bb = TableProps.BALL_DIAM / 1000;
            dx = white.getPosT().X - red.getPosT().X;
            dy = white.getPosT().Y - red.getPosT().Y;
            d_wy = Math.Sqrt(dx * dx + dy * dy);
            d_psi = Math.Abs((float)Math.Atan2(bb, d_wy));
            //float step = d_psi/(resolutionVanOB1/2);
            psi = (float)Math.Atan2(-dy, -dx);

            ob1 = 2;
            v = default_v;
            for (int i = 0; i < its; i++)
            {
                //rechts effect
                a = 0.005f;
                searchShots(psi - d_psi, psi + d_psi, resolutionVanOB1, v, a, b, pos, false, (float)d_wy, ob1);
                //links effect
                a = -0.005f;
                searchShots(psi - d_psi, psi + d_psi, resolutionVanOB1, v, a, b, pos, false, (float)d_wy, ob1);
                v += 0.5f;
            }

            clusters = sortClusters(getClusters());
            //System.out.println("clusters: " + clusters.size());
            foreach (Cluster c in clusters)
                solutionList.Add(c.getRepSolution());
            //System.out.println("sols: " + solutionList.size());
            return solutionList;
        }

        public void solve(Ball white, Ball yellow, Ball red, int dircnt)
        {
            // long startT = (new Date()).getTime();

            this.white = white;
            this.yellow = yellow;
            this.red = red;

            clusters = new List<Cluster>();
            allSolutions = new List<ShotSolution>();

            int resolutionLos = 360;
            int resolutionVanOB1 = 90;

            /*            System.out.println("yellow: " + yellow.getPosT());
                        System.out.println("red: " + red.getPosT());
                        System.out.println("white: " + white.getPosT());
            */
            otherballs = new List<Ball>();
            otherballs.Add(yellow);
            otherballs.Add(red);

            //out = new HTMLMaker("" + dircnt);

            Vector2 pos = white.getPosT();
            float a = 0.01f;
            float b = 0f;
            float psi = 0f;
            float v = 0;
            float pi = (float)Math.PI;

            float default_v = 3f;

            //first if desirable look for cushion first solutions
            float dx = red.getPosT().X - yellow.getPosT().X;
            float dy = red.getPosT().Y - yellow.getPosT().Y;
            double d_ry = Math.Sqrt(dx * dx + dy * dy);
            byte ob1 = 0;
            v = default_v;
            if (d_ry < 0.25f)
            {
                for (int i = 0; i < 1; i++)
                {
                    a = 0.005f;
                    searchShots(-pi, pi, resolutionLos, v, a, b, pos, true, 0, ob1);
                    a = -0.005f;
                    searchShots(-pi, pi, resolutionLos, v, a, b, pos, true, 0, ob1);
                    v += 0.5f;
                }
            }

            // next the yellow ball from one extreme to the other in 180 steps
            double bb = TableProps.BALL_DIAM / 1000;
            dx = white.getPosT().X - yellow.getPosT().X;
            dy = white.getPosT().Y - yellow.getPosT().Y;
            double d_wy = Math.Sqrt(dx * dx + dy * dy);
            float d_psi = Math.Abs((float)Math.Atan2(bb, d_wy));
            //float step = d_psi/(resolutionVanOB1/2);
            psi = (float)Math.Atan2(-dy, -dx);

            ob1 = 1;
            v = default_v;
            for (int i = 0; i < 1; i++)
            {

                //rechts effect
                a = 0.005f;
                searchShots(psi - d_psi, psi + d_psi, resolutionVanOB1, v, a, b, pos, false, (float)d_wy, ob1);
                //links effect
                a = -0.005f;
                searchShots(psi - d_psi, psi + d_psi, resolutionVanOB1, v, a, b, pos, false, (float)d_wy, ob1);
                v += 0.5f;
            }

            bb = TableProps.BALL_DIAM / 1000;
            dx = white.getPosT().X - red.getPosT().X;
            dy = white.getPosT().Y - red.getPosT().Y;
            d_wy = Math.Sqrt(dx * dx + dy * dy);
            d_psi = Math.Abs((float)Math.Atan2(bb, d_wy));
            //float step = d_psi/(resolutionVanOB1/2);
            psi = (float)Math.Atan2(-dy, -dx);

            ob1 = 2;
            v = default_v;
            for (int i = 0; i < 1; i++)
            {
                //rechts effect
                a = 0.005f;
                searchShots(psi - d_psi, psi + d_psi, resolutionVanOB1, v, a, b, pos, false, (float)d_wy, ob1);
                //links effect
                a = -0.005f;
                searchShots(psi - d_psi, psi + d_psi, resolutionVanOB1, v, a, b, pos, false, (float)d_wy, ob1);
                v += 0.5f;
            }

            /*            long endT = (new Date()).getTime();
                    out.addPar("generation time(ms): " + (endT - startT));
                        outputAllClustersToHTML();

                    out.save();
            */
        }

        /* play all shot in range minpsi..maxpsi in steps steps
         * 
         */
        private void searchShots(float minpsi, float maxpsi, int steps, float v, float a, float b, Vector2 pos, bool cf,
            float dToOB1, byte ob1)
        {
            ShotSolution solution;
            float psi = minpsi;
            float v_o = v;
            float step = (maxpsi - minpsi) / steps;
            Ball ball;
            for (int i = 0; i < steps; i++)
            {
                psi += step;
                //			v = v_o+(i/(float)steps);
                ball = new Ball(v, a, b, pos.X, pos.Y, psi, (List<Ball>)otherballs.clone());
                ball.setMu(muS, muR, muSP);
                ball.set_e(ea, eb, ec);
                if (isvalid(ball))
                {

                    solution = new ShotSolution(v, a, b, psi);
                    //store characteristics of solution, to use later for clustering and sorting

                    solution.cushionFirst = cf;
                    if (!cf)
                        solution.thickness =
                            1 - ((float)Math.Abs(steps / 4 -
                                                  Math.Abs(steps / 2 - i)) / (float)(steps / 4));
                    if (!cf)
                        solution.distanceToOb1 = dToOB1;
                    if (!cf)
                        solution.thicknessabs =
                            1 - (float)i / (float)steps;

                    //store first cushion relative spin direction
                    List<Byte> el = lastEventList;
                    int j = 0;
                    while (!((el[j] == Ball.MEE) || (el[j] == Ball.CONTRA))) j++;
                    //				System.out.println(Ball.events[el[j]]);
                    if (el[j] == Ball.MEE) solution.contraFirstCushion = false;
                    if (el[j] == Ball.CONTRA) solution.contraFirstCushion = true;

                    //store cushionorder
                    j = 0;
                    int bc = 0;
                    foreach (Byte e in el)
                    {
                        if ((bc < 2) && (j < 6))
                        {
                            if ((e > 4) && (e < 9))
                            {
                                solution.first6Cushions[j] = e;
                                j++;
                            }

                            if (e == Ball.BC) bc++;
                        }
                    }

                    //find and set ob1
                    bool found = false;
                    for (int ii = 0; ii < el.Count; ii++)
                    {
                        if (el[ii] == Ball.BC)
                        {
                            solution.ob1 = el[ii + 1];
                            found = true;
                        }

                        if (found) break;
                    }

                    //check for kisses
                    checkKissOld(solution);

                    //set ob1 member
                    solution.ob1 = ob1;

                    if ((solution.contraFirstCushion == false)
                        && (solution.shortestDistance > 0.06))
                        allSolutions.Add(solution);
                }
            }
        }

        private List<Byte> lastEventList;

        private bool isvalid(Ball ball)
        {
            int overflow = 0;
            while ((ball.getCurrentMotionState() != Ball.STATIONAIRY) && (overflow < 1000))
            {
                float t = ball.getTimeOfFirstEvent();
                ball.updateMV(t);
                overflow++;
            }

            if (overflow > 900)
            {
                /*                System.out.println("isvalid: overflow");
                                System.out.println("pos: " + ball.getPosT().X + ", " + ball.getPosT().Y);
                */
                return false;
            }

            lastEventList = ball.getEventList();
            return ball.isShotValid3c();
        }

        /* This private method finds the closest distance of OB1
         * with the cue ball or OB2, during the shot (before a valid shot is established)
         * Not 100% sure this code is correct
         * 
         */
        private void checkKissOld(ShotSolution sol)
        {
            //table.cls();

            Ball y = new Ball(0f, 0f, 0f, yellow.getPosT().X, yellow.getPosT().Y, 0f);
            Ball r = new Ball(0f, 0f, 0f, red.getPosT().X, red.getPosT().Y, 0f);

            y.setMu(muS, muR, muSP);
            y.set_e(ea, eb, ec);
            r.setMu(muS, muR, muSP);
            r.set_e(ea, eb, ec);

            List<Ball> ob = new List<Ball>();
            ob.Add(y);
            ob.Add(r);
            //		System.out.println("v: "+sol.v+" a: "+sol.a+" b: "+sol.b+" psi: "+sol.psi);
            Ball w = new Ball(sol.v, sol.a, sol.b, white.getPosT().X, white.getPosT().Y, sol.psi,
                (List<Ball>)otherballs.clone());

            w.setMu(muS, muR, muSP);
            w.set_e(ea, eb, ec);
            w.setOtherBalls(ob);


            int res = 100;
            byte OB2moving = 0;
            float t;
            float shortestDistance = 100f;

            while (
                !((w.getCurrentMotionState() == 4)
                  && (y.getCurrentMotionState() == 4)
                  && (r.getCurrentMotionState() == 4)
                    ) && (!(w.isShotValid3c())))
            {
                List<float> times = new List<float>();
                t = w.getTimeOfFirstEvent();
                if (t > 0) times.Add(t);
                t = y.getTimeOfFirstEvent();
                if (t > 0) times.Add(t);
                t = r.getTimeOfFirstEvent();
                if (t > 0) times.Add(t);
                if (times.Count > 0)
                {
                    t = times.Order().First();
                    /*                    Collections.sort(times);
                                        t = times.get(0);
                    */
                }
                else t = -1;

                if (t >= 0)
                {
                    for (int i = 0; i <= (int)(t * res); i++)
                    {
                        float tt = ((float)i) / ((float)res);
                        Vector2 pw = w.getPosAt(tt);
                        Vector2 py = y.getPosAt(tt);
                        Vector2 pr = r.getPosAt(tt);
                        if (OB2moving == 1)
                        {
                            //yellow is moving
                            //first check colision between yellow and red
                            float dist = py.subtract(pr).Length();
                            if (dist < shortestDistance) shortestDistance = dist;
                            //then check colision between white and yellow
                            dist = py.subtract(pw).Length();
                            if (dist < shortestDistance) shortestDistance = dist;
                        }

                        if (OB2moving == 2)
                        {
                            //red is moving
                            //first check colision between yellow and red
                            float dist = pr.subtract(py).Length();
                            if (dist < shortestDistance) shortestDistance = dist;
                            //then check colision between white and red
                            dist = pr.subtract(pw).Length();
                            if (dist < shortestDistance) shortestDistance = dist;
                        }
                    }
                }

                r.updateMV(t);
                y.updateMV(t);

                if ((OB2moving == 0) && (w.getDetectedEvent() == Ball.BC))
                {
                    w.updateMV(t);
                    if (y.getLinVel().Length() > 0) OB2moving = 1; //yellow is ob2
                    else OB2moving = 2; //red is ob2
                }
                else
                {
                    w.updateMV(t);
                }
            }

            sol.shortestDistance = shortestDistance;
        }

        /* This private method finds the closest distance of OB1
         * with the cue ball or OB2, during the shot (before a valid shot is established)
         * 
         */
        private void checkKiss(ShotSolution sol)
        {
            //table.cls();

            Ball y = new Ball(0f, 0f, 0f, yellow.getPosT().X, yellow.getPosT().Y, 0f);
            Ball r = new Ball(0f, 0f, 0f, red.getPosT().X, red.getPosT().Y, 0f);

            y.setMu(muS, muR, muSP);
            y.set_e(ea, eb, ec);
            r.setMu(muS, muR, muSP);
            r.set_e(ea, eb, ec);

            List<Ball> ob = new List<Ball>();
            ob.Add(y);
            ob.Add(r);
            //		System.out.println("v: "+sol.v+" a: "+sol.a+" b: "+sol.b+" psi: "+sol.psi);
            Ball w = new Ball(sol.v, sol.a, sol.b, white.getPosT().X, white.getPosT().Y, sol.psi,
                (List<Ball>)otherballs.clone());

            w.setMu(muS, muR, muSP);
            w.set_e(ea, eb, ec);
            w.setOtherBalls(ob);


            int res = 100;
            byte OB2moving = 0;
            float t;
            float shortestDistance = 100f;

            while (
                !((w.getCurrentMotionState() == 4)
                  && (y.getCurrentMotionState() == 4)
                  && (r.getCurrentMotionState() == 4)
                    ) && !w.isShotValid3c())
            {
                List<float> times = new List<float>();
                t = w.getTimeOfFirstEvent();
                if (t > 0) times.Add(t);
                t = y.getTimeOfFirstEvent();
                if (t > 0) times.Add(t);
                t = r.getTimeOfFirstEvent();
                if (t > 0) times.Add(t);
                if (times.Count() > 0)
                {
                    t = times.Order().FirstOrDefault();
                    /*                    Collections.sort(times);
                                        t = times.get(0);
                    */
                }
                else t = -1;

                if (t >= 0)
                {
                    for (int i = 0; i <= (int)(t * res); i++)
                    {
                        float tt = ((float)i) / ((float)res);
                        Vector2 pw = w.getPosAt(tt);
                        Vector2 py = y.getPosAt(tt);
                        Vector2 pr = r.getPosAt(tt);
                        if (OB2moving > 0)
                        {
                            //yellow is moving
                            //first check colision between yellow and red
                            float dist = py.subtract(pr).Length();
                            if (dist < shortestDistance) shortestDistance = dist;
                            //then check colision between white and yellow
                            dist = py.subtract(pw).Length();
                            if (dist < shortestDistance) shortestDistance = dist;
                            //then check colision between white and red
                            dist = pr.subtract(pw).Length();
                            if (dist < shortestDistance) shortestDistance = dist;
                        }
                    }
                }

                r.updateMV(t);
                y.updateMV(t);

                if ((OB2moving == 0) && (w.getDetectedEvent() == Ball.BC))
                {
                    w.updateMV(t);
                    //				if (y.getLinVel().Length()>0) OB2moving = 1;//yellow is ob2
                    //				else OB2moving = 2;//red is ob2
                    OB2moving = 1;
                }
                else
                {
                    w.updateMV(t);
                }
            }

            sol.shortestDistance = shortestDistance;
        }

        /* Clusters solution according to the method described in 2.Xs 
         * 
         */
        private List<Cluster> getClusters()
        {
            List<List<ShotSolution>> tmp = new List<List<ShotSolution>>();
            List<Cluster> ret = new List<Cluster>();

            //split all solutions into clusters
            tmp.Add(allSolutions);
            tmp = splitIntoClusters(tmp);
            foreach (List<ShotSolution> ssList in tmp)
                ret.Add(new Cluster(ssList));
            return ret;
        }

        private List<List<ShotSolution>> splitIntoClusters(List<List<ShotSolution>> clusters)
        {
            List<List<ShotSolution>> ret = new List<List<ShotSolution>>();

            //init tree, [ob][c1][c2][c3]
            Cluster[,,,,,] solutionTree = new Cluster[3,5,5,5,5,2];
            for (int i = 0; i < 3; i++)
                for (int i1 = 0; i1 < 5; i1++)
                    for (int i2 = 0; i2 < 5; i2++)
                        for (int i3 = 0; i3 < 5; i3++)
                            for (int i4 = 0; i4 < 5; i4++)
                                for (int i5 = 0; i5 < 2; i5++)
                                    solutionTree[i,i1,i2,i3,i4,i5] = new Cluster(new List<ShotSolution>());
            //populate tree
            foreach (List<ShotSolution> cluster in clusters)
                foreach (ShotSolution sol in cluster)
                {
                    int a = sol.ob1;
                    int b = sol.first6Cushions[0] - 5;
                    int c = sol.first6Cushions[1] - 5;
                    int d = sol.first6Cushions[2] - 5;
                    int e = sol.first6Cushions[3] - 5;
                    int f = 0;
                    if (sol.a > 0f) f = 1;
                    solutionTree[a,b,c,d,e,f].add(sol);
                }

            //flatten tree
            for (int i = 0; i < 3; i++)
                for (int i1 = 0; i1 < 5; i1++)
                    for (int i2 = 0; i2 < 5; i2++)
                        for (int i3 = 0; i3 < 5; i3++)
                            for (int i4 = 0; i4 < 5; i4++)
                                for (int i5 = 0; i5 < 2; i5++)
                                    if (solutionTree[i,i1,i2,i3,i4,i5].getSize() > 0)
                                    {
                                        List<ShotSolution> ssl = solutionTree[i,i1,i2,i3,i4,i5].getSolutions();
                                        foreach (ShotSolution sol in ssl)
                                            sol.nInGroup = ssl.Count();
                                        ret.Add(ssl);
                                    }

            return ret;
        }

        private void outputAllClustersToHTML()
        {
            clusters = sortClusters(getClusters());
            Ball ball;
            Vector2 pos = white.getPosT();
            /*                out.addPar("white: " + pos.X + ", " + pos.Y);
                            out.addPar("yellow: " + yellow.getPosT().X + ", " + yellow.getPosT().Y);
                            out.addPar("red: " + red.getPosT().X + ", " + red.getPosT().Y);
                            out.addPar(pos.X + "," + pos.Y + "," + yellow.getPosT().X + "," + yellow.getPosT().Y + red.getPosT().X +
                                       "," + red.getPosT().Y);
            */
            foreach (Cluster cluster in clusters)
            {
                /*              printStats(cluster);
                */
                foreach (ShotSolution sol2 in cluster.getSolutions())
                {
                    ball = new Ball(sol2.v, sol2.a, sol2.b, pos.X, pos.Y, sol2.psi, (List<Ball>)otherballs.clone());
                    ball.setMu(muS, muR, muSP);
                    ball.set_e(ea, eb, ec);
                    getShot(ball, sol2.a, sol2.v);
                }
                // out.addImage(table.getCurrentImage());
                // table.cls();
            }
        }

        /*        private void printStats(Cluster cluster)
                {
                    int nokiss = 0;
                    for (ShotSolution sol:
                    cluster.getSolutions())
                    {
                        if (sol.shortestDistance < 0.061) nokiss++;
                    }
                    out.addPar("solutions in cluster: " + cluster.getSize());
                        out.addPar("kisses in cluster: " + nokiss);
                        out.addPar("cluster score: " + cluster.getScore());
                        out.addPar("LKL: " + cluster.isLKL());
                        out.addPar("Thickness: " + cluster.getThickness());
                        out.addPar("ob2: " + cluster.getOB2());
                }*/

        private List<Cluster> sortClusters(List<Cluster> clusters)
        {
            //dit moet allemaal even wat fraaier nog...gebruik Cluster in hele klasse bv
            List<Cluster> clustersRet = new List<Cluster>();
            //first collections are sorted based upon cluster size, see Cluster.compareTo
            clusters.Sort();
            //Collections.sort(clusters);
            int n = clusters.Count;
            if (n > 0)
            {
                int size_last = clusters[0].getSize();
                foreach (Cluster clu in
                clusters)
                {
                    if (size_last != clu.getSize()) n--;
                    size_last = clu.getSize();
                    float score = n;
                    if (clu.isLKL()) score += 2;
                    if (clu.getThickness() < 0.05f) score -= 1;
                    if (clu.getOB2() == 2) score += 0.1f;
                    clu.setScore(score);
                }
            }

            //now clusters are sorted based on score
            clusters.Sort();
            //Collection.sort(clusters);
            return clusters;
        }

        private void getShot(Ball w, float a, float v)
        {
/*            table.drawBall(white, Color.WHITE);
            table.drawBall(yellow, Color.YELLOW);
            table.drawBall(red, Color.RED);
*/            Ball y = new Ball(0f, 0f, 0f, yellow.getPosT().X, yellow.getPosT().Y, 0f);
            Ball r = new Ball(0f, 0f, 0f, red.getPosT().X, red.getPosT().Y, 0f);

            y.setMu(muS, muR, muSP);
            y.set_e(ea, eb, ec);
            r.setMu(muS, muR, muSP);
            r.set_e(ea, eb, ec);

            List<Ball> ob = new List<Ball>();
            ob.Add(y);
            ob.Add(r);

            w.setMu(muS, muR, muSP);
            w.set_e(ea, eb, ec);
            w.setOtherBalls(ob);

            int res = 100;
            float t;
            //		System.out.println("v: "+(v/4f));
/*            Color c = Color.white;
            if (a > 0)
                c = new Color(0f, 0f, ((v - 2f) / 2f));
            if (a <= 0)
                c = new Color(((v - 2f) / 2f), 0f, 0f);
*/
            while (
                !((w.getCurrentMotionState() == 4)
                //				&&(y.getCurrentMotionState()==4)
                //				&&(r.getCurrentMotionState()==4)
                ))
            {
                List<float> times = new List<float>();
                t = w.getTimeOfFirstEvent();
                if (t > 0) times.Add(t);
                //				t = y.getTimeOfFirstEvent(); if (t>0) times.Add(t);
                //				t = r.getTimeOfFirstEvent(); if (t>0) times.Add(t);
                if (times.Count > 0)
                {
                    t = times.Order().First();
/*                    Collections.sort(times);
                    t = times.get(0);
*/                }
                else t = -1;

                if (t >= 0)
                {
                    for (int i = 0; i <= (int)(t * res); i++)
                    {
                        float tt = ((float)i) / ((float)res);
                        Vector2 pw = w.getPosAt(tt);
                        //table.drawDot(pw.X, pw.Y, c);
                    }
                }

                //			r.updateMV(t);
                //			y.updateMV(t);
                w.updateMV(t);
                if (w.isShotValid3c()) break;
            }

            //return table.getCurrentImage();
        }
    }

    class Cluster : IComparable
    {

        private List<ShotSolution> ssList;

        private float score;

        public Cluster(List<ShotSolution> sslist)
        {
            ssList = sslist;
            score = -100;
        }

        public ShotSolution getRepSolution()
        {
            return ssList[getSize() / 2];
        }

        public List<ShotSolution> getSolutions()
        {
            return ssList;
        }

        public void add(ShotSolution sol)
        {
            ssList.Add(sol);
        }

        public float getScore()
        {
            return score;
        }

        public void setScore(float s)
        {
            score = s;
        }

        public bool isLKL()
        {
            if (ssList.Count > 0) return ssList[0].isLKL();
            else return false;
        }

        public bool roodLaatst()
        {

            return false;
        }

        public int getSize()
        {
            return ssList.Count;
        }

        public byte getOB2()
        {
            byte ret = 0;
            if (getSize() > 0)
            {
                ret = (byte)(3 - (ssList[getSize() / 2].ob1));
            }

            return ret;
        }

        public float getThickness()
        {
            float ret = 0;
            if (getSize() > 0)
            {
                ret = ssList[getSize() / 2].thickness;
            }

            return ret;
        }

        public int CompareTo(object? o)
        {
            if (score == -100)
            {
                Cluster clu = (Cluster)o;
                return -(this.getSize() - clu.getSize());
            }
            else
            {
                Cluster clu = (Cluster)o;
                float d = clu.getScore() - this.getScore();
                if (d == 0) return 0;
                else if (d < 0) return -1;
                return 1;
            }
        }

    }

}