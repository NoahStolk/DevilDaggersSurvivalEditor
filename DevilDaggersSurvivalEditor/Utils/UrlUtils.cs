namespace DevilDaggersSurvivalEditor.Utils
{
	public static class UrlUtils
	{
		#region Misc
		public static string Discord { get { return "https://discord.gg/NF32j8S"; } }
		#endregion

		#region Site
		private static readonly string devilDaggersWebsiteBaseUrl = "https://devildaggers.info";
		#region Api
		public static string GetToolVersions { get { return $"{devilDaggersWebsiteBaseUrl}/API/GetToolVersions"; } }
		public static string GetSpawnsets { get { return $"{devilDaggersWebsiteBaseUrl}/API/GetSpawnsets"; } }
		public static string GetSpawnset(string fileName) { return $"{devilDaggersWebsiteBaseUrl}/API/GetSpawnset?fileName={fileName}"; }
		#endregion
		public static string Spawnsets { get { return $"{devilDaggersWebsiteBaseUrl}/Spawnsets"; } }
		public static string DownloadUrl(string versionOnline) { return $"{devilDaggersWebsiteBaseUrl}/tools/DevilDaggersSurvivalEditor/DevilDaggersSurvivalEditor{versionOnline}.zip"; }
		#endregion
	}
}