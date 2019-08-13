using System;

namespace DevilDaggersSurvivalEditor.Code.CustomLeaderboards.MemoryHandling.Variables
{
	public abstract class AbstractVariable<T>
	{
		private const uint PointerSize = 4; // For 32-bit applications

		protected byte[] BytesPrevious { get; private set; }
		protected byte[] Bytes { get; private set; }
		public abstract T ValuePrevious { get; }
		public abstract T Value { get; }

		public int LocalBaseAddress { get; set; }
		public int Offset { get; set; }
		public uint Size { get; set; }

		public AbstractVariable(int localBaseAddress, int offset, uint size)
		{
			LocalBaseAddress = localBaseAddress;
			Offset = offset;
			Size = size;

			BytesPrevious = new byte[Size];
			Bytes = new byte[Size];
		}

		public void PreScan()
		{
			BytesPrevious = Bytes;
		}

		/// <summary>
		/// Gets the bytes for this <see cref="AbstractGameVariable"/>.
		/// 
		/// Process.MainModule.BaseAddress is where the process has its memory start point.
		/// <see cref="LocalBaseAddress"/> bytes ahead of the process base address brings us to 4 bytes (for a 32-bit application), which contain a memory address.
		/// 
		/// Use that memory address and add the <see cref="Offset"/> to it to get to the bytes that contain the actual value.
		/// Note that in the second read the process's base address is not needed.
		/// </summary>
		public void Scan()
		{
			try
			{
				Memory memory = Scanner.Instance.Memory;

				byte[] bytes = memory.Read(memory.ReadProcess.MainModule.BaseAddress + LocalBaseAddress, PointerSize, out _);
				int ptr = AddressUtils.ToDec(AddressUtils.MakeAddress(bytes));

				Bytes = memory.Read(new IntPtr(ptr) + Offset, Size, out _);
			}
			catch (Exception ex)
			{
				Program.App.LogError($"Error scanning {typeof(T)} variable", ex);
			}
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}
}