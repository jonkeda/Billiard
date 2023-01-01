using System.Collections.ObjectModel;
using Billiards.Base.Threading;

namespace Billiards.Base.UI
{

    public class SyncedObservableCollection<T> : ObservableCollection<T>
    {
        protected override void ClearItems()
        {
            ThreadDispatcher.Invoke(() => base.ClearItems());
        }

        protected override void InsertItem(int index, T item)
        {
            ThreadDispatcher.Invoke(() => base.InsertItem(index, item));
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            ThreadDispatcher.Invoke(() => base.MoveItem(oldIndex, newIndex));
        }

        protected override void RemoveItem(int index)
        {
            ThreadDispatcher.Invoke(() => base.RemoveItem(index));
        }

        protected override void SetItem(int index, T item)
        {
            ThreadDispatcher.Invoke(() => base.SetItem(index, item));
        }
    }
}
