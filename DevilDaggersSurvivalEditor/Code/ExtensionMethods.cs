using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace DevilDaggersSurvivalEditor.Code
{
	public static class ExtensionMethods
	{
		private const float _byteValueAmount = byte.MaxValue + 1;

		public static Point3D ToPoint3D(this Color color)
			=> new Point3D(color.R / _byteValueAmount, color.G / _byteValueAmount, color.B / _byteValueAmount);

		public static Point4D ToPoint4D(this Color color)
			=> new Point4D(color.R / _byteValueAmount, color.G / _byteValueAmount, color.B / _byteValueAmount, color.A / _byteValueAmount);

		public static Point4D ToPoint4D(this Color color, float alpha)
			=> new Point4D(color.R / _byteValueAmount, color.G / _byteValueAmount, color.B / _byteValueAmount, alpha);

		public static string HtmlToPlainText(this string html)
		{
			if (string.IsNullOrEmpty(html))
				return string.Empty;

			StringBuilder sb = new StringBuilder();
			bool inside = false;
			foreach (char c in html)
			{
				if (c == '<')
					inside = true;

				if (!inside)
					sb.Append(c);

				if (c == '>')
					inside = false;
			}

			return sb.ToString();
		}
	}
}