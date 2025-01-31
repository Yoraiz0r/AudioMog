using System.IO;

namespace AudioMog.Core.Audio.ExtraData
{
	public class Atrac9ExtraData : ACodecExtraData
	{
		public Atrac9ExtraData(MaterialSection.MaterialEntry entry, BinaryReader reader)
		{
			var extraDataOffset = entry.ExtraDataOffset;

			var version = reader.ReadByteAt(extraDataOffset);
			var reserved = reader.ReadByteAt(extraDataOffset + 0x01);
			var size = reader.ReadUInt16At(extraDataOffset + 0x02);
			var blockAlign = reader.ReadUInt16At(extraDataOffset + 0x04);
			var blockSamples = reader.ReadUInt16At(extraDataOffset + 0x06);
			var channelLayout = reader.ReadUInt32At(extraDataOffset + 0x08);
			var config = reader.ReadUInt32At(extraDataOffset + 0x0c);
			var samples = reader.ReadUInt32At(extraDataOffset + 0x10);
			var overlapDelay = reader.ReadUInt32At(extraDataOffset + 0x14);
			var encoderDelay = reader.ReadUInt32At(extraDataOffset + 0x18);
			var sampleRate = reader.ReadUInt32At(extraDataOffset + 0x1c);
			var loopStart = reader.ReadUInt32At(extraDataOffset + 0x20);
			var loopEnd = reader.ReadUInt32At(extraDataOffset + 0x24);

			Size = size;
		}
	}
}