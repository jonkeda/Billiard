using System.Windows;

namespace VouwwandImages.UI.Interfaces
{
    public interface IParentWindow
    {
        WindowState WindowState { get; set; }
        void Focus();
    }
}