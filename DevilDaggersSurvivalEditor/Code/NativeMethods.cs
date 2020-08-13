using System;
using System.Runtime.InteropServices;

namespace DevilDaggersSurvivalEditor.Code
{
	internal static class NativeMethods
	{
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
	}
}