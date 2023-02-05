using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Billiards.Base.Drawings;
using Billiards.Wpf.UI.Converters;
using DrawingImage = Billiards.Base.Drawings.DrawingImage;

namespace Billiards.Wpf.Drawings
{
    public class ImageStretchConverter : BaseConverter, IValueConverter
    {
        public static readonly IValueConverter Instance = new ImageStretchConverter();
        public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            return (Stretch) value;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


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