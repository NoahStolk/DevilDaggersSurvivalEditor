using System;

namespace DevilDaggersSurvivalEditor.Code
{
	public static class ContentUtils
	{
		public static Uri MakeUri(string localPath)
		{
			return new Uri($"pack://application:,,,/{App.Instance.Assembly.GetName().Name};component/{localPath}");
		}
	}
}