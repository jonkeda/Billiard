using System.Windows;

namespace Billiards.Wpf.UI.Interfaces
{
    public interface IParentWindow
    {
        WindowState WindowState { get; set; }
        void Focus();
    }
}