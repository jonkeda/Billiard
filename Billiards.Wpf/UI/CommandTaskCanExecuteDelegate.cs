using System.Threading.Tasks;

namespace Billiard.UI
{
    public delegate Task<bool> CommandTaskCanExecuteDelegate<in T>(T parameter);

    public delegate Task<bool> CommandTaskCanExecuteDelegate();
}