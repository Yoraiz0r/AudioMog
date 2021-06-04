using System.IO;

namespace AudioMog.Core.Audio.ExtraData
{
	public class OggVorbisExtraData : ACodecExtraData
	{
		public byte Version;
		public uint LoopStart;
		public uint LoopEnd;
		public uint NumberOfSamples;
		public uint HeaderSize;
		public uint SeekTableSize;
		public ushort UnknownAt18;

		public OggVorbisExtraData(MaterialSection.MaterialEntry entry, BinaryReader reader)
		{
			var extraDataOffset = entry.ExtraDataOffset;
			
			/* extradata: */
			/* 0x00: version */
			/* 0x01: reserved */
			/* 0x02: size */
			/* 0x04: loop start offset */
			/* 0x08: loop end offset */
			/* 0x0c: num samples */
			/* 0x10: header size */
			/* 0x14: seek table size */
			/* 0x18: reserved x2 */
			/* 0x20: seek table */

			Version = reader.ReadByteAt(extraDataOffset);
			var unknownAt1 = reader.ReadByteAt(extraDataOffset + 0x01);
			var badSize = reader.ReadUInt16At(extraDataOffset + 0x02);
			LoopStart = reader.ReadUInt32At(extraDataOffset + 0x04);
			LoopEnd = reader.ReadUInt32At(extraDataOffset + 0x08);
			NumberOfSamples = reader.ReadUInt32At(extraDataOffset + 0x0c);
			HeaderSize = reader.ReadUInt32At(extraDataOffset + 0x10);
			SeekTableSize = reader.ReadUInt32At(extraDataOffset + 0x14);
			UnknownAt18 = reader.ReadUInt16At(extraDataOffset + 0x18);
			//seek table at 20
		}
	}
}