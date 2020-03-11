using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace DevilDaggersSurvivalEditor.Code
{
	public static class ExtensionMethods
	{
		public static Point3D ToPoint3D(this Color color) => new Point3D(color.R / 256f, color.G / 256f, color.B / 256f);

		public static Point4D ToPoint4D(this Color color) => new Point4D(color.R / 256f, color.G / 256f, color.B / 256f, color.A / 256f);

		public static Point4D ToPoint4D(this Color color, float alpha) => new Point4D(color.R / 256f, color.G / 256f, color.B / 256f, alpha);

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