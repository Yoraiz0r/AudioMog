using System.IO;

namespace AudioMog.Core.Audio.ExtraData
{
	public abstract class ACodecExtraData
	{
		public ushort Size { get; protected set; }
		

		protected ushort ReadBigEndian16(BinaryReader reader, long extraDataOffset, long offset)
		{
			var bigEndian1 = reader.ReadByteAt(extraDataOffset + offset);
			var bigEndian2 = reader.ReadByteAt(extraDataOffset + offset + 0x01);
			var littleEndian = (ushort) ((bigEndian1 << 8) + bigEndian2);
			return littleEndian;
		}
		protected uint ReadBigEndian32(BinaryReader reader, long extraDataOffset, long offset)
		{
			var bigEndian1 = reader.ReadByteAt(extraDataOffset + offset);
			var bigEndian2 = reader.ReadByteAt(extraDataOffset + offset + 0x01);
			var bigEndian3 = reader.ReadByteAt(extraDataOffset + offset + 0x02);
			var bigEndian4 = reader.ReadByteAt(extraDataOffset + offset + 0x03);
			var badEndian = reader.ReadUInt32At(extraDataOffset + offset);
			var littleEndian = (uint)(
				(bigEndian1 << 24) 
				+ (bigEndian2 << 16) 
				+ (bigEndian3 << 8) 
				+ bigEndian4);
			return littleEndian;
		}
	}
}