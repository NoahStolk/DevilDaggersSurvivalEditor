using System;

namespace DevilDaggersSurvivalEditor.Utils;

public static class UrlUtils
{
#if TESTING
	private static readonly Uri _baseUrl = new("http://localhost:2963");
#else
	private static readonly Uri _baseUrl = new("https://devildaggers.info");
#endif

	public static string DiscordInviteLink => "https://discord.gg/NF32j8S";

	public static string SpawnsetsPage => $"{_baseUrl}custom/spawnsets";

	public static string GuidePage => $"{_baseUrl}guides/creating-spawnsets";

	public static string SourceCode => $"https://github.com/NoahStolk/{App.ApplicationName}";
}
