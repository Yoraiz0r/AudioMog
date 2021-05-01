using System.Collections.Generic;
using System.IO;
using AudioMog.Core.Audio;

namespace AudioMog.Core.Music
{
	public class MusicAudioBinaryFile : AAudioBinaryFile
	{
		public static int Magic_Musc = 1668511085;  //hex ASCII for 'musc' section of the file
		public static int Magic_Inst = 1953721961;  //hex ASCII for 'inst' section of the file
		
		public long MusicSectionOffset;
		public long InstrumentSectionOffset;
		
		public List<MusicEntry> Entries = new List<MusicEntry>();
		public List<MusicInstrument> Instruments = new List<MusicInstrument>();

		protected override void ParseSections(BinaryReader reader)
		{
			MusicSectionOffset = InnerFileStartOffset + GetSectionDeclaration(Magic_Musc).OffsetInInnerFile;
			InstrumentSectionOffset = InnerFileStartOffset + GetSectionDeclaration(Magic_Inst).OffsetInInnerFile;
			
			
			var trackEntryCount = reader.ReadUInt16At(MusicSectionOffset + 0x04);
			for (int entryIndex = 0; entryIndex < trackEntryCount; entryIndex++)
			{
				var entry = new MusicEntry(this, reader, entryIndex);
				Entries.Add(entry);
			}

			var instrumentEntryCount = reader.ReadUInt16At(InstrumentSectionOffset + 0x04);
			for (int instrumentIndex = 0; instrumentIndex < instrumentEntryCount; instrumentIndex++)
			{
				var instrument = new MusicInstrument(this, reader, instrumentIndex);
				Instruments.Add(instrument);
			}

			AddMaterialUsers();
		}

		private void AddMaterialUsers()
		{
			foreach (var entry in Entries)
				foreach (var slice in entry.Slices)
					foreach (var layer in slice.Layers)
						MaterialSection.AddUser(new MusicLayerMaterialUser(slice, layer), layer.MaterialIndex);
			
			foreach (var instrument in Instruments)
				foreach (var material in instrument.Materials)
					MaterialSection.AddUser(instrument, material.MaterialIndex);
		}
	}
}