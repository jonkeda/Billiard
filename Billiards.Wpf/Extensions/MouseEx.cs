﻿using System;
using System.Windows.Input;

namespace Billiards.Wpf.Extensions
{
    public static class MouseEx
    {
        public static void SetCursor(Cursor cursor, Action action)
        {
            try
            {
                Mouse.OverrideCursor = cursor;
                action.Invoke();
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
    }
}