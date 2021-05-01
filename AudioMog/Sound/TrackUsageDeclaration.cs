using System.IO;

namespace AudioMog.Core.Sound
{
	public class TrackUsageDeclaration
	{
		public uint TrackIndex;

		public long DeclarationOffset;
		public byte IsLooping;
		public byte UnknownAt5;
		public ushort Id;
		public float PlayLength;


		public TrackUsageDeclaration(BinaryReader binaryReader, long offset)
		{
			DeclarationOffset = offset;
			TrackIndex = binaryReader.ReadUInt32At(offset);
			IsLooping = binaryReader.ReadByteAt(offset + 0x04);
			UnknownAt5 = binaryReader.ReadByteAt(offset + 0x05);
			Id = binaryReader.ReadUInt16At(offset + 0x06);
			PlayLength = binaryReader.ReadFloatAt(offset + 0x08);
		}
	}
}