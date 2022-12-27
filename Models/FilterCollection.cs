using System.Collections.ObjectModel;

namespace Billiard.Models;

public class FilterCollection : ObservableCollection<AbstractFilter>
{
    public T AddFilter<T>(T filter) where T : AbstractFilter
    {
        Add(filter);
        return filter;
    }
}