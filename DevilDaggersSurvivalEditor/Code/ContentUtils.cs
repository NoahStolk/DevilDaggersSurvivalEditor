using System;

namespace DevilDaggersSurvivalEditor.Code
{
	public static class ContentUtils
	{
		public static Uri MakeUri(string localPath)
		{
			return new Uri($"pack://application:,,,/{Program.App.Assembly.GetName().Name};component/{localPath}");
		}
	}
}