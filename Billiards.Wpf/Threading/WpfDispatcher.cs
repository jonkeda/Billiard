﻿using System;
using System.Windows;
using Billiards.Base.Threading;

namespace Billiards.Wpf.Threading
{
    public class WpfDispatcher
        : IThreadDispatcher
    {
        #region IThreadDispatcher Members

        public bool ShouldInvoke()
        {
            if (Application.Current == null)
            {
                return false;
            }
            return !Application.Current.Dispatcher.CheckAccess();
        }

        public void Invoke(Action action)
        {
            if (ShouldInvoke())
            {
                Application.Current.Dispatcher.Invoke(action);
            }
            else
            {
                action.Invoke();
            }
        }

        #endregion
    }
}