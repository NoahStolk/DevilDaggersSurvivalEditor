using System;
using System.Diagnostics;

namespace DevilDaggersSurvivalEditor.Code.CustomLeaderboards.MemoryHandling
{
	public class Memory
	{
		public Process ReadProcess { get; set; }

		private IntPtr hProcess = IntPtr.Zero;

		public void Open()
		{
			MemoryAPI.ProcessAccessType access = MemoryAPI.ProcessAccessType.PROCESS_VM_READ
			| MemoryAPI.ProcessAccessType.PROCESS_VM_WRITE
			| MemoryAPI.ProcessAccessType.PROCESS_VM_OPERATION;
			hProcess = MemoryAPI.OpenProcess((uint)access, 1, (uint)ReadProcess.Id);
		}

		public void CloseHandle()
		{
			int iRetValue;
			iRetValue = MemoryAPI.CloseHandle(hProcess);
			if (iRetValue == 0)
				throw new Exception("CloseHandle Failed");
		}

		public byte[] Read(IntPtr memoryAddress, uint bytesToRead, out int bytesRead)
		{
			byte[] buffer = new byte[bytesToRead];
			MemoryAPI.ReadProcessMemory(hProcess, memoryAddress, buffer, bytesToRead, out IntPtr ptrBytesRead);
			bytesRead = ptrBytesRead.ToInt32();
			return buffer;
		}

		public byte[] PointerRead(IntPtr memoryAddress, uint bytesToRead, int[] offset, out int bytesRead)
		{
			int iPointerCount = offset.Length - 1;
			IntPtr ptrBytesRead;
			bytesRead = 0;
			byte[] buffer = new byte[4]; // DWORD to hold an Address
			int tempAddress = 0;

			if (iPointerCount == 0)
			{
				MemoryAPI.ReadProcessMemory(hProcess, memoryAddress, buffer, 4, out _);
				tempAddress = AddressUtils.ToDec(AddressUtils.MakeAddress(buffer)) + offset[0]; // Final Address

				buffer = new byte[bytesToRead];
				MemoryAPI.ReadProcessMemory(hProcess, (IntPtr)tempAddress, buffer, bytesToRead, out ptrBytesRead);

				bytesRead = ptrBytesRead.ToInt32();
				return buffer;
			}

			for (int i = 0; i <= iPointerCount; i++)
			{
				if (i == iPointerCount)
				{
					MemoryAPI.ReadProcessMemory(hProcess, (IntPtr)tempAddress, buffer, 4, out _);
					tempAddress = AddressUtils.ToDec(AddressUtils.MakeAddress(buffer)) + offset[i]; // Final Address

					buffer = new byte[bytesToRead];
					MemoryAPI.ReadProcessMemory(hProcess, (IntPtr)tempAddress, buffer, bytesToRead, out ptrBytesRead);

					bytesRead = ptrBytesRead.ToInt32();
					return buffer;
				}
				else if (i == 0)
				{
					MemoryAPI.ReadProcessMemory(hProcess, memoryAddress, buffer, 4, out _);
					tempAddress = AddressUtils.ToDec(AddressUtils.MakeAddress(buffer)) + offset[1];
				}
				else
				{
					MemoryAPI.ReadProcessMemory(hProcess, (IntPtr)tempAddress, buffer, 4, out _);
					tempAddress = AddressUtils.ToDec(AddressUtils.MakeAddress(buffer)) + offset[i];
				}
			}

			return buffer;
		}

		public void Write(IntPtr memoryAddress, byte[] bytesToWrite, out int bytesWritten)
		{
			MemoryAPI.WriteProcessMemory(hProcess, memoryAddress, bytesToWrite, (uint)bytesToWrite.Length, out IntPtr ptrBytesWritten);
			bytesWritten = ptrBytesWritten.ToInt32();
		}

		public string PointerWrite(IntPtr memoryAddress, byte[] bytesToWrite, int[] Offset, out int bytesWritten)
		{
			int iPointerCount = Offset.Length - 1;
			IntPtr ptrBytesWritten;
			bytesWritten = 0;
			byte[] buffer = new byte[4]; // DWORD to hold an Address
			int tempAddress = 0;

			if (iPointerCount == 0)
			{
				MemoryAPI.ReadProcessMemory(hProcess, memoryAddress, buffer, 4, out _);
				tempAddress = AddressUtils.ToDec(AddressUtils.MakeAddress(buffer)) + Offset[0]; // Final Address
				MemoryAPI.WriteProcessMemory(hProcess, (IntPtr)tempAddress, bytesToWrite, (uint)bytesToWrite.Length, out ptrBytesWritten);

				bytesWritten = ptrBytesWritten.ToInt32();
				return AddressUtils.ToHex(tempAddress);
			}

			for (int i = 0; i <= iPointerCount; i++)
			{
				if (i == iPointerCount)
				{
					MemoryAPI.ReadProcessMemory(hProcess, (IntPtr)tempAddress, buffer, 4, out _);
					tempAddress = AddressUtils.ToDec(AddressUtils.MakeAddress(buffer)) + Offset[i]; // Final Address
					MemoryAPI.WriteProcessMemory(hProcess, (IntPtr)tempAddress, bytesToWrite, (uint)bytesToWrite.Length, out ptrBytesWritten);

					bytesWritten = ptrBytesWritten.ToInt32();
					return AddressUtils.ToHex(tempAddress);
				}
				else if (i == 0)
				{
					MemoryAPI.ReadProcessMemory(hProcess, memoryAddress, buffer, 4, out _);
					tempAddress = AddressUtils.ToDec(AddressUtils.MakeAddress(buffer)) + Offset[i];
				}
				else
				{
					MemoryAPI.ReadProcessMemory(hProcess, (IntPtr)tempAddress, buffer, 4, out _);
					tempAddress = AddressUtils.ToDec(AddressUtils.MakeAddress(buffer)) + Offset[i];
				}
			}

			return AddressUtils.ToHex(tempAddress);
		}
	}
}