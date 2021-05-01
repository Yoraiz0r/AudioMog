using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AudioMog.Core.Music
{
	public class MusicEntry
	{
		public int Index;
		public byte Version;
		public byte Flags;
		public long Offset;
		public ushort Size;
		public byte SliceCount;
		public byte ModeCount;
		public long TableOffset;

		public byte NameSize;
		public long NameOffset;
		public string Name;

		public List<MusicSlice> Slices = new List<MusicSlice>();
		public List<MusicMode> Modes = new List<MusicMode>();

		public MusicEntry(MusicAudioBinaryFile file, BinaryReader binaryReader, int entryIndex)
		{
			Index = entryIndex;
			Offset = file.MusicSectionOffset +
			             binaryReader.ReadUInt32At(file.MusicSectionOffset + 0x10 + entryIndex * 0x04);

			Version = binaryReader.ReadByteAt(Offset);
			Flags = binaryReader.ReadByte();
			Size = binaryReader.ReadUInt16();
			SliceCount = binaryReader.ReadByte();
			ModeCount = binaryReader.ReadByte();
			NameSize = binaryReader.ReadByteAt(Offset + 0x48);

			if (Version <= 8)
			{
				NameOffset = Offset + 0x10;
				Size = 0x0f;
			}
			else
				NameOffset = Offset + Size;

			Name = Encoding.ASCII.GetString(binaryReader.ReadBytesAt(NameOffset, NameSize));

			TableOffset = file.AlignToBlockStart(NameOffset + NameSize + 0x0f);

			for (int sliceIndex = 0; sliceIndex < SliceCount; sliceIndex++)
			{
				var slice = new MusicSlice(binaryReader, file, this, sliceIndex);
				Slices.Add(slice);
			}

			for (int modeIndex = 0; modeIndex < ModeCount; modeIndex++)
			{
				var section = new MusicMode(this, binaryReader, modeIndex);
				Modes.Add(section);
			}
		}
	}
}