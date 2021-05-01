using AudioMog.Core.Audio;
using AudioMog.Core.Exceptions;
using AudioMog.Core.Music;
using AudioMog.Core.Sound;

namespace AudioMog.Core
{
	public class FileParser
	{
		public FileParserSettings Settings { set; private get; }
		public AAudioBinaryFile Parse(byte[] fileBytes)
		{
			var mabfString = new byte[] {109, 97, 98, 102, Settings.MusicFileFileVersion.Main, Settings.MusicFileFileVersion.Sub};
			var mabfStringPosition = FindSubArray(fileBytes, mabfString);
			if (mabfStringPosition >= 0)
			{
				var file = new MusicAudioBinaryFile();
				file.Read(fileBytes, mabfStringPosition);
				return file;
			}
			
			var sabfString = new byte[] {115, 97, 98, 102, Settings.SoundFileFileVersion.Main, Settings.SoundFileFileVersion.Sub};
			var sabfStringPosition = FindSubArray(fileBytes, sabfString);
			if (sabfStringPosition >= 0)
			{
				var file = new SoundAudioBinaryFile();
				file.Read(fileBytes, sabfStringPosition);
				return file;
			}

			throw new FileDoesNotContainAudioBinaryException();
		}

		private long FindSubArray(byte[] array, byte[] subArray)
		{
			int maxAttempts = array.Length - subArray.Length;
			for (int i = 0; i < maxAttempts; i++)
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

	}
}