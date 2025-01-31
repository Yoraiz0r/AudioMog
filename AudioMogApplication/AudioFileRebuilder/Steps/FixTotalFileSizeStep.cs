using System;
using System.Linq;
using AudioMog.Core;
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
			
			var originalInnerSize = (uint)file.InnerFileBytes.Length;
			var changedInnerSize = fileBytes.Length - positionBeforeMagic - file.BytesAfterFile.Length;
			
			var wroteIntoAudioBundle = WriteUintIfMatch(fileBytes, (int)positionBeforeMagic + 0x0c, originalInnerSize, (uint)changedInnerSize);
			
			//2025
			var tightWrite1 = WriteUintIfMatch(fileBytes, (int)positionBeforeMagic - 4, originalInnerSize, (uint)changedInnerSize);
			var tightWrite2 = WriteUintIfMatch(fileBytes, (int)positionBeforeMagic - 8, originalInnerSize, (uint)changedInnerSize);
			if (tightWrite1 && tightWrite2)
			{
				var originalWeirdSize = BitConverter.GetBytes(originalInnerSize + 16).Concat(new byte[] { 0x00, 0x00, 0x01, 0x00, 0x10, 0x00 }).ToArray();
				var hasMagic = ExtensionMethods.FindMagicFromEnd(fileBytes, originalWeirdSize, out var offset);
				if (hasMagic)
					WriteUintIfMatch(fileBytes, (int)offset, originalInnerSize + 16, (uint)changedInnerSize + 16);
				return;
			}
			
			//2019
			if (positionBeforeMagic >= 16)
			{
				WriteUintIfMatch(fileBytes, (int)positionBeforeMagic - 16, originalInnerSize, (uint)changedInnerSize);
				WriteUintIfMatch(fileBytes, (int)positionBeforeMagic - 12, originalInnerSize, (uint)changedInnerSize);
			}
			if (positionBeforeMagic >= 44)
				WriteUintIfMatch(fileBytes, (int)positionBeforeMagic - 44, originalInnerSize, (uint)changedInnerSize);
		}
	}
}