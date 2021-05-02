using System;
using System.Collections.Generic;
using System.IO;
using AudioMog.Core.Exceptions;

namespace AudioMog.Core.Audio
{
	public abstract class AAudioBinaryFile
	{
		public long InnerFileStartOffset;
		public AudioBinaryFileHeader Header;
		public byte[] InnerFileBytes;
		public byte[] BytesAfterFile;
		public byte[] BytesBeforeFile;
		public long SectionDeclarationsOffset;

		public List<AudioBinarySectionDeclaration> SectionDeclarations = new List<AudioBinarySectionDeclaration>();

		public MaterialSection MaterialSection;
		
		public void Read(Byte[] fileBytes, long offsetForFileStart)
		{
			InnerFileStartOffset = offsetForFileStart;

			var stream = new MemoryStream(fileBytes);
			var reader = new BinaryReader(stream);
			
			Header = new AudioBinaryFileHeader(reader, offsetForFileStart);
			if (Header.IsBigEndian)
				throw new FileIsUnsupportedBigEndianAudioBinaryException();

			ReadSectionDeclarations(reader);
			
			MaterialSection = new MaterialSection(this, reader);
			
			ParseSections(reader);

			CacheFileBytes(fileBytes, offsetForFileStart);
		}

		protected abstract void ParseSections(BinaryReader reader);

		private void ReadSectionDeclarations(BinaryReader reader)
		{
			var position = InnerFileStartOffset + Header.HeaderSize;
			for (int sectionIndex = 0; sectionIndex < Header.SectionsCount; sectionIndex++)
			{
				reader.GoTo(position);

				var section = new AudioBinarySectionDeclaration(reader);
				SectionDeclarations.Add(section);

				position += section.SectionDeclarationSize;
			}
		}

		private void CacheFileBytes(byte[] fileBytes, long offsetForFileStart)
		{
			var endOfInnerFile = offsetForFileStart + Header.FileSize;
			BytesBeforeFile = fileBytes.SubArray(0, offsetForFileStart);
			BytesAfterFile = fileBytes.SubArray(endOfInnerFile, fileBytes.Length - endOfInnerFile);
			InnerFileBytes = fileBytes.SubArray(offsetForFileStart, Header.FileSize);
		}

		public AudioBinarySectionDeclaration GetSectionDeclaration(int magic)
		{
			foreach (var declaration in SectionDeclarations)
				if (declaration.Magic == magic)
					return declaration;
			
			throw new FileDoesNotHaveSectionDeclarationException($"Could not find section magic code: {magic}!");
		}

		public long AlignToBlockStart(long position)
		{
			var ret = position - (position - InnerFileStartOffset) % 16;
			return ret;
		}
	}
}