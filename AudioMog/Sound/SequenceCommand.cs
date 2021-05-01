using System.IO;

namespace AudioMog.Core.Sound
{
	public class SequenceCommand
	{
		public byte Version;
		public byte Size;
		public byte Type;
		public byte BodySize;

		public TrackUsageDeclaration Body;

		public SequenceCommand(BinaryReader binaryReader, long offset)
		{
			Version = binaryReader.ReadByteAt(offset);
			Size = binaryReader.ReadByteAt(offset + 0x01);
			Type = binaryReader.ReadByteAt(offset + 0x02);
			BodySize = binaryReader.ReadByteAt(offset + 0x03);

			if (Type == (int) SequenceCommandType.KeyOn)
				Body = new TrackUsageDeclaration(binaryReader, offset + Size);
		}
	}
}