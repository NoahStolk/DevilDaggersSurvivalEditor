using System;
using System.Runtime.InteropServices;

namespace DevilDaggersSurvivalEditor.Code.CustomLeaderboards.MemoryHandling
{
	public class MemoryAPI
	{
		[Flags]
		public enum ProcessAccessType
		{
			PROCESS_TERMINATE = 0x0001,
			PROCESS_CREATE_THREAD = 0x0002,
			PROCESS_SET_SESSIONID = 0x0004,
			PROCESS_VM_OPERATION = 0x0008,
			PROCESS_VM_READ = 0x0010,
			PROCESS_VM_WRITE = 0x0020,
			PROCESS_DUP_HANDLE = 0x0040,
			PROCESS_CREATE_PROCESS = 0x0080,
			PROCESS_SET_QUOTA = 0x0100,
			PROCESS_SET_INFORMATION = 0x0200,
			PROCESS_QUERY_INFORMATION = 0x0400
		}

		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

		[DllImport("kernel32.dll")]
		public static extern int CloseHandle(IntPtr hObject);

		[DllImport("kernel32.dll")]
		public static extern int ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, uint size, out IntPtr lpNumberOfBytesRead);

		[DllImport("kernel32.dll")]
		public static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, uint size, out IntPtr lpNumberOfBytesWritten);
	}
}