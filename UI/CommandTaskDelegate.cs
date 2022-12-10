using System.Threading.Tasks;

namespace VouwwandImages.UI
{
    public delegate Task CommandTaskDelegate();


    public delegate Task CommandTaskDelegate<in T>(T parameter);
}