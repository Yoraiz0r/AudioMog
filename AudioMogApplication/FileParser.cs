using AudioMog.Core;
using AudioMog.Core.Audio;
using AudioMog.Core.Exceptions;
using AudioMog.Core.Music;
using AudioMog.Core.Sound;

namespace AudioMog.Application
{
	public class FileParser
	{
		public static readonly byte[] SabfString = {115, 97, 98, 102 };
		public static readonly byte[] MabfString = {109, 97, 98, 102 };
		public FileParserSettings Settings { set; private get; }
		public IApplicationLogger Logger { set; protected get; }

		public AAudioBinaryFile Parse(byte[] fileBytes)
		{

			var hasSabf = TryGetInternalFilePosition<SoundAudioBinaryFile>(fileBytes, SabfString, out var sabfStart);
			var hasMabf = TryGetInternalFilePosition<MusicAudioBinaryFile>(fileBytes, MabfString, out var mabfStart);

			var sabfFirst = false;
			var mabfFirst = false;

			if (hasSabf && hasMabf)
			{
				sabfFirst = sabfStart < mabfStart;
				mabfFirst = !sabfFirst;
			}
			else
			{
				sabfFirst = hasSabf;
				mabfFirst = hasMabf;
			}

			if (mabfFirst && TryReading(fileBytes, MabfString, Settings.MusicFileFileVersion, out MusicAudioBinaryFile mab))
				return mab;

			if (sabfFirst && TryReading(fileBytes, SabfString, Settings.SoundFileFileVersion, out SoundAudioBinaryFile sab))
				return sab;

			if (TryReading(fileBytes, MabfString, Settings.MusicFileFileVersion, out mab))
				return mab;

			if (TryReading(fileBytes, SabfString, Settings.SoundFileFileVersion, out sab))
				return sab;
			
			throw new FileDoesNotContainAudioBinaryException();
		}

		private bool TryReading<TAudioBinaryFileType>(byte[] fileBytes, byte[] arrayOfBytesMagic, AudioBinaryFileVersion expectedVersion, out TAudioBinaryFileType file) where TAudioBinaryFileType : AAudioBinaryFile, new()
		{
			file = null;
			
			if (!TryGetInternalFilePosition<TAudioBinaryFileType>(fileBytes, arrayOfBytesMagic, out var internalFileStartPosition))
				return false;

			if (!FindExpectedVersion(fileBytes, internalFileStartPosition + arrayOfBytesMagic.Length,
					expectedVersion, out var failureReason))
				Logger.Warn(failureReason);
			
			file = new TAudioBinaryFileType();
			file.Read(fileBytes, internalFileStartPosition);
			return true;

		}

		private static bool TryGetInternalFilePosition<TAudioBinaryFileType>(byte[] fileBytes, byte[] arrayOfBytesMagic,
			out long internalFileStartPosition) where TAudioBinaryFileType : AAudioBinaryFile, new()
		{
			internalFileStartPosition = fileBytes.FindSubArray(arrayOfBytesMagic);
			if (internalFileStartPosition < 0)
				return false;
			return true;
		}

		private bool FindExpectedVersion(byte[] fileBytes, long position, AudioBinaryFileVersion version, out string failureReason)
		{
			failureReason = null;
			
			if (fileBytes[position] != version.Main || fileBytes[position + 1] != version.Sub)
			{
				failureReason = $"Unexpected Audio Binary File Version found! {fileBytes[position]}.{fileBytes[position+1]} instead of the expected {version.Main}.{version.Sub}!";
				return false;
			}
			
			return true;
		}
	}
}