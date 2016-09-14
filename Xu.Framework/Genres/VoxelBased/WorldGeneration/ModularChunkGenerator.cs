using System.Collections.Generic;
using System.Linq;
using Xu.Types;

namespace Xu.Genres.VoxelBased.WorldGeneration
{
	public class ModularChunkGenerator : IChunkGenerator
	{
		private readonly List<IVoxelProcessor> _modules = new List<IVoxelProcessor>();
		private IVoxelClassifier _classifier;

		#region IChunkGenerator Members

		public void GenerateBlocks(IntVector3 chunkPosition, Block[] destination)
		{
			var voxelData = new VoxelData[destination.Length];

			foreach (var module in _modules.Where(module => module.Enabled).ToList())
			{
				module.Process(chunkPosition, voxelData);
			}

			_classifier.Classify(chunkPosition, voxelData, destination);
		}

		#endregion

		public void AddProcessor(IVoxelProcessor module)
		{
			_modules.Add(module);
		}

		public void SetClassifier(IVoxelClassifier classifier)
		{
			_classifier = classifier;
		}
	}
}
