using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace DevilDaggersSurvivalEditor.Code
{
	public static class ExtensionMethods
	{
		private const float byteValueAmount = 256;

		public static Point3D ToPoint3D(this Color color) => new Point3D(color.R / byteValueAmount, color.G / byteValueAmount, color.B / byteValueAmount);

		public static Point4D ToPoint4D(this Color color) => new Point4D(color.R / byteValueAmount, color.G / byteValueAmount, color.B / byteValueAmount, color.A / byteValueAmount);

		public static Point4D ToPoint4D(this Color color, float alpha) => new Point4D(color.R / byteValueAmount, color.G / byteValueAmount, color.B / byteValueAmount, alpha);

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