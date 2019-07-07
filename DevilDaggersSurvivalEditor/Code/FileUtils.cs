using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code.User;
using System;
using System.IO;

namespace DevilDaggersSurvivalEditor.Code
{
	public static class FileUtils
	{
		public static void WriteSpawnsetToFile(Spawnset spawnset, string destinationPath)
		{
			if (spawnset.TryGetBytes(out byte[] bytes))
			{
				File.WriteAllBytes(destinationPath, bytes);
				Program.App.ShowMessage("Success", $"Successfully wrote file to {destinationPath}.");
			}
			else
			{
				Program.App.ShowError("An unexpected error occurred", $"Error while writing file to {destinationPath}.");
			}
		}

		public static void RestoreSurvivalFile()
		{
			try
			{
				using (Stream stream = Program.App.Assembly.GetManifestResourceStream("DevilDaggersSurvivalEditor.Content.survival"))
				{
					byte[] data = new byte[stream.Length];
					using (BinaryReader reader = new BinaryReader(stream))
						reader.Read(data, 0, data.Length);

					using (FileStream fileStream = new FileStream(UserSettingsHandler.Instance.userSettings.SurvivalFileLocation, FileMode.OpenOrCreate))
						fileStream.Write(data, 0, data.Length);
				}

				Program.App.ShowMessage("Success", $"Successfully wrote file to {UserSettingsHandler.Instance.userSettings.SurvivalFileLocation}.");
			}
			catch (Exception ex)
			{
				Program.App.ShowError("An unexpected error occurred", $"Error while writing file to {UserSettingsHandler.Instance.userSettings.SurvivalFileLocation}.", ex);
			}
		}
	}
}