using AudioMog.Core.Audio;

namespace AudioMog.Application.AudioFileRebuilder.Steps
{
	public class FixTotalFileSizeStep : ARebuilderStep
	{
		public override void Run(Blackboard blackboard)
		{
			FixTotalFileSize(blackboard.File, blackboard.FileBytes);
		}
		private void FixTotalFileSize(AAudioBinaryFile file, byte[] fileBytes)
		{
			var positionBeforeMagic = file.InnerFileStartOffset;
			
			var expectedSize = fileBytes.Length - positionBeforeMagic - file.BytesAfterFile.Length;
			
			WriteUint(fileBytes, (int)positionBeforeMagic + 0x0c, (uint)expectedSize);
			
			if (positionBeforeMagic >= 16)
			{
				WriteUint(fileBytes, (int)positionBeforeMagic - 16, (uint)expectedSize);
				WriteUint(fileBytes, (int)positionBeforeMagic - 12, (uint)expectedSize);
			}
			
			if (positionBeforeMagic >= 44)
				WriteUint(fileBytes, (int)positionBeforeMagic - 44, (uint)expectedSize);
		}
	}
}