using System;

namespace AudioMog.Application.AudioFileRebuilder.Steps
{
	public abstract class ARebuilderStep
	{
		public abstract void Run(Blackboard blackboard);
		protected void WriteUint(byte[] bytes, int offset, uint value)
		{
			var sizeBytes = BitConverter.GetBytes(value);
			Buffer.BlockCopy(sizeBytes, 0, bytes, offset, sizeBytes.Length);
		}
		protected void WriteUshort(byte[] bytes, int offset, ushort value)
		{
			var sizeBytes = BitConverter.GetBytes(value);
			Buffer.BlockCopy(sizeBytes, 0, bytes, offset, sizeBytes.Length);
		}
		protected void WriteByte(byte[] bytes, int offset, byte value)
		{
			bytes[offset] = value;
		}

		protected bool WriteUintIfMatch(byte[] bytes, int offset, uint matchValue, uint writeValue)
		{
			var matches = ReadUint(bytes, offset) == matchValue;
			if (matches)
				WriteUint(bytes, offset, writeValue);
			return matches;
		}
		
		protected uint ReadUint(byte[] bytes, int offset)
		{
			return BitConverter.ToUInt32(bytes, offset);
		}
	}
}