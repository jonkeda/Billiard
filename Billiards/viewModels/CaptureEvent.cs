using System;
using Emgu.CV;

namespace Billiard.viewModels;

public class CaptureEvent : EventArgs
{
    public CaptureEvent(Mat image)
    {
        Image = image;
    }

    public Mat Image { get; }
}