using System.Collections.ObjectModel;

namespace Billiards.Base.Filters;

public class ContourCollection : Collection<Contour>
{
    public ContourCollection()
    {

    }

    public ContourCollection(List<Contour> toList)
    {
        foreach (Contour contour in toList)
        {
            Add(contour);
        }
    }
}