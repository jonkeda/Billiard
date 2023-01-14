using System;
using Billiards.Base.Filters;
using OpenCvSharp;

namespace Billiards.Wpf.ViewModels;

public class CaptureEvent : EventArgs
{
    public CaptureEvent(Mat image, BallColor cueball)
    {
        Image = image;
        Cueball = cueball;
    }

    public Mat Image { get; }
    public BallColor Cueball { get; }
}