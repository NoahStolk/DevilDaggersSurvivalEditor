using DevilDaggersCore.Website.Models;
using DevilDaggersSurvivalEditor.Code.Network;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class ChangelogWindow : Window
	{
		public ChangelogWindow()
		{
			InitializeComponent();

			int i = 0;
			foreach (ChangeLogEntry entry in NetworkHandler.Instance.VersionResult.Tool.ChangeLog)
			{
				bool isLocal = entry.VersionNumber == App.LocalVersion;
				SolidColorBrush color = new SolidColorBrush(isLocal ? Color.FromRgb(208, 240, 208) : i++ % 2 == 0 ? Color.FromRgb(208, 208, 208) : Color.FromRgb(224, 224, 224));
				Border border = new Border { Padding = new Thickness(8, 16, 8, 16), Background = color };
				StackPanel entryStackPanel = new StackPanel { Background = color };
				if (isLocal)
					entryStackPanel.Children.Add(new TextBlock { Text = "Currently running", FontSize = 12, FontWeight = FontWeights.Bold, Padding = new Thickness(6, 0, 0, 6), Foreground = new SolidColorBrush(Color.FromRgb(0, 128, 0)) });
				entryStackPanel.Children.Add(new TextBlock { Text = $"{entry.VersionNumber} - {entry.Date.ToString("MMMM dd, yyyy")}", FontSize = 16, FontWeight = FontWeights.Bold, Padding = new Thickness(6, 0, 0, 6) });
				foreach (Change change in entry.Changes)
					foreach (Grid stackPanel in GetGrids(change, 1))
						entryStackPanel.Children.Add(stackPanel);
				border.Child = entryStackPanel;
				Main.Children.Add(border);
			}
		}

		private IEnumerable<Grid> GetGrids(Change change, int level)
		{
			Grid changeGrid = new Grid();
			changeGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(level++ * 32) });
			changeGrid.ColumnDefinitions.Add(new ColumnDefinition());

			changeGrid.Children.Add(new TextBlock { Text = "• ", TextAlignment = TextAlignment.Right });

			TextBlock descriptionTextBlock = new TextBlock { Text = change.Description, TextWrapping = TextWrapping.WrapWithOverflow };
			Grid.SetColumn(descriptionTextBlock, 1);
			changeGrid.Children.Add(descriptionTextBlock);

			yield return changeGrid;

			if (change.SubChanges != null)
				foreach (Change subChange in change.SubChanges)
					foreach (Grid stackPanel in GetGrids(subChange, level))
						yield return stackPanel;
		}
	}
}