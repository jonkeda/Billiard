using System;
using OpenCvSharp;

namespace Billiards.Wpf.ViewModels;

public class CaptureEvent : EventArgs
{
    public CaptureEvent(Mat image)
    {
        Image = image;
    }

    public Mat Image { get; }
}