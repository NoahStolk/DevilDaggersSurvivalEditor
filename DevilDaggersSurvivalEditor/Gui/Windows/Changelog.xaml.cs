using DevilDaggersSurvivalEditor.Code.Clients;
using DevilDaggersSurvivalEditor.Code.Network;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class ChangelogWindow : Window
	{
		public ChangelogWindow()
		{
			InitializeComponent();

			if (NetworkHandler.Instance.Tool == null)
			{
				App.Log.Warn($"{nameof(ChangelogWindow)} was opened but tool info was not received.");
				Close();
				return;
			}

			int i = 0;
			foreach (ChangelogEntry entry in NetworkHandler.Instance.Tool.Changelog)
			{
				bool isLocalCurrentVersion = Version.Parse(entry.VersionNumber) == App.LocalVersion;
				SolidColorBrush color = new SolidColorBrush(isLocalCurrentVersion ? Color.FromRgb(207, 239, 207) : i++ % 2 == 0 ? Color.FromRgb(207, 207, 207) : Color.FromRgb(223, 223, 223));
				Border border = new Border { Padding = new Thickness(8, 16, 8, 16), Background = color };
				StackPanel entryStackPanel = new StackPanel { Background = color };
				if (isLocalCurrentVersion)
					entryStackPanel.Children.Add(new TextBlock { Text = "Currently running", FontSize = 12, FontWeight = FontWeights.Bold, Padding = new Thickness(6, 0, 0, 6), Foreground = new SolidColorBrush(Color.FromRgb(0, 127, 0)) });
				entryStackPanel.Children.Add(new TextBlock { Text = $"{entry.VersionNumber} - {entry.Date:MMMM dd, yyyy}", FontSize = 16, FontWeight = FontWeights.Bold, Padding = new Thickness(6, 0, 0, 6) });
				foreach (Change change in entry.Changes)
				{
					foreach (Grid stackPanel in GetGrids(change, 1))
						entryStackPanel.Children.Add(stackPanel);
				}

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
			{
				foreach (Change subChange in change.SubChanges)
				{
					foreach (Grid stackPanel in GetGrids(subChange, level))
						yield return stackPanel;
				}
			}
		}
	}
}