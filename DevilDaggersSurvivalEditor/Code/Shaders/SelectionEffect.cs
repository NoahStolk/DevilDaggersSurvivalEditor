using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;

namespace DevilDaggersSurvivalEditor.Code.Shaders
{
	public class SelectionEffect : ShaderEffect
	{
		private static readonly PixelShader pixelShader = new PixelShader { UriSource = MiscUtils.MakeUri(Path.Combine("Content", "Shaders", "Selection.ps")) };

		public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(SelectionEffect), 0);
		public Brush Input
		{
			get => (Brush)GetValue(InputProperty);
			set => SetValue(InputProperty, value);
		}

		public static readonly DependencyProperty NormalMapProperty = RegisterPixelShaderSamplerProperty("NormalMap", typeof(SelectionEffect), 1);
		public Brush NormalMap
		{
			get => (Brush)GetValue(NormalMapProperty);
			set => SetValue(NormalMapProperty, value);
		}

		public static readonly DependencyProperty DiffuseIntensityProperty = DependencyProperty.Register("DiffuseIntensity", typeof(float), typeof(SelectionEffect), new UIPropertyMetadata(1f, PixelShaderConstantCallback(0)));
		public float DiffuseIntensity
		{
			get => (float)GetValue(DiffuseIntensityProperty);
			set => SetValue(DiffuseIntensityProperty, value);
		}

		public static readonly DependencyProperty AmbientIntensityProperty = DependencyProperty.Register("AmbientIntensity", typeof(float), typeof(SelectionEffect), new UIPropertyMetadata(0f, PixelShaderConstantCallback(1)));
		public float AmbientIntensity
		{
			get => (float)GetValue(AmbientIntensityProperty);
			set => SetValue(AmbientIntensityProperty, value);
		}

		public static readonly DependencyProperty AmbientColorProperty = DependencyProperty.Register("AmbientColor", typeof(Point4D), typeof(SelectionEffect), new UIPropertyMetadata(new Point4D(1, 1, 1, 1), PixelShaderConstantCallback(2)));
		public Point4D AmbientColor
		{
			get => (Point4D)GetValue(AmbientColorProperty);
			set => SetValue(AmbientColorProperty, value);
		}

		public static readonly DependencyProperty LightDirectionProperty = DependencyProperty.Register("LightDirection", typeof(Point3D), typeof(SelectionEffect), new UIPropertyMetadata(new Point3D(1, 1, 1), PixelShaderConstantCallback(3)));
		public Point3D LightDirection
		{
			get => (Point3D)GetValue(LightDirectionProperty);
			set => SetValue(LightDirectionProperty, value);
		}

		public SelectionEffect()
		{
			PixelShader = pixelShader;
			UpdateShaderValue(InputProperty);
			UpdateShaderValue(NormalMapProperty);
			UpdateShaderValue(DiffuseIntensityProperty);
			UpdateShaderValue(AmbientIntensityProperty);
			UpdateShaderValue(AmbientColorProperty);
			UpdateShaderValue(LightDirectionProperty);
		}

		public override string ToString()
		{
			return $"Diffuse intensity: {DiffuseIntensity}\nAmbient intensity: {AmbientIntensity}\nAmbient color: {AmbientColor}\nLight direction: {LightDirection}";
		}
	}
}