using System;
using System.IO;

namespace AudioMog.Core
{
	public static class ExtensionMethods
	{
		public static void GoTo(this BinaryReader reader, long position)
		{
			reader.BaseStream.Position = position;
		}

		public static ushort ReadUInt16At(this BinaryReader reader, long position)
		{
			reader.GoTo(position);
			return reader.ReadUInt16();
		}
		public static uint ReadUInt32At(this BinaryReader reader, long position)
		{
			reader.GoTo(position);
			return reader.ReadUInt32();
		}	
		public static float ReadFloatAt(this BinaryReader reader, long position)
		{
			reader.GoTo(position);
			return reader.ReadSingle();
		}
		public static byte ReadByteAt(this BinaryReader reader, long position)
		{
			reader.GoTo(position);
			return reader.ReadByte();
		}
		public static byte[] ReadBytesAt(this BinaryReader reader, long position, int count)
		{
			reader.GoTo(position);
			return reader.ReadBytes(count);
		}   
		public static T[] SubArray<T>(this T[] array, long offset, long length)
		{
			T[] result = new T[length];
			Array.Copy(array, offset, result, 0, length);
			return result;
		}
		
		public static long AlignSizeToMatchInBlocksOf(this long value, long blockAlignSize)
		{
			var extraSize = value % blockAlignSize;
			if (extraSize == 0)
				return value;
			return (value + blockAlignSize - extraSize);
		}
	}
}