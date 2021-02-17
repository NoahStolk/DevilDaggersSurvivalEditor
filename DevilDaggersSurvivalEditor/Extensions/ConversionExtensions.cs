using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace DevilDaggersSurvivalEditor.Extensions
{
	public static class ConversionExtensions
	{
		private const float _byteValueAmount = byte.MaxValue + 1;

		public static Point3D ToPoint3D(this Color color)
			=> new(color.R / _byteValueAmount, color.G / _byteValueAmount, color.B / _byteValueAmount);

		public static Point4D ToPoint4D(this Color color)
			=> new(color.R / _byteValueAmount, color.G / _byteValueAmount, color.B / _byteValueAmount, color.A / _byteValueAmount);

		public static Point4D ToPoint4D(this Color color, float alpha)
			=> new(color.R / _byteValueAmount, color.G / _byteValueAmount, color.B / _byteValueAmount, alpha);
	}
}
