using System.Collections.Generic;
using System.IO;
using AudioMog.Core.Audio;

namespace AudioMog.Core.Sound
{
	public class SoundAudioBinaryFile : AAudioBinaryFile
	{
		public static int Magic_Snd = 543452787; //hex ASCII for 'snd' section of the file
		public static int Magic_Seq = 544302451; //hex ASCII for 'seq' section of the file
		public static int Magic_Trk = 543912564; //hex ASCII for 'trk' section of the file
		
		public long SoundSectionOffset;
		public long SequenceSectionOffset;
		public long TrackSectionOffset;

		public List<SoundEntry> SoundEntries = new List<SoundEntry>();
		public List<TrackEntry> Tracks = new List<TrackEntry>();

		protected override void ParseSections(BinaryReader reader)
		{
			SoundSectionOffset = InnerFileStartOffset + GetSectionDeclaration(Magic_Snd).OffsetInInnerFile;
			SequenceSectionOffset = InnerFileStartOffset + GetSectionDeclaration(Magic_Seq).OffsetInInnerFile;
			TrackSectionOffset = InnerFileStartOffset + GetSectionDeclaration(Magic_Trk).OffsetInInnerFile;

			var entryCount = reader.ReadUInt16At(SoundSectionOffset + 0x04);
			for (int entryIndex = 0; entryIndex < entryCount; entryIndex++)
			{
				var entry = new SoundEntry(this, reader, entryIndex);
				SoundEntries.Add(entry);
			}

			var trackCount = reader.ReadUInt16At(TrackSectionOffset + 0x04);
			for (int trackIndex = 0; trackIndex < trackCount; trackIndex++)
			{
				var track = new TrackEntry(this, reader, trackIndex);
				Tracks.Add(track);
			}

			AddMaterialUsers();
		}

		private void AddMaterialUsers()
		{
			foreach (var entry in SoundEntries)
				foreach (var sequence in entry.Sequences)
					foreach (var command in sequence.Commands)
						AddMaterialUser(command, entry);
		}

		private void AddMaterialUser(SequenceCommand command, SoundEntry entry)
		{
			var trackIndex = command.Body?.TrackIndex ?? null;
			if (trackIndex == null)
				return;

			if (trackIndex < 0 || trackIndex >= Tracks.Count)
				return;

			var track = Tracks[(int) trackIndex.Value];
			if (track.HasMaterial)
				MaterialSection.AddUser(entry, track.MaterialIndex);
		}
	}
}