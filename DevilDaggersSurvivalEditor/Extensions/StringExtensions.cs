using System.Text;

namespace DevilDaggersSurvivalEditor.Extensions
{
	public static class StringExtensions
	{
		public static string ToUserFriendlyString(this object input)
		{
			StringBuilder sb = new();
			bool first = true;
			foreach (char c in input.ToString() ?? string.Empty)
			{
				if (char.IsUpper(c) && !first)
					sb.Append(' ').Append(char.ToLower(c));
				else
					sb.Append(c);
				first = false;
			}

			return sb.ToString();
		}
	}
}
