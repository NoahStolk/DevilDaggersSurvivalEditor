using DevilDaggersCore.Spawnset;
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

					using (FileStream fileStream = new FileStream(Program.App.userSettings.SurvivalFileLocation, FileMode.OpenOrCreate))
						fileStream.Write(data, 0, data.Length);
				}

				Program.App.ShowMessage("Success", $"Successfully wrote file to {Program.App.userSettings.SurvivalFileLocation}.");
			}
			catch (Exception ex)
			{
				Program.App.ShowError("An unexpected error occurred", $"Error while writing file to {Program.App.userSettings.SurvivalFileLocation}.", ex);
			}
		}
	}
}