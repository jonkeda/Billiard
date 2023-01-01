namespace Billiards.Base.Threading
{
    public interface IThreadDispatcher
    {
        bool ShouldInvoke();

        void Invoke(Action action);
    }
}