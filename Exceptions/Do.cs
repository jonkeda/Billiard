using System;
using System.Threading;

namespace VouwwandImages.Exceptions
{
    public static class Do
    {
        public static bool WaitUntilAsserted(WaitCondition condition, int timeout, string message)
        {
            if (!WaitUntil(condition, timeout))
            {
                throw new Exception(message);
            }
            return true;
        }

        public static bool WaitUntilVerified(WaitCondition condition, int timeout, string message)
        {
            if (!WaitUntil(condition, timeout))
            {
                throw new Exception(message);
            }
            return true;
        }

        public static bool WaitUntil(WaitCondition condition, int timeout, string message)
        {
            if (!WaitUntil(condition, timeout))
            {
                throw new Exception(message);
            }
            return true;
        }

        public static bool WaitUntil(WaitCondition condition, int timeout)
        {
            DateTime now = DateTime.Now;
            while (true)
            {
                if (condition())
                {
                    return true;
                }

                if ((DateTime.Now - now).TotalMilliseconds > timeout)
                {
                    return false;
                }
                Thread.Sleep(100);
            }
        }
    }
}
