﻿using System;
using System.Windows;
using System.Windows.Media;
using Billiard.Camera.vision.Geometries;
using Billiard.UI;
using Emgu.CV;

namespace Billiard.Models;

public abstract class AbstractFilter : PropertyNotifier
{
    private Mat resultMat = new Mat();
    private string name;
    private string exception;
    private bool enabled = true;
    private DrawingImage drawingImage;

    protected AbstractFilter InputFilter { get; private set; }

    protected AbstractFilter(AbstractFilter filter)
    {
        InputFilter = filter;
    }

    public Mat ResultMat
    {
        get
        {
            if (enabled)
            {
                return resultMat;
            }
            return InputFilter?.ResultMat;
        }
        set { SetProperty(ref resultMat, value); }
    }

    public ImageSource ResultImage
    {
        get
        {

            return ResultMat?.ToImageSource();
        }
    }

    public bool Enabled
    {
        get { return enabled; }
        set { SetProperty(ref enabled, value); }
    }

    public string Name
    {
        get { return name; }
        set { SetProperty(ref name, value); }
    }

    public string Exception
    {
        get { return exception; }
        set { SetProperty(ref exception, value); }
    }

    public void DoApplyFilter(Mat originalImage)
    {
        if (!Enabled)
        {
            ResultMat = null;
            NotifyChanged();
            return;
        }

        try
        {
            ApplyFilter(originalImage);
        }
        catch (Exception ex)
        {
            Exception = ex.Message;
        }
        NotifyChanged();

    }

    private void NotifyChanged()
    {
        NotifyPropertyChanged(nameof(ResultImage));
        NotifyPropertyChanged(nameof(DrawingImage));
    }

    protected abstract void ApplyFilter(Mat originalImage);

    public Mat GetInputMat()
    {
        return InputFilter.ResultMat;
    }

    public DrawingImage DrawingImage
    {
        get { return drawingImage; }
        private set { drawingImage = value; }
    }

    protected void Draw(Action<DrawingContext> action)
    {
        Mat mat = GetInputMat();
        if (mat?.GetData() == null)
        {
            return;
        }

        DrawingVisual visual = new DrawingVisual();
        using (DrawingContext drawingContext = visual.RenderOpen())
        {
            drawingContext.PushClip(new RectangleGeometry(
                new Rect(new Point(0, 0),
                    new Point(mat.Width, mat.Height))));
            drawingContext.DrawRectangle(new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0)), null,
                new Rect(0, 0, mat.Width, mat.Height));

            action.Invoke(drawingContext);

            drawingContext.Close();
        }

        DrawingImage = new DrawingImage(visual.Drawing);
    }
}