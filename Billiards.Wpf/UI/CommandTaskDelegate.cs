using System.Threading.Tasks;

namespace Billiards.Wpf.UI
{
    public delegate Task CommandTaskDelegate();


    public delegate Task CommandTaskDelegate<in T>(T parameter);
}