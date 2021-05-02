using System.IO;

namespace AudioMog.Core.Audio
{
	public class AudioBinaryFileHeader
	{
		public uint Magic;

		public byte VersionMain;
		public byte VersionSub;

		public ushort UnknownAt6;

		public byte SectionsCount;

		public byte DescriptorLength;

		public ushort UnknownAtA;

		public uint FileSize;
		public int HeaderSize;


		public bool IsBigEndian => UnknownAt6 == 0x1000;

		public AudioBinaryFileHeader(BinaryReader reader, long offsetForFileStart)
		{
			Magic = reader.ReadUInt32At(offsetForFileStart);
			VersionMain = reader.ReadByteAt(offsetForFileStart + 0x04);
			VersionSub = reader.ReadByteAt(offsetForFileStart + 0x05);
			UnknownAt6 = reader.ReadUInt16At(offsetForFileStart + 0x06);
			SectionsCount = reader.ReadByteAt(offsetForFileStart + 0x08);
			DescriptorLength = reader.ReadByteAt(offsetForFileStart + 0x09);
			UnknownAtA = reader.ReadUInt16At(offsetForFileStart + 0x0a);
			FileSize = reader.ReadUInt32At(offsetForFileStart + 0x0c);

			int bytesNeededToPad = 16 - DescriptorLength % 16;
			HeaderSize = 16 + DescriptorLength + bytesNeededToPad;
		}
	}
}