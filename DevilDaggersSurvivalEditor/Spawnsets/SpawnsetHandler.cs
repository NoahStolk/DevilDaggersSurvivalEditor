using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Wpf.Windows;
using DevilDaggersSurvivalEditor.Arena;
using DevilDaggersSurvivalEditor.Utils;
using Microsoft.Win32;
using System;
using System.IO;

namespace DevilDaggersSurvivalEditor.Spawnsets
{
	public sealed class SpawnsetHandler
	{
		private bool _hasUnsavedChanges;

		private static readonly Lazy<SpawnsetHandler> _lazy = new Lazy<SpawnsetHandler>(() => new SpawnsetHandler());

		private SpawnsetHandler()
		{
			Spawnset = new Spawnset { ArenaTiles = ArenaPresetHandler.Instance.DefaultPreset.GetTiles() };
		}

		public static SpawnsetHandler Instance => _lazy.Value;

		public Spawnset Spawnset { get; set; }

		public bool HasUnsavedChanges
		{
			get => _hasUnsavedChanges;
			set
			{
				_hasUnsavedChanges = value;
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

			ConfirmWindow confirmWindow = new ConfirmWindow("Save changes?", "The current spawnset has unsaved changes. Save before proceeding?", false);
			confirmWindow.ShowDialog();

			if (confirmWindow.IsConfirmed)
				FileSave();
		}

		public void FileSave()
		{
			if (File.Exists(SpawnsetFileLocation))
			{
				if (SpawnsetFileUtils.TryWriteSpawnsetToFile(Spawnset, SpawnsetFileLocation))
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
			if (result.HasValue && result.Value && SpawnsetFileUtils.TryWriteSpawnsetToFile(Spawnset, dialog.FileName))
				UpdateSpawnsetState(Path.GetFileName(dialog.FileName), dialog.FileName);
		}
	}
}