using System;

namespace DevilDaggersSurvivalEditor.Code
{
	internal static class ContentUtils
	{
		internal static Uri MakeUri(string localPath) => new Uri($"pack://application:,,,/{App.Assembly.GetName().Name};component/{localPath}");
	}
}