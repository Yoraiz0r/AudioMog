using System.IO;

namespace AudioMog.Core.Music
{
	public class MusicInstrumentMaterial
	{
		public byte Version;
		public byte UnknownAt1; 
		public ushort MaterialSize;
		public ushort MaterialIndex;
		public ushort Id;
		public float Volume;
		public uint SyncPoint;
		public uint SampleRate;

		public void Read(BinaryReader binaryReader, long materialOffset)
		{
			Version = binaryReader.ReadByteAt(materialOffset);
			UnknownAt1 = binaryReader.ReadByteAt(materialOffset + 0x01);

			MaterialSize = binaryReader.ReadUInt16At(materialOffset + 0x02);
			MaterialIndex = binaryReader.ReadUInt16At(materialOffset + 0x04);

			Id = binaryReader.ReadUInt16At(materialOffset + 0x06);
			Volume = binaryReader.ReadFloatAt(materialOffset + 0x08);
			SyncPoint = binaryReader.ReadUInt32At(materialOffset + 0x0c);
			SampleRate = binaryReader.ReadUInt32At(materialOffset + 0x10);
		}
	}
}