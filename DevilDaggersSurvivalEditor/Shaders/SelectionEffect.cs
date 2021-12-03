using DevilDaggersSurvivalEditor.Utils;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;

namespace DevilDaggersSurvivalEditor.Shaders;

public class SelectionEffect : ShaderEffect
{
	private static readonly PixelShader _pixelShader = new() { UriSource = ContentUtils.MakeUri(Path.Combine("Content", "Shaders", "Selection.ps")) };

	public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(SelectionEffect), 0);
	public static readonly DependencyProperty NormalMapProperty = RegisterPixelShaderSamplerProperty("NormalMap", typeof(SelectionEffect), 1);
	public static readonly DependencyProperty FlashIntensityProperty = DependencyProperty.Register("FlashIntensity", typeof(float), typeof(SelectionEffect), new UIPropertyMetadata(0f, PixelShaderConstantCallback(0)));
	public static readonly DependencyProperty HighlightColorProperty = DependencyProperty.Register("HighlightColor", typeof(Point4D), typeof(SelectionEffect), new UIPropertyMetadata(new Point4D(0, 0, 0, 1), PixelShaderConstantCallback(1)));
	public static readonly DependencyProperty HighlightRadiusSquaredProperty = DependencyProperty.Register("HighlightRadiusSquared", typeof(float), typeof(SelectionEffect), new UIPropertyMetadata(0.1f, PixelShaderConstantCallback(2)));
	public static readonly DependencyProperty MousePositionProperty = DependencyProperty.Register("MousePosition", typeof(Point), typeof(SelectionEffect), new UIPropertyMetadata(new Point(0, 0), PixelShaderConstantCallback(3)));

	public SelectionEffect()
	{
		PixelShader = _pixelShader;
		UpdateShaderValue(InputProperty);
		UpdateShaderValue(NormalMapProperty);
		UpdateShaderValue(FlashIntensityProperty);
		UpdateShaderValue(HighlightColorProperty);
		UpdateShaderValue(HighlightRadiusSquaredProperty);
		UpdateShaderValue(MousePositionProperty);
	}

	public Brush Input
	{
		get => (Brush)GetValue(InputProperty);
		set => SetValue(InputProperty, value);
	}

	public Brush NormalMap
	{
		get => (Brush)GetValue(NormalMapProperty);
		set => SetValue(NormalMapProperty, value);
	}

	public float FlashIntensity
	{
		get => (float)GetValue(FlashIntensityProperty);
		set => SetValue(FlashIntensityProperty, value);
	}

	public Point4D HighlightColor
	{
		get => (Point4D)GetValue(HighlightColorProperty);
		set => SetValue(HighlightColorProperty, value);
	}

	public float HighlightRadiusSquared
	{
		get => (float)GetValue(HighlightRadiusSquaredProperty);
		set => SetValue(HighlightRadiusSquaredProperty, value);
	}

	public Point MousePosition
	{
		get => (Point)GetValue(MousePositionProperty);
		set => SetValue(MousePositionProperty, value);
	}

	public override string ToString()
		=> $"{nameof(FlashIntensity)}: {FlashIntensity}\n{nameof(HighlightColor)}: {HighlightColor}\n{nameof(HighlightRadiusSquared)}: {HighlightRadiusSquared}\n{nameof(MousePosition)}: {MousePosition}";
}
