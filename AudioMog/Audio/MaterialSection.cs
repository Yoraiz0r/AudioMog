using System.Collections.Generic;
using System.IO;

namespace AudioMog.Core.Audio
{
	public class MaterialSection
	{
		public static int Magic_Mtrl = 1819440237;
		
		public long MaterialSectionOffset;
		public List<MaterialEntry> Entries;
		public List<MaterialUser> Users;
		public ushort EntryAddressesSize;
		public uint InnerFilePositionOfFirstTracks;

		public MaterialSection(AAudioBinaryFile file, BinaryReader binaryReader)
		{
			var sectionDeclaration = file.GetSectionDeclaration(Magic_Mtrl);
			MaterialSectionOffset = file.InnerFileStartOffset + sectionDeclaration.OffsetInInnerFile;
			Users = new List<MaterialUser>();
			
			EntryAddressesSize = binaryReader.ReadUInt16At(MaterialSectionOffset + 0x02);
			
			var entryCount = binaryReader.ReadUInt16At(MaterialSectionOffset + 0x04);
			Entries = new List<MaterialEntry>();
			for (int songEntryIndex = 0; songEntryIndex < entryCount; songEntryIndex++)
			{
				var positionOfOffsetFromMaterialSectionOffset = MaterialSectionOffset + 0x10 + songEntryIndex * 0x04;
				var localEntryOffset = binaryReader.ReadUInt32At(positionOfOffsetFromMaterialSectionOffset);
				if (songEntryIndex == 0)
					InnerFilePositionOfFirstTracks = sectionDeclaration.OffsetInInnerFile + localEntryOffset;
				
				var entryOffset = MaterialSectionOffset + localEntryOffset;

				var codec = binaryReader.ReadByteAt(entryOffset + 0x05);
				if (codec == 0)
					continue;

				var entry = new MaterialEntry();
				entry.Read(binaryReader, songEntryIndex, MaterialSectionOffset, positionOfOffsetFromMaterialSectionOffset);
				Entries.Add(entry);
			}
		}

		public void AddUser(INamedEntry user, int materialIndex)
		{
			Users.Add(new MaterialUser()
			{
				User = user,
				MaterialIndex = materialIndex,
			});
		}
		
		public class MaterialUser
		{
			public INamedEntry User;
			public int MaterialIndex;
		}
		public class MaterialEntry
		{
			public int EntryIndex;
			public long StreamPosition;
			public long MaterialHeaderSize;
			public long HeaderPosition;
			public ushort MtrlNumber;
			public long ExtraDataOffset;
			public long PositionOfOffsetFromMtrlSectionOffset;
			public uint StreamSize;
			public ushort HcaHeaderSize;
			public long HcaStreamStartPosition;
			public uint HcaStreamSize;
			public long NoHcaHeaderSize;
			public long TrackEndPosition;
			public uint NoHcaHeaderExtraDataSize;
			public uint LocalSectionOffset;
			public uint LoopStart;
			public uint LoopEnd;
			public uint ExtraDataSize;
			public uint SampleRate;
			public byte ChannelCount;
			public byte Codec;
			public ushort ExtraDataId;
			public bool IsLooping => LoopEnd > 0;

			public void Read(BinaryReader binaryReader, int entryIndex, long mtrlSectionOffset, long pointerPosition)
			{
				EntryIndex = entryIndex;
				PositionOfOffsetFromMtrlSectionOffset = pointerPosition;
				LocalSectionOffset = binaryReader.ReadUInt32At(PositionOfOffsetFromMtrlSectionOffset);
				HeaderPosition = mtrlSectionOffset + LocalSectionOffset;
				ChannelCount = binaryReader.ReadByteAt(HeaderPosition + 0x04);
				Codec = binaryReader.ReadByteAt(HeaderPosition + 0x05);
				MtrlNumber = binaryReader.ReadUInt16At(HeaderPosition + 0x06);
				SampleRate = binaryReader.ReadUInt32At(HeaderPosition + 0x08);
				LoopStart = binaryReader.ReadUInt32At(HeaderPosition + 0x0c);
				LoopEnd = binaryReader.ReadUInt32At(HeaderPosition + 0x10);
				ExtraDataSize = binaryReader.ReadUInt32At(HeaderPosition + 0x14);
				StreamSize = binaryReader.ReadUInt32At(HeaderPosition + 0x18);
				ExtraDataId = binaryReader.ReadUInt16At(HeaderPosition + 0x1c);
				ExtraDataOffset = HeaderPosition + 0x20;
				StreamPosition = ExtraDataOffset + ExtraDataSize;
				MaterialHeaderSize = StreamPosition - HeaderPosition;
				
				//why is this in big endian wtf?
				var hcaHeaderSizeByteBig = binaryReader.ReadByteAt(ExtraDataOffset + 0x10 + 0x06);
				var hcaHeaderSizeByteSmall = binaryReader.ReadByteAt(ExtraDataOffset + 0x10 + 0x07);
				HcaHeaderSize = (ushort)((hcaHeaderSizeByteBig << 8) + hcaHeaderSizeByteSmall);
				
				NoHcaHeaderExtraDataSize = ExtraDataSize - HcaHeaderSize;
				HcaStreamStartPosition = ExtraDataOffset + 0x10;
				HcaStreamSize = HcaHeaderSize + StreamSize;
				NoHcaHeaderSize = HcaStreamStartPosition - HeaderPosition;
				TrackEndPosition = HcaStreamStartPosition + HcaStreamSize;
			}
		}
	}
}