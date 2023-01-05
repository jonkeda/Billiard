using System.Runtime.InteropServices;
using System.Windows.Media;
using Emgu.CV;

namespace Billiard.Camera.vision.Geometries;

public static class MatExtensions
{
    public static ImageSource ToImageSource(this Mat mat)
    {
        if (mat == null
            || mat.IsEmpty)
        {
            return null;
        }
        return mat.ToBitmapSource();
    }

    public static float height(this Mat list)
    {
        return list.Height;
    }

    public static float width(this Mat list)
    {
        return list.Height;
    }

    public static void SetDoubleValue(this Mat mat, int row, int col, double value)
    {
        var target = new[] { value };
        Marshal.Copy(target, 0, mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, 1);
    }


    public static double GetDoubleValue(this Mat mat, int row, int col)
    {
        var value = new double[1];
        Marshal.Copy(mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, value, 0, 1);
        return value[0];
    }

    public static void SetFloatValue(this Mat mat, int row, int col, float value)
    {
        var target = new[] { value };
        Marshal.Copy(target, 0, mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, 1);
    }


    public static float GetFloatValue(this Mat mat, int row, int col)
    {
        var value = new float[1];
        Marshal.Copy(mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, value, 0, 1);
        return value[0];
    }

}