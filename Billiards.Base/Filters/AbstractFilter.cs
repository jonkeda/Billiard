using Billiards.Base.Drawings;
using Billiards.Base.UI;
using OpenCvSharp;

namespace Billiards.Base.Filters;

public enum ImageStretch
{
    None,
    Fill,
    Uniform,
    UniformToFill
}

public abstract class AbstractFilter : PropertyNotifier
{
    private Mat? resultMat = new Mat();
    private string? name;
    private bool enabled = true;
    private DrawingImage? drawingImage;
    private FilterValueCollection filterValues = new FilterValueCollection();
    private double timeTaken;

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

    public double TimeTaken
    {
        get { return timeTaken; }
        private set { SetProperty(ref timeTaken, value); }
    }


    public void DoApplyFilter(Mat? originalImage)
    {
        DateTime now = DateTime.Now;
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
        TimeTaken = (DateTime.Now - now).TotalMilliseconds;
    }

    private void NotifyChanged()
    {
        NotifyPropertyChanged(nameof(ResultMat));
        NotifyPropertyChanged(nameof(DrawingImage));
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

    public bool DrawImage  { get; set; }
    public ImageStretch ImageStretch { get; protected set; } = ImageStretch.Uniform;

    protected void Draw(Action<DrawingContext> action)
    {
        if (!DrawImage)
        {
            return;
        }

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
            drawingContext.DrawRectangle(Brushes.Transparent, null,
                new Rect2f(0, 0, mat.Width, mat.Height));

            action.Invoke(drawingContext);

            drawingContext.Close();
        }

        DrawingImage = new DrawingImage(visual.Drawing);
    }
}