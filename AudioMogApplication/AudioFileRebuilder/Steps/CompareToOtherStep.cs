using System;
using System.Diagnostics;
using System.IO;

namespace AudioMog.Application.AudioFileRebuilder.Steps
{
	public class CompareToOtherStep : ARebuilderStep
	{
		private readonly byte[] _originalBackup;
		public CompareToOtherStep(string otherFilePath)
		{
			var otherBytes = File.ReadAllBytes(otherFilePath);
			_originalBackup = otherBytes;
		}
		public override void Run(Blackboard blackboard)
		{
			var fileBytes = blackboard.FileBytes;
				
			if (fileBytes.Length != _originalBackup.Length)
				Debug.WriteLine($"other: wtf why is this size different?! original: {_originalBackup.Length}, new: {fileBytes.Length}");

			int sizeWeCanCheck = Math.Min(_originalBackup.Length, fileBytes.Length);
			for (int i = 0; i < sizeWeCanCheck; i++)
				if (_originalBackup[i] != fileBytes[i])
					Debug.WriteLine($"other index: {i},  {_originalBackup[i]} : {fileBytes[i]}");
		}
	}
}