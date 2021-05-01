using System.IO;

namespace AudioMog.Core.Music
{
	public class MusicLayer
	{
		public int Index;
		public long Offset;
		
		public byte Version;
		public byte Flags;
		public ushort Size;
		public ushort MaterialIndex;
		public ushort LoopCount;
		public uint UnknownAt8;
		public uint EndPointSample;

		public void Read(BinaryReader binaryReader, MusicSlice slice, int layerIndex)
		{
			Index = layerIndex;
			Offset = slice.Offset +
			              binaryReader.ReadUInt32At(slice.SubTableOffset + layerIndex * 0x04);

			Version = binaryReader.ReadByteAt(Offset);
			Flags = binaryReader.ReadByteAt(Offset + 0x01);
			Size = binaryReader.ReadUInt16At(Offset + 0x02);
			MaterialIndex = binaryReader.ReadUInt16At(Offset + 0x04);
			LoopCount = binaryReader.ReadUInt16At(Offset + 0x06);
			UnknownAt8 = binaryReader.ReadUInt32At(Offset + 0x08);
			EndPointSample = binaryReader.ReadUInt32At(Offset + 0x0c);
		}

		public void RunFixes(MusicSliceFixer info)
		{
			info.TryReplacing(Offset + 0x0c, info.ExitSample, addedValue: 1);
		}
	}
}