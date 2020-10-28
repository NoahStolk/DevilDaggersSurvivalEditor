using System.Globalization;
using System.Text;

namespace DevilDaggersSurvivalEditor.Extensions
{
	public static class StringExtensions
	{
		public static string ToUserFriendlyString(this object input)
		{
			StringBuilder sb = new StringBuilder();
			bool first = true;
			foreach (char c in input.ToString() ?? string.Empty)
			{
				if (char.IsUpper(c) && !first)
					sb.Append(' ').Append(c.ToString().ToLower(CultureInfo.InvariantCulture));
				else
					sb.Append(c);
				first = false;
			}

			return sb.ToString();
		}
	}
}