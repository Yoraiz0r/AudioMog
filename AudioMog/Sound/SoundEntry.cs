using System.Collections.Generic;
using System.IO;
using System.Text;
using AudioMog.Core.Audio;

namespace AudioMog.Core.Sound
{
	public class SoundEntry : INamedEntry
	{
		public uint Offset;
		public byte Version;
		public byte UnknownAt1;
		public ushort Size;
		public byte Type;
		public byte SequenceCount;
		public ushort SequencesOffsetInEntry;
		public int NameSize;
		public uint NameOffset;
		public string Name;

		public string DisplayName => Name;
		public List<SequenceEntry> Sequences = new List<SequenceEntry>();

		public SoundEntry(SoundAudioBinaryFile file, BinaryReader binaryReader, int entryIndex)
		{
			Offset = (uint)file.SoundSectionOffset + binaryReader.ReadUInt32At(file.SoundSectionOffset + 0x10 + entryIndex * 0x04);

			Version = binaryReader.ReadByteAt(Offset);
			UnknownAt1 = binaryReader.ReadByteAt(Offset + 0x01);

			Size = binaryReader.ReadUInt16At(Offset + 0x02);
			Type = binaryReader.ReadByteAt(Offset + 0x04);

			SequenceCount = binaryReader.ReadByte();

			SequencesOffsetInEntry = binaryReader.ReadUInt16At(Offset + 0x1a);

			if (Version <= 8)
			{
				NameSize = 0x0f;
				NameOffset = Offset + 0x50;
			}
			else
			{
				NameSize = binaryReader.ReadByteAt(Offset + 0x23);
				NameOffset = Offset + Size;
			}

			Name = Encoding.ASCII.GetString(binaryReader.ReadBytesAt(NameOffset, NameSize));

			var currentSequenceOffset = Offset + SequencesOffsetInEntry;
			for (int sequenceIndex = 0; sequenceIndex < SequenceCount; sequenceIndex++)
			{
				var sequence = new SequenceEntry(file, binaryReader, this, currentSequenceOffset);
				
				Sequences.Add(sequence);
				
				currentSequenceOffset += sequence.DeclarationSize;
			}
		}
	}
}