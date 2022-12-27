using System.Linq;
using Billiard.UI;
using Emgu.CV;

namespace Billiard.Models;

public class FilterSet : PropertyNotifier
{
    public FilterSet(string name)
    {
        Name = name;
    }

    private string name;
    public string Name
    {
        get { return name; }
        set { SetProperty(ref name, value); }
    }

    public FilterCollection Filters { get; } = new();

    public void ApplyFilters(Mat originalImage)
    {
        foreach (AbstractFilter filter in Filters)
        {
            filter.DoApplyFilter(originalImage);
        }
    }

    protected OriginalFilter Original()
    {
        return Filters.AddFilter(new OriginalFilter());
    }

    protected CvtColorBgr2HsvFilter CvtColorBgr2Hsv()
    {
        return CvtColorBgr2Hsv(Filters.LastOrDefault());
    }

    protected CvtColorBgr2HsvFilter CvtColorBgr2Hsv(AbstractFilter result)
    {
        return Filters.AddFilter(new CvtColorBgr2HsvFilter(result));
    }

    protected GaussianBlurFilter GaussianBlur()
    {
        return GaussianBlur(Filters.LastOrDefault());
    }

    protected GaussianBlurFilter GaussianBlur(AbstractFilter result)
    {
        return Filters.AddFilter(new GaussianBlurFilter(result));
    }

    protected CannyFilter Canny()
    {
        return Canny(Filters.LastOrDefault());
    }

    protected CannyFilter Canny(AbstractFilter result)
    {
        return Filters.AddFilter(new CannyFilter(result));
    }

    protected ExtractChannelFilter ExtractChannel(int channel = 0)
    {
        return ExtractChannel(Filters.LastOrDefault(), channel);
    }

    protected ExtractChannelFilter ExtractChannel(AbstractFilter result, int channel)
    {
        return Filters.AddFilter(new ExtractChannelFilter(result, channel));
    }

    protected FloodFillFilter FloodFill(int floodFill = 100)
    {
        return FloodFill(Filters.LastOrDefault(), floodFill);
    }

    protected FloodFillFilter FloodFill(AbstractFilter result, int floodFill)
    {
        return Filters.AddFilter(new FloodFillFilter(result, floodFill));
    }

    protected MaskFilter Mask()
    {
        return Mask(Filters.LastOrDefault());
    }

    protected MaskFilter Mask(AbstractFilter result)
    {
        return Filters.AddFilter(new MaskFilter(result));
    }


    protected DrawBoundingRectFilter DrawBoundingRect()
    {
        return DrawBoundingRect(Filters.LastOrDefault());
    }

    protected DrawBoundingRectFilter DrawBoundingRect(AbstractFilter result)
    {
        return Filters.AddFilter(new DrawBoundingRectFilter(result));
    }

    protected MorphOpenFilter MorphOpen()
    {
        return MorphOpen(Filters.LastOrDefault());
    }

    protected MorphOpenFilter MorphOpen(AbstractFilter result)
    {
        return Filters.AddFilter(new MorphOpenFilter(result));
    }

    protected MorphCloseFilter MorphClose()
    {
        return MorphClose(Filters.LastOrDefault());
    }

    protected MorphCloseFilter MorphClose(AbstractFilter result)
    {
        return Filters.AddFilter(new MorphCloseFilter(result));
    }

    protected FindCornersFilter FindCorners()
    {
        return FindCorners(Filters.LastOrDefault());
    }

    protected FindCornersFilter FindCorners(AbstractFilter result)
    {
        return Filters.AddFilter(new FindCornersFilter(result));
    }

}