using System;
using System.Collections.Generic;
using System.IO;
using AudioMog.Core.Audio.ExtraData;

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
			public ushort StreamHeaderSize;
			public long InnerStreamStartPosition;
			public uint InnerStreamSize;
			public long NoStreamHeaderSize;
			public long TrackEndPosition;
			public uint NoStreamHeaderExtraDataSize;
			public uint LocalSectionOffset;
			public uint LoopStart;
			public uint LoopEnd;
			public uint ExtraDataSize;
			public uint SampleRate;
			public byte ChannelCount;
			public MaterialCodecType Codec;
			public ushort ExtraDataId;
			public bool IsLooping => LoopEnd > 0;

			public ACodecExtraData ExtraDataObject;

			public void Read(BinaryReader binaryReader, int entryIndex, long mtrlSectionOffset, long pointerPosition)
			{
				EntryIndex = entryIndex;
				PositionOfOffsetFromMtrlSectionOffset = pointerPosition;
				LocalSectionOffset = binaryReader.ReadUInt32At(PositionOfOffsetFromMtrlSectionOffset);
				 HeaderPosition = mtrlSectionOffset + LocalSectionOffset;
				ChannelCount = binaryReader.ReadByteAt(HeaderPosition + 0x04);
				Codec = (MaterialCodecType)binaryReader.ReadByteAt(HeaderPosition + 0x05);
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


				InnerStreamStartPosition = ExtraDataOffset + 0x10;
				ReadCodecInformation(binaryReader);
				StreamHeaderSize = ExtraDataObject?.Size ?? 0;
				NoStreamHeaderExtraDataSize = ExtraDataSize - StreamHeaderSize;

				InnerStreamSize = StreamHeaderSize + StreamSize;
				NoStreamHeaderSize = InnerStreamStartPosition - HeaderPosition;
				TrackEndPosition = InnerStreamStartPosition + InnerStreamSize;
			}

			private void ReadCodecInformation(BinaryReader binaryReader)
			{
				switch (Codec)
				{
					case MaterialCodecType.OGGVorbis:
						InnerStreamStartPosition = StreamPosition;
						ExtraDataObject = new OggVorbisExtraData(this, binaryReader);
						break;
					
					case MaterialCodecType.HCA:
						ExtraDataObject = new HcaExtraData(this, binaryReader);
						break;
				}
			}
		}
	}

	public enum MaterialCodecType : byte
	{
		Dummy = 0,
		PCM = 1, //chrono trigger
		MSADPCM = 2, //dragon quest builders
		OGGVorbis = 3, //ffxv benchmark sfx pc
		ATRAC9 = 4, //dragon quest builders vita, ffxv ps4
		XMA2 = 5,
		MSMP3 = 6, //dragon quest builders ps3
		HCA = 7,
		SWITCHOPUS = 8, //no extra data?
	}
}