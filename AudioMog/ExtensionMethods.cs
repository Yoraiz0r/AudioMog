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
		
		public static long FindSubArray(this byte[] array, byte[] subArray)
		{
			int maxAttempts = array.Length - subArray.Length;
			for (int i = 0; i < maxAttempts; i++)
				if (CompareSubArray(array, i, subArray))
					return i;

			return -1;
		}

		public static long FindSubArrayInReverse(this byte[] array, byte[] subArray)
		{
			int maxAttempts = array.Length - subArray.Length;
			for (int i = maxAttempts - 1; i >= 0; i--)
				if (CompareSubArray(array, i, subArray))
					return i;

			return -1;
		}
		private static bool CompareSubArray (byte[] array, int position, byte[] candidate)
		{
			if (candidate.Length > (array.Length - position))
				return false;

			for (int i = 0; i < candidate.Length; i++)
				if (array[position + i] != candidate[i])
					return false;

			return true;
		}
		public static string ChangeExtension(string filePath, string newExtension)
		{
			var extensionIndex = filePath.LastIndexOf('.');
			if (extensionIndex != -1)
				filePath = filePath.Substring(0, extensionIndex);
			return filePath + newExtension;
		}

		public static string AddExtension(string filePath, string newExtension)
		{
			return filePath + newExtension;
		}
		
		public static bool FindMagicFromEnd(byte[] array, byte[] magic, out uint magicStartPosition)
		{
			magicStartPosition = 0;
			for (var indexOnBig = array.Length - magic.Length; indexOnBig >= 0; indexOnBig--)
			{
				var match = true;
				for (var indexInMagic = 0; indexInMagic < magic.Length; indexInMagic++)
					if (array[indexOnBig + indexInMagic] != magic[indexInMagic])
					{
						match = false;
						break;
					}

				if (match)
				{
					magicStartPosition = (uint)indexOnBig;
					return true;
				}
			}
			return false;
		}
	}
}