using System.Collections.Generic;
using System.IO;
using System.Text;
using AudioMog.Core.Audio;

namespace AudioMog.Core.Music
{
	public class MusicInstrument : INamedEntry
	{
		public long Offset;

		public byte Version;
		public byte UnknownAt1;
		public ushort Size;
		public byte Type;
		public byte Category;
		public byte Priority;
		public ushort UnknownAt8;
		public byte Flags;
		public byte DistanceAttenuationCurve;
		public float InteriorFactor;
		public float AudibleRange;
		public float InnerRange;
		public float PlayLength;
		public int NameSize;
		public long NameOffset;
		public string Name;
		public string DisplayName => Name;

		public List<MusicInstrumentMaterial> Materials = new List<MusicInstrumentMaterial>();

		public MusicInstrument(MusicAudioBinaryFile file, BinaryReader binaryReader, int entryIndex)
		{
			Offset = file.InstrumentSectionOffset +
			         binaryReader.ReadUInt32At(file.InstrumentSectionOffset + 0x10 + entryIndex * 0x04);

			Version = binaryReader.ReadByteAt(Offset);
			UnknownAt1 = binaryReader.ReadByteAt(Offset + 1);
			Size = binaryReader.ReadUInt16At(Offset + 2);
			Type = binaryReader.ReadByteAt(Offset + 4);
			var materialCount = binaryReader.ReadByteAt(Offset + 0x05);
			Category = binaryReader.ReadByteAt(Offset + 0x06);
			Priority = binaryReader.ReadByteAt(Offset + 0x07);
			UnknownAt8 = binaryReader.ReadUInt16At(Offset + 0x08);
			Flags = binaryReader.ReadByteAt(Offset + 0x0a);
			DistanceAttenuationCurve = binaryReader.ReadByteAt(Offset + 0x0b);
			InteriorFactor = binaryReader.ReadFloatAt(Offset + 0x0c);
			AudibleRange = binaryReader.ReadFloatAt(Offset + 0x10);
			InnerRange = binaryReader.ReadFloatAt(Offset + 0x14);
			PlayLength = binaryReader.ReadFloatAt(Offset + 0x18);

			NameOffset = Offset + 0x30;
			NameSize = 15;

			Name = Encoding.ASCII.GetString(binaryReader.ReadBytesAt(NameOffset, NameSize));

			var materialOffset = NameOffset + NameSize + 0x01;
			for (int materialIndex = 0; materialIndex < materialCount; materialIndex++)
			{
				var material = new MusicInstrumentMaterial();
				material.Read(binaryReader, materialOffset);
				materialOffset += material.MaterialSize;
			}
		}

	}
}