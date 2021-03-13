using Newtonsoft.Json;
using System;
using System.IO;

namespace DevilDaggersSurvivalEditor.User
{
	public sealed class UserHandler
	{
		private static readonly Lazy<UserHandler> _lazy = new(() => new());

		private UserHandler()
		{
		}

		public static UserHandler Instance => _lazy.Value;

		public UserCache Cache { get; set; } = new();
		public UserSettings Settings { get; set; } = new();

		public void ReadCache()
		{
			if (!File.Exists(UserCache.FilePath))
				return;

			using StreamReader sr = new(File.OpenRead(UserCache.FilePath));
			Cache = JsonConvert.DeserializeObject<UserCache>(sr.ReadToEnd());
		}

		public void ReadSettings()
		{
			if (!File.Exists(UserSettings.FilePath))
				return;

			using StreamReader sr = new(File.OpenRead(UserSettings.FilePath));
			Settings = JsonConvert.DeserializeObject<UserSettings>(sr.ReadToEnd());
		}

		public void SaveCache()
		{
			if (!Directory.Exists(UserCache.FileDirectory))
				Directory.CreateDirectory(UserCache.FileDirectory);

			using StreamWriter sw = new(File.Create(UserCache.FilePath));
			sw.Write(JsonConvert.SerializeObject(Cache, Formatting.Indented));
		}

		public void SaveSettings()
		{
			if (!Directory.Exists(UserSettings.FileDirectory))
				Directory.CreateDirectory(UserSettings.FileDirectory);

			using StreamWriter sw = new(File.Create(UserSettings.FilePath));
			sw.Write(JsonConvert.SerializeObject(Settings, Formatting.Indented));
		}
	}
}
