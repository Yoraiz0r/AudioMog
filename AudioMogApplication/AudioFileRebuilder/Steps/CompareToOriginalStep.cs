using System;
using System.Diagnostics;

namespace AudioMog.Application.AudioFileRebuilder.Steps
{
	public class CompareToOriginalStep : ARebuilderStep
	{
		private readonly byte[] _originalBackup;
		private readonly int _comparisonIndex;
		private readonly int _changeReportLimit;
		public CompareToOriginalStep(byte[] originalBackup, int comparisonIndex, int changeReportLimit)
		{
			_originalBackup = originalBackup;
			_comparisonIndex = comparisonIndex;
			_changeReportLimit = changeReportLimit;
		}
		public override void Run(Blackboard blackboard)
		{
			var fileBytes = blackboard.FileBytes;
			
			if (fileBytes.Length != _originalBackup.Length)
				Debug.WriteLine($"There's a size diff, original: {_originalBackup.Length}, new: {fileBytes.Length}");

			var sizeWeCanCheck = Math.Min(_originalBackup.Length, fileBytes.Length);
			var changesShown = 0;
			for (var i = 0; i < sizeWeCanCheck; i++)
			{
				if (_originalBackup[i] == fileBytes[i])
					continue;
				
				if (changesShown == 0)
					Debug.WriteLine($"Comparison {_comparisonIndex}!");
					
				Debug.WriteLine($"index: {i},  {_originalBackup[i]} : {fileBytes[i]}");
					
				changesShown++;
				if (changesShown == _changeReportLimit)
				{
					Debug.WriteLine("Reached difference limit!");
					break;
				}
			}

			if (changesShown > 0)
				Debug.WriteLine("");
		}
	}
}