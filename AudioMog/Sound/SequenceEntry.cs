using System.Collections.Generic;
using System.IO;

namespace AudioMog.Core.Sound
{
	public class SequenceEntry
	{
		public byte DeclarationVersion;
		public ushort DeclarationSize;
		public ushort DeclarationIndex;
		public long Offset;
		public byte Version;

		public List<SequenceCommand> Commands = new List<SequenceCommand>();

		public SequenceEntry(SoundAudioBinaryFile file, BinaryReader binaryReader, SoundEntry soundEntry, long offset)
		{
			DeclarationVersion = binaryReader.ReadByteAt(offset);
			//1 reserved

			DeclarationSize = binaryReader.ReadUInt16At(offset + 0x02);
			DeclarationIndex = binaryReader.ReadUInt16At(offset + 0x04);

			Offset = file.SequenceSectionOffset 
			            + binaryReader.ReadUInt32At(file.SequenceSectionOffset + 0x10 + DeclarationIndex * 0x04);

			Version = binaryReader.ReadByteAt(Offset);
			//1 reserved
			//2 size?

			ReadCommands(file, binaryReader, soundEntry);
		}

		private void ReadCommands(SoundAudioBinaryFile file, BinaryReader binaryReader, SoundEntry soundEntry)
		{
			uint commandsOffsetInEntry;
			if (Version <= 2)
			{
				commandsOffsetInEntry = binaryReader.ReadUInt16At(Offset + 0x16);
			}
			else
			{
				commandsOffsetInEntry = binaryReader.ReadUInt16At(Offset + 0x06);
			}

			var currentCommandsOffset = Offset + commandsOffsetInEntry;
			while (currentCommandsOffset < file.TrackSectionOffset)
			{
				var command = new SequenceCommand(binaryReader, currentCommandsOffset);
				Commands.Add(command);

				currentCommandsOffset += (uint) (command.Size + command.BodySize);

				if (ShouldStopReadingCommands(soundEntry, command))
					break;
			}
		}

		private static bool ShouldStopReadingCommands(SoundEntry sndEntry, SequenceCommand command)
		{
			if (command.Type <= 0 || command.Type >= (byte) SequenceCommandType.Count)
				return true;

			//named entries typically only have 1 command
			if (sndEntry.NameOffset > 0)
				return true;
			
			return false;
		}
	}
}