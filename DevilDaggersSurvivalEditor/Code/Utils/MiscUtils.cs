using System.Text;

namespace DevilDaggersSurvivalEditor.Code.Utils
{
	public static class MiscUtils
	{
		public static string ToUserFriendlyString(this string input)
		{
			StringBuilder sb = new StringBuilder();
			bool first = true;
			foreach (char c in input)
			{
				if (char.IsUpper(c) && !first)
					sb.Append($" {c.ToString().ToLower()}");
				else
					sb.Append(c);
				first = false;
			}
			return sb.ToString();
		}
	}
}