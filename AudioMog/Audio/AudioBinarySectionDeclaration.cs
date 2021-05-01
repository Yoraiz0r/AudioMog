using System.IO;

namespace AudioMog.Core.Audio
{
	public class AudioBinarySectionDeclaration
	{
		public uint Magic;
		public ushort UnknownAt4;
		public ushort UnknownAt6;
		public uint OffsetInInnerFile;
		public uint UnknownAtC;
		public int SectionDeclarationSize;

		public AudioBinarySectionDeclaration(BinaryReader reader)
		{
			var declarationPosition = reader.BaseStream.Position;
			Magic = reader.ReadUInt32At(declarationPosition);
			UnknownAt4 = reader.ReadUInt16At(declarationPosition + 0x04);
			UnknownAt6 = reader.ReadUInt16At(declarationPosition + 0x06);
			OffsetInInnerFile = reader.ReadUInt32At(declarationPosition + 0x08);
			UnknownAtC = reader.ReadUInt32At(declarationPosition + 0x0c);
			SectionDeclarationSize = 16;
		}
	}
}