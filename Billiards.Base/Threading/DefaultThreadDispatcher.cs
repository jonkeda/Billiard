﻿namespace Billiards.Base.Threading
{
    public class DefaultThreadDispatcher : IThreadDispatcher
    {
        public bool ShouldInvoke()
        {
            return false;
        }

        public void Invoke(Action action)
        {
            action.Invoke();
        }
    }
}