using System;
using System.Diagnostics;

namespace AudioMog.Application.AudioFileRebuilder.Steps
{
	public class CompareToOriginalStep : ARebuilderStep
	{
		private readonly byte[] _originalBackup;
		private readonly int _comparisonIndex;
		public CompareToOriginalStep(byte[] originalBackup, int comparisonIndex)
		{
			_originalBackup = originalBackup;
			_comparisonIndex = comparisonIndex;
		}
		public override void Run(Blackboard blackboard)
		{
			var fileBytes = blackboard.FileBytes;
				
			if (fileBytes.Length != _originalBackup.Length)
				Debug.WriteLine($"wtf why is this size different?! original: {_originalBackup.Length}, new: {fileBytes.Length}");

			int sizeWeCanCheck = Math.Min(_originalBackup.Length, fileBytes.Length);
			for (int i = 0; i < sizeWeCanCheck; i++)
				if (_originalBackup[i] != fileBytes[i])
					Debug.WriteLine($"index: {i},  {_originalBackup[i]} : {fileBytes[i]}");
		}
	}
}