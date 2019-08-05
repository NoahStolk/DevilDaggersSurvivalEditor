﻿namespace DevilDaggersSurvivalEditor.Code.Web
{
	public static class UrlUtils
	{
		private static readonly string baseUrl = "https://devildaggers.info";

		public static string DiscordInviteLink => "https://discord.gg/NF32j8S";

		public static string GetToolVersions => $"{baseUrl}/API/GetToolVersions";
		public static string GetSpawnsets => $"{baseUrl}/API/GetSpawnsets";
		public static string GetCustomLeaderboards => $"{baseUrl}/API/GetCustomLeaderboards";
		public static string Spawnsets => $"{baseUrl}/Spawnsets";

		public static string SourceCode => "https://bitbucket.org/NoahStolk/devildaggerssurvivaleditor/src/master/";

		public static string GetSpawnset(string fileName) => $"{baseUrl}/API/GetSpawnset?fileName={fileName}";

		public static string CustomLeaderboard(string spawnsetFileName) => $"{baseUrl}/CustomLeaderboards/Leaderboard?spawnset={spawnsetFileName}";

		public static string ApplicationDownloadUrl(string versionNumber) => $"{baseUrl}/tools/{ApplicationUtils.ApplicationName}/{ApplicationUtils.ApplicationName}{versionNumber}.zip";
	}
}