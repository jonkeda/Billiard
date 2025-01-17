﻿using com.walter.eightball.objects;

namespace com.walter.eightball;
/*
    import com.badlogic.gdx.graphics.glutils.ShapeRenderer
    import com.badlogic.gdx.graphics.glutils.ShapeRenderer.ShapeType
    import com.walter.eightball.ui.Styles
*/
/** Represents the in-game board */
public class Board
{

    public static float Height = 1.17f;
    public static float Width = 2.34f;
    public static float PocketRadius = 1.6f * Ball.Radius; //1.6x ball radius

    /** Renders the board
      *
      * @param renderer The shape renderer to use
      * @param scale    Screen pixels per in-game meter
      */
/*    def render(renderer: ShapeRenderer, scale: Float) = {
        renderer.setColor(Styles.BoardColor)

        //Draw the board
        renderer.set(ShapeType.Filled)
        renderer.rect(0f, 0f, Board.Width * scale, Board.Height * scale)

        //Draw the pockets
        renderer.setColor(Styles.PocketColor)

        for (px < -0.0f to 1.0f by 0.5f;
        py < -0.0f to 1.0f by 1f)
        {
            renderer.circle(px * Board.Width * scale, py * Board.Height * scale, scale * Board.PocketRadius)
        }
    }*/
}