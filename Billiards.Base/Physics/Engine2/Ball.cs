using OpenCvSharp;

namespace com.walter.eightball.objects;

/*import com.badlogic.gdx.graphics.Color
import com.badlogic.gdx.graphics.glutils.ShapeRenderer
import com.badlogic.gdx.graphics.glutils.ShapeRenderer.ShapeType
import com.walter.eightball.math.Vector3D
import com.walter.eightball.ui.Styles
*/

using System.Numerics;
/** Represents a ball on the pool table
 *  
 *  Balls implement 3D vectors that define their position. They also have a render method
  * to render themselves.
  *
  *
 *  The numbering of the balls is as follows:

      0.  The cue balls
      1.  Yellow
      2.  Blue
      3.  Red
      4.  Purple
      5.  Orange
      6.  Green
      7.  Brown
      8.  Black
      9.  Yellow and white
      10.  Blue and white
      11.  Red and white
      12.  Purple and white
      13.  Orange and white
      14.  Green and white
      15.  Brown and white */
//@SerialVersionUID(1L)
public class Ball  //with Serializable 
{
    public Ball(float x, float y, float z, int number)
    {
        Number = number;
        location = new Vector3(x, y, z);
    }

    public const float Radius = 0.028575f;

    public int Number { get; }

    public float mass { get; set; } = 0.16f; //In kg according to the WPA spec
    public float radius { get; set; } = 0.028575f; //In m according to the WPA spec


    public Vector3 location { get; set; }
    public Vector3 velocity { get; set; } = new Vector3(0f, 0f, 0f);
    public Vector3 angularVelocity { get; set; } = new Vector3(0f, 0f, 0f);

    /** Renders the ball
      *
      * @param renderer The shape renderer to use
      * @param scale Screen pixels per in-game meter
      * @param active Whether the ball should be rendered with its color or not
      */

    /*  def render(renderer: ShapeRenderer, scale: Float, active: Boolean): Unit = {
        renderer.set(ShapeType.Filled)

        val color = if (active) Ball.ColorOfBall(number) else Styles.Gray
        renderer.setColor(color)

        if (number > 8)
        {
            renderer.setColor(Styles.White)
          renderer.circle(x * scale, y * scale, radius * scale)
          renderer.setColor(color)
          renderer.circle(x * scale, y * scale, 0.8f * radius * scale)
        }
        else
        {
            renderer.setColor(color)
          renderer.circle(x * scale, y * scale, radius * scale)
        }
    */
}



/** Companion object for the Ball class which specifies mass, radius, and the
  * colors associated with each number */
/*object Ball
{

    val Mass = 0.16f //In kg according to the WPA spec
  val Radius = 0.028575f //In m according to the WPA spec
  val ColorOfBall: Map [Int, Color] = Map(
      0  -> Styles.White,
      1  -> Styles.Yellow,
      2  -> Styles.Blue,
      3  -> Styles.Red,
      4  -> Styles.Purple,
      5  -> Styles.Orange,
      6  -> Styles.Green,
      7  -> Styles.Brown,
      8  -> Styles.Black,
      9  -> Styles.Yellow,
      10 -> Styles.Blue,
      11 -> Styles.Red,
      12 -> Styles.Purple,
      13 -> Styles.Orange,
      14 -> Styles.Green,
      15 -> Styles.Brown)
}*/