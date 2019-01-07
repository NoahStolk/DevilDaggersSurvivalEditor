namespace DevilDaggersSurvivalEditor.Utils
{
	public static class UrlUtils
	{
		public static string Discord { get { return "https://discord.gg/NF32j8S"; } }
		
		private static readonly string devilDaggersWebsiteBaseUrl = "https://devildaggers.info";

		public static string GetToolVersions { get { return $"{devilDaggersWebsiteBaseUrl}/API/GetToolVersions"; } }
		public static string GetSpawnsets { get { return $"{devilDaggersWebsiteBaseUrl}/API/GetSpawnsets"; } }
		public static string GetSpawnset(string fileName) { return $"{devilDaggersWebsiteBaseUrl}/API/GetSpawnset?fileName={fileName}"; }

		public static string Spawnsets { get { return $"{devilDaggersWebsiteBaseUrl}/Spawnsets"; } }
		public static string DownloadUrl(string versionOnline) { return $"{devilDaggersWebsiteBaseUrl}/tools/DevilDaggersSurvivalEditor/DevilDaggersSurvivalEditor{versionOnline}.zip"; }
	}
}