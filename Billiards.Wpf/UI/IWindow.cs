namespace Billiards.Wpf.UI
{
    public interface IWindow
    {
        void Close();
        bool? DialogResult { get; set; }
    }
}