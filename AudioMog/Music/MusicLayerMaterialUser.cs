using AudioMog.Core.Audio;

namespace AudioMog.Core.Music
{
	public class MusicLayerMaterialUser : INamedEntry
	{
		private readonly MusicSlice _slice;
		private readonly MusicLayer _layer;
		public string DisplayName => $"{_slice.Name} (layer {_layer.Index})";

		public MusicLayerMaterialUser(MusicSlice slice, MusicLayer layer)
		{
			_slice = slice;
			_layer = layer;
		}
	}
}