using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Billiards.Base.Threading;

namespace Billiards.Base.UI
{
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

    public class PropertyNotifier : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            Debug.Assert(propertyName != null, "propertyName != null");
            if (!Equals(field, value))
            {
                NotifyPropertyChanging(propertyName);

                field = value;

                NotifyPropertyChanged(propertyName);

                return true;
            }
            return false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        protected void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                ThreadDispatcher.Invoke(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        protected void NotifyPropertyChanging([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanging != null)
            {
                ThreadDispatcher.Invoke(() => PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName)));
            }
        }
    }
}
