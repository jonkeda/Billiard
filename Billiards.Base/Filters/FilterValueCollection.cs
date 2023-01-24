using System.Collections.ObjectModel;

namespace Billiards.Base.Filters;

public class FilterValueCollection : ObservableCollection<FilterValue>
{
    public void Add(string name, double value)
    {
        Add(new FilterValue(name, value));
    }

    public void Add(string name, double value, string? text)
    {
        Add(new FilterValue(name, value, text));
    }

    public void Add(string name, string? text)
    {
        Add(new FilterValue(name, text));
    }

}