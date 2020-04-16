﻿using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;

namespace DevilDaggersSurvivalEditor.Code.Shaders
{
	public class SelectionEffect : ShaderEffect
	{
		private static readonly PixelShader pixelShader = new PixelShader { UriSource = ContentUtils.MakeUri(Path.Combine("Content", "Shaders", "Selection.ps")) };

		internal static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(SelectionEffect), 0);
		internal Brush Input
		{
			get => (Brush)GetValue(InputProperty);
			set => SetValue(InputProperty, value);
		}

		internal static readonly DependencyProperty NormalMapProperty = RegisterPixelShaderSamplerProperty("NormalMap", typeof(SelectionEffect), 1);
		internal Brush NormalMap
		{
			get => (Brush)GetValue(NormalMapProperty);
			set => SetValue(NormalMapProperty, value);
		}

		internal static readonly DependencyProperty FlashIntensityProperty = DependencyProperty.Register("FlashIntensity", typeof(float), typeof(SelectionEffect), new UIPropertyMetadata(0f, PixelShaderConstantCallback(0)));
		internal float FlashIntensity
		{
			get => (float)GetValue(FlashIntensityProperty);
			set => SetValue(FlashIntensityProperty, value);
		}

		internal static readonly DependencyProperty HighlightColorProperty = DependencyProperty.Register("HighlightColor", typeof(Point4D), typeof(SelectionEffect), new UIPropertyMetadata(new Point4D(0, 0, 0, 1), PixelShaderConstantCallback(1)));
		internal Point4D HighlightColor
		{
			get => (Point4D)GetValue(HighlightColorProperty);
			set => SetValue(HighlightColorProperty, value);
		}

		internal static readonly DependencyProperty HighlightRadiusSquaredProperty = DependencyProperty.Register("HighlightRadiusSquared", typeof(float), typeof(SelectionEffect), new UIPropertyMetadata(0.1f, PixelShaderConstantCallback(2)));
		internal float HighlightRadiusSquared
		{
			get => (float)GetValue(HighlightRadiusSquaredProperty);
			set => SetValue(HighlightRadiusSquaredProperty, value);
		}

		internal static readonly DependencyProperty MousePositionProperty = DependencyProperty.Register("MousePosition", typeof(Point), typeof(SelectionEffect), new UIPropertyMetadata(new Point(0, 0), PixelShaderConstantCallback(3)));
		internal Point MousePosition
		{
			get => (Point)GetValue(MousePositionProperty);
			set => SetValue(MousePositionProperty, value);
		}

		public SelectionEffect()
		{
			PixelShader = pixelShader;
			UpdateShaderValue(InputProperty);
			UpdateShaderValue(NormalMapProperty);
			UpdateShaderValue(FlashIntensityProperty);
			UpdateShaderValue(HighlightColorProperty);
			UpdateShaderValue(HighlightRadiusSquaredProperty);
			UpdateShaderValue(MousePositionProperty);
		}

		public override string ToString() => $"{nameof(FlashIntensity)}: {FlashIntensity}\n{nameof(HighlightColor)}: {HighlightColor}\n{nameof(HighlightRadiusSquared)}: {HighlightRadiusSquared}\n{nameof(MousePosition)}: {MousePosition}";
	}
}