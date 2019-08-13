using System.Text;

namespace DevilDaggersSurvivalEditor.Code.CustomLeaderboards.MemoryHandling.Variables
{
	public class StringVariable : AbstractVariable<string>
	{
		public override string ValuePrevious => GetStringFromBytes(BytesPrevious);
		public override string Value => GetStringFromBytes(Bytes);

		public StringVariable(int localBaseAddress, int offset, uint maxSize)
			: base(localBaseAddress, offset, maxSize)
		{
		}

		private string GetStringFromBytes(byte[] bytes)
		{
			string str = Encoding.UTF8.GetString(bytes);
			return str.Substring(0, str.IndexOf('\0'));
		}
	}
}