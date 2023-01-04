using System.Xml.Serialization;

namespace Billiards.Wpf.UI;

public abstract class TreeViewItemModel : PropertyNotifier, ITreeViewItem
{
    private bool _isExpanded;
    private bool _bringIntoView;
    private bool _isSelected;
    private bool _isHighlighted;

    [XmlIgnore]
    public bool IsExpanded
    {
        get { return _isExpanded; }
        set { SetProperty(ref _isExpanded, value); }
    }

    [XmlIgnore]
    public bool BringIntoView
    {
        get { return _bringIntoView; }
        set { SetProperty(ref _bringIntoView, value); }
    }

    [XmlIgnore]
    public bool IsSelected
    {
        get { return _isSelected; }
        set { SetProperty(ref _isSelected, value); }
    }
        
    [XmlIgnore]
    public bool IsHighlighted
    {
        get { return _isHighlighted; }
        set { SetProperty(ref _isHighlighted, value); }
    }
}