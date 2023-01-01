using System.Collections.ObjectModel;

namespace Billiard.UI
{
    public class ConcurrentObservableCollection<T> : ObservableCollection<T>
    {
        readonly object _lock = new object();

        protected override void ClearItems()
        {
            lock (_lock)
            {
                base.ClearItems();
                //ThreadDispatcher.Invoke(() => base.ClearItems());
            }
        }

        protected override void InsertItem(int index, T item)
        {
            lock (_lock)
            {
                base.InsertItem(index, item);
                //ThreadDispatcher.Invoke(() => base.InsertItem(index, item));
            }
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            lock (_lock)
            {
                base.MoveItem(oldIndex, newIndex);
                //ThreadDispatcher.Invoke(() => base.MoveItem(oldIndex, newIndex));
            }
        }

        protected override void RemoveItem(int index)
        {
            lock (_lock)
            {
                base.RemoveItem(index);
                //ThreadDispatcher.Invoke(() => base.RemoveItem(index));
            }
        }

        protected override void SetItem(int index, T item)
        {
            lock (_lock)
            {
                base.SetItem(index, item);
                //ThreadDispatcher.Invoke(() => base.SetItem(index, item));
            }
        }
    }
}
