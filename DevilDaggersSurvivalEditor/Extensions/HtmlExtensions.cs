using System.Text;

namespace DevilDaggersSurvivalEditor.Extensions
{
	public static class HtmlExtensions
	{
		public static string HtmlToPlainText(this string html)
		{
			if (string.IsNullOrEmpty(html))
				return string.Empty;

			StringBuilder sb = new();
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
