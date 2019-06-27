using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace DevilDaggersSurvivalEditor.Code
{
	public static class ExtensionMethods
	{
		public static Point3D ToPoint3D(this Color color)
		{
			return new Point3D(color.R / 256f, color.G / 256f, color.B / 256f);
		}

		public static Point4D ToPoint4D(this Color color)
		{
			return new Point4D(color.R / 256f, color.G / 256f, color.B / 256f, color.A / 256f);
		}

		public static Point4D ToPoint4D(this Color color, float alpha)
		{
			return new Point4D(color.R / 256f, color.G / 256f, color.B / 256f, alpha);
		}
	}
}