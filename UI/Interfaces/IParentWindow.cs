using System.Windows;

namespace Billiard.UI.Interfaces
{
    public interface IParentWindow
    {
        WindowState WindowState { get; set; }
        void Focus();
    }
}