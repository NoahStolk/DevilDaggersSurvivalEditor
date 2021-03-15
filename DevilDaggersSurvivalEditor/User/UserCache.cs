using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DevilDaggersSurvivalEditor.User
{
	[JsonObject(MemberSerialization.OptIn)]
	public class UserCache
	{
		private const string _fileName = "cache.json";

		public static string FileDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DevilDaggersSurvivalEditor");
		public static string FilePath => Path.Combine(FileDirectory, _fileName);

		[JsonProperty]
		public string? DownloadAuthorFilter { get; set; }
		[JsonProperty]
		public string? DownloadSpawnsetFilter { get; set; }
		[JsonProperty]
		public bool DownloadCustomLeaderboardFilter { get; set; }
		[JsonProperty]
		public bool DownloadPracticeFilter { get; set; }
		[JsonProperty]
		public int? DownloadSortingIndex { get; set; }
		[JsonProperty]
		public List<bool>? DownloadSortingDirections { get; set; }
	}
}
