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
			if (TryReading(fileBytes, MabfString, Settings.MusicFileFileVersion, out MusicAudioBinaryFile mabf))
				return mabf;

			if (TryReading(fileBytes, SabfString, Settings.SoundFileFileVersion, out SoundAudioBinaryFile sabf))
				return sabf;
			
			throw new FileDoesNotContainAudioBinaryException();
		}

		private bool TryReading<TAudioBinaryFileType>(byte[] fileBytes, byte[] arrayOfBytesMagic, AudioBinaryFileVersion expectedVersion, out TAudioBinaryFileType file) where TAudioBinaryFileType : AAudioBinaryFile, new()
		{
			file = null;
			
			var internalFileStartPosition = fileBytes.FindSubArray(arrayOfBytesMagic);
			if (internalFileStartPosition < 0)
				return false;

			if (!FindExpectedVersion(fileBytes, internalFileStartPosition + arrayOfBytesMagic.Length,
				expectedVersion, out var failureReason))
				Logger.Warn(failureReason);
			
			file = new TAudioBinaryFileType();
			file.Read(fileBytes, internalFileStartPosition);
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