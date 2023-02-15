using System.Numerics;

namespace Billiards.Base.Physics.Engine3
{

    public class GeomSolver
    {
        private List<Stoot> stootList;
        private Ball white;
        private Ball yellow;

        public GeomSolver(Ball w, Ball y)
        {
            white = w;
            yellow = y;
            stootList = new List<Stoot>();
            generate3C();
        }

        void generate3C()
        {
            addStoot(0, 3);
            addStoot(0, -3);
            addStoot(-3, 0);
            addStoot(3, 0);
            for (int i = 1; i < 3; i++)
            {
                int j = 3 - i;
                addStoot(i, j);
                addStoot(i, -j);
                addStoot(-i, -j);
                addStoot(-i, j);
            }
        }

        void addStoot(int x, int y)
        {
            float x_t = yellow.getPosT().X - 0.031f;
            float y_t = yellow.getPosT().Y - 0.031f;
            float x_o = x_t;
            float y_o = y_t;
            float tw = (((float) TableProps.CLOTH_WIDTH) / 1000f) - 0.062f;
            float th = (((float) TableProps.CLOTH_HEIGHT) / 1000f) - 0.062f;

            if (x % 2 == 0)
            {
                x_t = x_t + x * tw;
            }
            else
            {
                if (x < 0)
                {
                    x_t = x_t + (x + 1) * tw;
                    x_t = x_t - 2 * x_o;
                }
                else
                {
                    x_t = x_t + (x - 1) * tw;
                    x_t = x_t + 2 * (tw - x_o);
                }
            }

            if (y % 2 == 0)
            {
                y_t = y_t + y * th;
            }
            else
            {
                if (y < 0)
                {
                    y_t = y_t + ((y + 1) * th);
                    y_t = y_t - 2 * y_o;
                }
                else
                {
                    y_t = y_t + (y - 1) * th;
                    y_t = y_t + 2 * (th - y_o);
                }
            }

            Stoot s = new Stoot();
            float xw = white.getPosT().X;
            float yw = white.getPosT().Y;
            s.cb = new Vector2(xw, yw);
            float vx = x_t - xw;
            float vy = y_t - yw;
            //		System.out.println(
            //				x+", "+y+"\n"+
            //				"dx: "+vx+"\n"+
            //				"dy: "+vy+"\n"+
            //				"x_o: "+x_o+"\n"+
            //				"y_o: "+y_o+"\n"+
            //				"x_t: "+x_t+"\n"+
            //				"y_t: "+y_t				
            //		);
            for (int i = 0; i < 5; i++)
            {
                float t1 = 1000;
                if (vx > 0)
                    t1 = (tw - xw) / vx;
                float t2 = 1000;
                if (vx < 0)
                    t2 = (xw) / -vx;
                float t3 = 1000;
                if (vy > 0)
                    t3 = (th - yw) / vy;
                float t4 = 1000;
                if (vy < 0)
                    t4 = (yw) / -vy;
                float[] t_arr = new float[] {t1, t2, t3, t4};
                Array.Sort(t_arr);
                //Arrays.sort(t_arr);
                float t = t_arr[0];
                xw += vx * t;
                yw += vy * t;

                if (t == t1) vx = -vx;
                if (t == t2) vx = -vx;
                if (t == t3) vy = -vy;
                if (t == t4) vy = -vy;
                //System.out.println(t_arr[0]+","+t_arr[1]+","+t_arr[2]+","+t_arr[3]+",");

                if (i == 0) s.c1 = new Vector2(xw + 0.031f, yw + 0.031f);
                if (i == 1) s.c2 = new Vector2(xw + 0.031f, yw + 0.031f);
                if (i == 2) s.c3 = new Vector2(xw + 0.031f, yw + 0.031f);
                if (i == 3) s.c4 = new Vector2(xw + 0.031f, yw + 0.031f);
                if (i == 4) s.c5 = new Vector2(xw + 0.031f, yw + 0.031f);
            }

            //s.c1 = new Vector2(x_t,y_t);
            s.naam = x + ", " + y;
            stootList.Add(s);
        }

        public List<Stoot> getStootList()
        {
            return stootList;
        }
    }
}