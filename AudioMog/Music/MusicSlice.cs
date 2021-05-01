using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AudioMog.Core.Music
{
	public class MusicSlice
	{
		public int Index;
		public byte Version;
		public ushort Size;
		public long SubTableOffset;
		public long Offset;
		public string Name;
		public ushort CustomPointsCount;
		public uint EntryPointsSample;
		public uint ExitPointsSample;
		public uint LoopStart;
		public uint LoopEnd;
		public uint MeterCount;
		public long NameOffset;
		public int NameSize;

		public List<MusicLayer> Layers = new List<MusicLayer>();


		public MusicSlice(BinaryReader binaryReader, MusicAudioBinaryFile file, MusicEntry entry, int sliceIndex)
		{
			Index = sliceIndex;
			Offset = entry.Offset + binaryReader.ReadUInt32At(entry.TableOffset + sliceIndex * 0x04);
			Version = binaryReader.ReadByteAt(Offset);
			Size = binaryReader.ReadUInt16At(Offset + 0x02);

			int layerCount;
			if (Version <= 7)
			{
				/* 0x04: meter count */
				layerCount = binaryReader.ReadByteAt(Offset + 0x05);
				CustomPointsCount = binaryReader.ReadUInt16At(Offset + 0x06);
				EntryPointsSample = binaryReader.ReadUInt32At(Offset + 0x08);
				ExitPointsSample = binaryReader.ReadUInt32At(Offset + 0x0c);
				LoopStart = binaryReader.ReadUInt32At(Offset + 0x10);
				LoopEnd = binaryReader.ReadUInt32At(Offset + 0x14);
				/* 0x18+: meter transition timing info (offsets, points, curves, etc) */
				NameOffset = 0x30;
				NameSize = 0x0f;
				SubTableOffset = Offset + Size;
			}
			else
			{
				NameSize = binaryReader.ReadByteAt(Offset + 0x04);
				layerCount = binaryReader.ReadByteAt(Offset + 0x05);
				CustomPointsCount = binaryReader.ReadUInt16At(Offset + 0x06);
				EntryPointsSample = binaryReader.ReadUInt32At(Offset + 0x08);
				ExitPointsSample = binaryReader.ReadUInt32At(Offset + 0x0c);
				LoopStart = binaryReader.ReadUInt32At(Offset + 0x10);
				LoopEnd = binaryReader.ReadUInt32At(Offset + 0x14);
				MeterCount = binaryReader.ReadUInt32At(Offset + 0x18);
				/* 0x18: meter count */
				/* 0x1c+: meter transition timing info (offsets, points, curves, etc) */
				NameOffset = Offset + Size;
				SubTableOffset = file.AlignToBlockStart(NameOffset + NameSize + 0x0f);
			}
				
				
			Name = Encoding.ASCII.GetString(binaryReader.ReadBytesAt(NameOffset, NameSize));
				
			for (int layerIndex = 0; layerIndex < layerCount; layerIndex++)
			{
				var musicSectionlayer = new MusicLayer();
				musicSectionlayer.Read(binaryReader, this, layerIndex);
				Layers.Add(musicSectionlayer);
			}
		}

		public void RunFixes(MusicSliceFixer info)
		{
			info.TryReplacing(Offset + 0x08, info.EntrySample);
			info.TryReplacing(Offset + 0x0c, info.ExitSample);
			info.TryReplacing(Offset + 0x10, info.LoopStartSample);
			info.TryReplacing(Offset + 0x14, info.LoopEndSample);

			foreach (var layer in Layers)
				layer.RunFixes(info);
		}
	}
}