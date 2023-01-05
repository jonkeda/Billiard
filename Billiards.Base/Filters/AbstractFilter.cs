using Billiards.Base.Drawings;
using Billiards.Base.UI;
using OpenCvSharp;

namespace Billiards.Base.Filters;

public abstract class AbstractFilter : PropertyNotifier
{
    private Mat? resultMat = new Mat();
    private string? name;
    private bool enabled = true;
    private DrawingImage? drawingImage;
    private FilterValueCollection filterValues = new FilterValueCollection();

    protected AbstractFilter? InputFilter { get; private set; }

    public FilterValueCollection FilterValues
    {
        get { return filterValues; }
        private set { SetProperty(ref filterValues, value); }
    }

    protected AbstractFilter(AbstractFilter? filter)
    {
        InputFilter = filter;
    }

    public Mat? ResultMat
    {
        get
        {
            if (enabled)
            {
                return resultMat;
            }
            return InputFilter?.ResultMat;
        }
        set
        {
/*            if (resultMat != value
                && resultMat != null)
            {
                resultMat.Dispose();
            }*/

            SetProperty(ref resultMat, value);
        }
    }

/*    public ImageSource ResultImage
    {
        get
        {

            return ResultMat?.ToImageSource();
        }
    }
*/
    public bool Enabled
    {
        get { return enabled; }
        set { SetProperty(ref enabled, value); }
    }

    public string? Name
    {
        get { return name; }
        set { SetProperty(ref name, value); }
    }

    public void DoApplyFilter(Mat? originalImage)
    {
        FilterValues?.Clear();
        if (!Enabled)
        {
            ResultMat = null;
            NotifyChanged();
            return;
        }

        try
        {
            ApplyFilter(originalImage);
        }
        catch (Exception ex)
        {
            FilterValues?.Add("Exception", ex.Message);
        }
        NotifyChanged();

    }

    private void NotifyChanged()
    {
        NotifyPropertyChanged(nameof(ResultMat));
        // NotifyPropertyChanged(nameof(DrawingImage));
    }

    protected abstract void ApplyFilter(Mat? originalImage);

    public Mat? GetInputMat()
    {
        return InputFilter?.ResultMat;
    }

    public DrawingImage? DrawingImage
    {
        get { return drawingImage; }
        protected set { SetProperty(ref drawingImage, value); }
    }

    protected void Draw(Action<DrawingContext> action)
    {
        Mat? mat = GetInputMat();
        if (mat == null
            || mat.Data == 0)
        {
            return;
        }

        DrawingVisual visual = new DrawingVisual();
        using (DrawingContext drawingContext = visual.RenderOpen())
        {
            drawingContext.PushClip(new RectangleGeometry(
                new Rect2f(new Point2f(0, 0),
                    new Size2f(mat.Width, mat.Height))));
            drawingContext.DrawRectangle(new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)), null,
                new Rect2f(0, 0, mat.Width, mat.Height));

            action.Invoke(drawingContext);

            drawingContext.Close();
        }

        DrawingImage = new DrawingImage(visual.Drawing);
    }
}