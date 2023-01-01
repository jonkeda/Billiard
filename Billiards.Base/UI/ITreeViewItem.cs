namespace Billiards.Base.UI
{
    public interface ITreeViewItem
    {
        bool IsExpanded { get; set; }
        bool BringIntoView { get; set; }
        bool IsSelected { get; set; }
        bool IsHighlighted { get; set; }
    }
}
