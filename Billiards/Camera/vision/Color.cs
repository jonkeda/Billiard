using Billiard.Camera.vision.Geometries;

namespace Billiard.Camera.vision
{
    public class Color
    {

        public static Scalar WHITE = new Scalar(255, 255, 255);


        public static Scalar BLACK = new Scalar(0, 0, 0);


        public static Scalar RED = new Scalar(0, 0, 255);


        public static Scalar LIGHT_RED = new Scalar(75, 75, 200);


        public static Scalar BLUE = new Scalar(255, 0, 0);


        public static Scalar GREEN = new Scalar(0, 255, 0);


        public static Scalar TURQUOISE = new Scalar(255, 255, 0);


        public static Scalar PURPLE = new Scalar(255, 0, 255);


        public static Scalar YELLOW = new Scalar(0, 255, 255);
    


        static Scalar getIndexColor(int index)
        {
            switch (index)
            {
                case 0:
                    return RED;
                    break;
    
                case 1:
                    return BLUE;
                    break;
    
                case 2:
                    return GREEN;
                    break;
    
                case 3:
                    return BLACK;
                    break;
    
                case 4:
                    return TURQUOISE;
                    break;
    
                case 5:
                    return WHITE;
                    break;
    
                case 6:
                    return PURPLE;
                    break;
    
                default:
                    return new Scalar(128, index * 10, index * 30);
    
            }
        }


/*        static def getIndexAwtColor(int index)
        {
            switch (index)
            {
                case 0:
                    return java.awt.Color.RED; break
    
                case 1:
                    return java.awt.Color.BLUE; break
    
                case 2:
                    return java.awt.Color.GREEN; break
    
                case 3:
                    return java.awt.Color.BLACK; break
    
                case 4:
                    return java.awt.Color.CYAN; break
    
                case 5:
                    return java.awt.Color.WHITE; break
    
                case 6:
                    return java.awt.Color.PINK; break
    
                default:
                    return new java.awt.Color(128, index * 10, index * 30)
    
            }
        }*/

    }
}
