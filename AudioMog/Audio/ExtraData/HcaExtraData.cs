using System.IO;

namespace AudioMog.Core.Audio.ExtraData
{
	public class HcaExtraData : ACodecExtraData
	{
		public byte Encryption;

		public HcaExtraData(MaterialSection.MaterialEntry entry, BinaryReader reader)
		{
			var extraDataOffset = entry.ExtraDataOffset;
			
			Encryption = reader.ReadByteAt(extraDataOffset + 0x0d);
			Size = ReadBigEndian16(reader, extraDataOffset, 0x16);
		}
	}
}