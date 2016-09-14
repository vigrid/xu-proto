using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Xu.Types;

namespace Xu.Genres.VoxelBased
{
	public interface IChunkManager
	{
		IEnumerable<Chunk> RenderCacheableChunks { get; }
		IEnumerable<Chunk> ChunksToRender { get; }

		bool HasPickedBlock { get; }
		IntVector3 DestructibleCoords { get; }
		IntVector3 PlaceableCoord { get; }

		Ray? PickingRay { get; set; }
	}
}
