namespace AudioMog.Core.Sound
{
	public enum SequenceCommandType : byte
	{
		None = 0, 
		End = 1, 
		KeyOn = 2,
		Interval = 3,
		KeyOff = 4,
		Wat = 5,
		LoopStart = 6,
		LoopEnd = 7,
		Trigger = 8,
		Count = 9
	}
}