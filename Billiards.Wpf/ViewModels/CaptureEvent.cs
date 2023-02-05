using System;
using Billiards.Base.Filters;
using OpenCvSharp;

namespace Billiards.Wpf.ViewModels;

public class CaptureEvent : EventArgs
{
    public CaptureEvent(Mat image, BallColor cueball, string? filename)
    {
        Image = image;
        Cueball = cueball;
        Filename = filename;
    }

    public string? Filename { get; set; }

    public Mat Image { get; }
    public BallColor Cueball { get; }
}