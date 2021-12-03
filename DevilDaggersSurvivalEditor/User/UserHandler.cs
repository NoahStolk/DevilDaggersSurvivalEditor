using Newtonsoft.Json;
using System;
using System.IO;

namespace DevilDaggersSurvivalEditor.User;

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

		Cache = JsonConvert.DeserializeObject<UserCache?>(File.ReadAllText(UserCache.FilePath)) ?? new();
	}

	public void ReadSettings()
	{
		if (!File.Exists(UserSettings.FilePath))
			return;

		Settings = JsonConvert.DeserializeObject<UserSettings?>(File.ReadAllText(UserSettings.FilePath)) ?? new();
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
