using System.IO;

namespace AudioMog.Core.Sound
{
	public class TrackEntry
	{
		public long Offset;
		public byte Version;
		public byte Type;
		public ushort Size;
		public ushort Id;
		public ushort ChildId;
		
		public ushort MaterialIndex;
		public bool HasMaterial => Type == (int) TrackEntryType.Material;
		public TrackEntry(SoundAudioBinaryFile file, BinaryReader reader, int index)
		{
			var trackSectionOffset = file.TrackSectionOffset;
			var trackOffsetInSection = reader.ReadUInt32At(trackSectionOffset + 0x10 + index * 0x04);
			var trackOffsetInFile = trackSectionOffset + trackOffsetInSection;

			Offset = trackOffsetInFile;
			Version = reader.ReadByteAt(Offset);
			Type = reader.ReadByteAt(Offset + 0x01);
			Size = reader.ReadUInt16At(Offset + 0x02);
			Id = reader.ReadUInt16At(Offset + 0x08);
			ChildId = reader.ReadUInt16At(Offset + 0x0a);
			if (HasMaterial)
				MaterialIndex = reader.ReadUInt16At(Offset + 0x04);
		}
	}
}