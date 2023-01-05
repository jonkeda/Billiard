using Billiards.Base.Drawings;

namespace Billiards.Wpf.Drawings;

public class Renderer
{
    public System.Windows.Media.DrawingImage Draw(DrawingImage drawingImage)
    {
        System.Windows.Media.DrawingVisual visual = new System.Windows.Media.DrawingVisual();
        using (System.Windows.Media.DrawingContext drawingContext = visual.RenderOpen())
        {
            WpfRenderer wpfRenderer = new WpfRenderer(drawingContext);

            foreach (AbstractShape shape in drawingImage.Shapes)
            {
                shape.Render(wpfRenderer);
            }

            drawingContext.Close();
        }

        return new System.Windows.Media.DrawingImage(visual.Drawing);
    }
}