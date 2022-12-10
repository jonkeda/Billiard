using System;

namespace VouwwandImages.Threading
{
    public interface IThreadDispatcher
    {
        bool ShouldInvoke();

        void Invoke(Action action);
    }
}