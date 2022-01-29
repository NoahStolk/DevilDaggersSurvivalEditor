using System;

namespace DevilDaggersSurvivalEditor.Utils;

public static class UrlUtils
{
#if TESTING
	public static Uri BaseUrl { get; } = new("http://localhost:2963");
#else
	public static Uri BaseUrl { get; } = new("https://devildaggers.info");
#endif

	public static string DiscordInviteLink => "https://discord.gg/NF32j8S";

	public static string SpawnsetsPage => $"{BaseUrl}custom/spawnsets";

	public static string GuidePage => $"{BaseUrl}guides/survival-editor";

	public static string SourceCode => $"https://github.com/NoahStolk/{App.ApplicationName}";
}
