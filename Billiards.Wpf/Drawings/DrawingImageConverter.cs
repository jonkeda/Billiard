using System;
using System.Globalization;
using System.Windows.Data;
using Billiards.Base.Drawings;
using Billiards.Wpf.UI.Converters;

namespace Billiards.Wpf.Drawings
{
    public class WpfDrawingImage : IPlatformDrawingImage
    {
        public WpfDrawingImage(System.Windows.Media.DrawingImage image)
        {
            Image = image;
        }
        public System.Windows.Media.DrawingImage Image { get; }
    }

    public class DrawingImageConverter : BaseConverter, IValueConverter
    {
        public static readonly IValueConverter Instance = new DrawingImageConverter();
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DrawingImage drawingImage)
            {
                if (drawingImage.PlatformDrawingImage is WpfDrawingImage wpfDrawingImage)
                {
                    return wpfDrawingImage.Image;
                }

                Renderer renderer = new ();
                System.Windows.Media.DrawingImage image = renderer.Draw(drawingImage);

                drawingImage.PlatformDrawingImage = new WpfDrawingImage(image);

                return image;
            }

            return null;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}