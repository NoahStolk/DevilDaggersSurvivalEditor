using System;

namespace DevilDaggersSurvivalEditor.Utils
{
	public static class ContentUtils
	{
		public static Uri MakeUri(string localPath)
			=> new Uri($"pack://application:,,,/{App.Assembly.GetName().Name};component/{localPath}");
	}
}