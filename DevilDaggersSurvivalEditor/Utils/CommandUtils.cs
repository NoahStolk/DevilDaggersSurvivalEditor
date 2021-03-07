using System.Windows.Input;

namespace DevilDaggersSurvivalEditor.Utils
{
	public static class CommandUtils
	{
		public static readonly RoutedUICommand Exit = new
		(
			"Exit",
			"Exit",
			typeof(CommandUtils),
			new InputGestureCollection
			{
				new KeyGesture(Key.F4, ModifierKeys.Alt),
			}
		);
		public static readonly RoutedUICommand Save = new
		(
			"Save",
			"Save",
			typeof(CommandUtils),
			new InputGestureCollection
			{
				new KeyGesture(Key.S, ModifierKeys.Control),
			}
		);
	}
}
