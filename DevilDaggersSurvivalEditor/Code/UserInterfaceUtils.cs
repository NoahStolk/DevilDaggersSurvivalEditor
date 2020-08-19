using System;
using System.Globalization;
using System.Text;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.Code
{
	public static class UserInterfaceUtils
	{
		public static string ToUserFriendlyString(this object input)
		{
			StringBuilder sb = new StringBuilder();
			bool first = true;
			foreach (char c in input.ToString() ?? string.Empty)
			{
				if (char.IsUpper(c) && !first)
					sb.Append($" {c.ToString().ToLower(CultureInfo.InvariantCulture)}");
				else
					sb.Append(c);
				first = false;
			}

			return sb.ToString();
		}

		/// <summary>
		/// Returns an integer between 0 and 255 representing the perceived brightness of the color.
		/// </summary>
		/// <param name="c">The color.</param>
		/// <returns>The perceived brightness between 0 and 255.</returns>
		public static int GetPerceivedBrightness(Color c)
			=> (int)Math.Sqrt(
				c.R * c.R * .299 +
				c.G * c.G * .587 +
				c.B * c.B * .114);
	}
}