using System.Threading.Tasks;

namespace Billiards.Wpf.UI
{
    public delegate Task<bool> CommandTaskCanExecuteDelegate<in T>(T parameter);

    public delegate Task<bool> CommandTaskCanExecuteDelegate();
}