using DevilDaggersCore.Spawnsets;
using DevilDaggersSurvivalEditor.Code.Arena;
using DevilDaggersSurvivalEditor.Gui.Windows;
using Microsoft.Win32;
using System;
using System.IO;

namespace DevilDaggersSurvivalEditor.Code.Spawnsets
{
	public sealed class SpawnsetHandler
	{
		private bool unsavedChanges;

		// Must be a field since properties can't be used as out parameters.
		public Spawnset spawnset;

		private static readonly Lazy<SpawnsetHandler> lazy = new Lazy<SpawnsetHandler>(() => new SpawnsetHandler());

		private SpawnsetHandler()
		{
			spawnset = new Spawnset { ArenaTiles = ArenaPresetHandler.Instance.DefaultPreset.GetTiles() };
		}

		public static SpawnsetHandler Instance => lazy.Value;

		public bool HasUnsavedChanges
		{
			get => unsavedChanges;
			set
			{
				unsavedChanges = value;
				App.Instance.UpdateMainWindowTitle();
			}
		}

		public string SpawnsetFileName { get; private set; } = "(new spawnset)";
		public string SpawnsetFileLocation { get; private set; } = string.Empty;

		public void UpdateSpawnsetState(string name, string fileLocation)
		{
			HasUnsavedChanges = false;

			SpawnsetFileName = name;
			SpawnsetFileLocation = fileLocation;

			App.Instance.UpdateMainWindowTitle();
		}

		public void ProceedWithUnsavedChanges()
		{
			if (!HasUnsavedChanges)
				return;

			ConfirmWindow confirmWindow = new ConfirmWindow("Save changes?", "The current spawnset has unsaved changes. Save before proceeding?");
			confirmWindow.ShowDialog();

			if (confirmWindow.Confirmed)
				FileSave();
		}

		public void FileSave()
		{
			if (File.Exists(SpawnsetFileLocation))
			{
				if (SpawnsetFileUtils.TryWriteSpawnsetToFile(spawnset, SpawnsetFileLocation))
					HasUnsavedChanges = false;
			}
			else
			{
				FileSaveAs();
			}
		}

		public void FileSaveAs()
		{
			SaveFileDialog dialog = new SaveFileDialog();
			bool? result = dialog.ShowDialog();
			if (result.HasValue && result.Value && SpawnsetFileUtils.TryWriteSpawnsetToFile(spawnset, dialog.FileName))
				UpdateSpawnsetState(Path.GetFileName(dialog.FileName), dialog.FileName);
		}
	}
}