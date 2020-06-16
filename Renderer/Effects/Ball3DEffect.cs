using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;

namespace Effects
{
    public class Ball3DEffect : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(Ball3DEffect), 0);
        public static readonly DependencyProperty Rot0Property = DependencyProperty.Register("Rot0", typeof(Point3D), typeof(Ball3DEffect), new UIPropertyMetadata(new Point3D(0d, 0d, 0d), PixelShaderConstantCallback(0)));
        public static readonly DependencyProperty Rot1Property = DependencyProperty.Register("Rot1", typeof(Point3D), typeof(Ball3DEffect), new UIPropertyMetadata(new Point3D(0d, 0d, 0d), PixelShaderConstantCallback(1)));
        public static readonly DependencyProperty Rot2Property = DependencyProperty.Register("Rot2", typeof(Point3D), typeof(Ball3DEffect), new UIPropertyMetadata(new Point3D(0d, 0d, 0d), PixelShaderConstantCallback(2)));
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size", typeof(Point), typeof(Ball3DEffect), new UIPropertyMetadata(new Point(0d, 0d), PixelShaderConstantCallback(3)));
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(double), typeof(Ball3DEffect), new UIPropertyMetadata(0d, PixelShaderConstantCallback(4)));
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(Point3D), typeof(Ball3DEffect), new UIPropertyMetadata(new Point3D(0d, 0d, 0d), PixelShaderConstantCallback(5)));
        public static readonly DependencyProperty LightPositionProperty = DependencyProperty.Register("LightPosition", typeof(Point3D), typeof(Ball3DEffect), new UIPropertyMetadata(new Point3D(0d, 0d, 0d), PixelShaderConstantCallback(6)));
        public static readonly DependencyProperty ShowPlaneProperty = DependencyProperty.Register("ShowPlane", typeof(double), typeof(Ball3DEffect), new UIPropertyMetadata(1d, PixelShaderConstantCallback(7)));
        public static readonly DependencyProperty HardnessProperty = DependencyProperty.Register("Hardness", typeof(double), typeof(Ball3DEffect), new UIPropertyMetadata(2d, PixelShaderConstantCallback(8)));
        public static readonly DependencyProperty IntensityProperty = DependencyProperty.Register("Intensity", typeof(double), typeof(Ball3DEffect), new UIPropertyMetadata(1250000d, PixelShaderConstantCallback(9)));


        private readonly PixelShader pixelShader = new PixelShader()
        {
            UriSource = new Uri("pack://application:,,,/Renderer/Effects/Ball3DEffect.ps"),
            ShaderRenderMode = ShaderRenderMode.HardwareOnly,
        };

        public Ball3DEffect()
        {
            PixelShader = pixelShader;
            pixelShader.Freeze();

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(Rot0Property);
            UpdateShaderValue(Rot1Property);
            UpdateShaderValue(Rot2Property);
            UpdateShaderValue(SizeProperty);
            UpdateShaderValue(RadiusProperty);
            UpdateShaderValue(PositionProperty);
            UpdateShaderValue(ShowPlaneProperty);
            UpdateShaderValue(HardnessProperty);
            UpdateShaderValue(IntensityProperty);
        }

        public Brush Input
        {
            get
            {
                return (Brush)GetValue(InputProperty);
            }
            set
            {
                SetValue(InputProperty, value);
            }
        }

        public Point3D Rot0
        {
            get
            {
                return (Point3D)GetValue(Rot0Property);
            }
            set
            {
                SetValue(Rot0Property, value);
            }
        }
        public Point3D Rot1
        {
            get
            {
                return (Point3D)GetValue(Rot1Property);
            }
            set
            {
                SetValue(Rot1Property, value);
            }
        }
        public Point3D Rot2
        {
            get
            {
                return (Point3D)GetValue(Rot2Property);
            }
            set
            {
                SetValue(Rot2Property, value);
            }
        }
        public Point Size
        {
            get
            {
                return (Point)GetValue(SizeProperty);
            }
            set
            {
                SetValue(SizeProperty, value);
            }
        }

        public double Radius
        {
            get
            {
                return (double)GetValue(RadiusProperty);
            }
            set
            {
                SetValue(RadiusProperty, value);
            }
        }

        public Point3D Position
        {
            get
            {
                return (Point3D)GetValue(PositionProperty);
            }
            set
            {
                SetValue(PositionProperty, value);
            }
        }

        public Point3D LightPosition
        {
            get
            {
                return (Point3D)GetValue(LightPositionProperty);
            }
            set
            {
                SetValue(LightPositionProperty, value);
            }
        }

        public double ShowPlane
        {
            get
            {
                return (double)GetValue(ShowPlaneProperty);
            }
            set
            {
                SetValue(ShowPlaneProperty, value);
            }
        }
        public double Hardness
        {
            get
            {
                return (double)GetValue(HardnessProperty);
            }
            set
            {
                SetValue(HardnessProperty, value);
            }
        }

        public double Intensity
        {
            get
            {
                return (double)GetValue(IntensityProperty);
            }
            set
            {
                SetValue(IntensityProperty, value);
            }
        }
    }
}
