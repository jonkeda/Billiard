﻿using System.Windows;

namespace Billiard.UI.Interfaces
{
    public interface IDialog
    {
        Window Owner { get; set; }

        bool? ShowDialog();

        void Show();

        void Close();

        bool? DialogResult { get; set; }
    }
}
