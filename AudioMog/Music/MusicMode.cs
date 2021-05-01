using System.IO;
using System.Text;

namespace AudioMog.Core.Music
{
	public class MusicMode
	{
		public long Offset;
		public byte Version;
		public ushort Size;
		
		public long NameOffset;
		public byte NameSize;
		public string Name;

		public MusicMode(MusicEntry entry, BinaryReader binaryReader, int index)
		{
			Offset = entry.Offset + binaryReader.ReadUInt32At(entry.TableOffset + entry.SliceCount * 0x04 + index * 0x04);
			Version = binaryReader.ReadByteAt(Offset);
			Size = binaryReader.ReadUInt16At(Offset + 0x02);
			NameSize = binaryReader.ReadByteAt(Offset + 0x06);

			if (Version <= 2)
			{
				NameOffset = Offset + 0x20;
				NameSize = 0x0f;
			}
			else
			{
				NameOffset = Offset + Size;
			}

			Name = Encoding.ASCII.GetString(binaryReader.ReadBytesAt(NameOffset, NameSize));
		}
		
	}
}