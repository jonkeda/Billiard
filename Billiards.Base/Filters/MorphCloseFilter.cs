using OpenCvSharp;

namespace Billiards.Base.Filters;

public class MorphCloseFilter : MorphOpenFilter
{
    public MorphCloseFilter(AbstractFilter filter) : base(filter, MorphTypes.Close)
    {
        Name = "Morph Close";
    }
}