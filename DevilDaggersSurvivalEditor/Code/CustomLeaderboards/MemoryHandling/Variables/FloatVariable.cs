using System;

namespace DevilDaggersSurvivalEditor.Code.CustomLeaderboards.MemoryHandling.Variables
{
	public class FloatVariable : AbstractVariable<float>
	{
		public override float ValuePrevious => BitConverter.ToSingle(BytesPrevious, 0);
		public override float Value => BitConverter.ToSingle(Bytes, 0);

		public FloatVariable(int localBaseAddress, int offset)
			: base(localBaseAddress, offset, sizeof(float))
		{
		}
	}
}