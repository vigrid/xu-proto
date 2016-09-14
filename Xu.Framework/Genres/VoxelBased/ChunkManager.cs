using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Xu.Async;
using Xu.Graphics.Cameras;
using Xu.Types;

namespace Xu.Genres.VoxelBased
{
	public class ChunkManager : GameComponent, IChunkManager
	{
		#region Delegates

		public delegate void BlockRemovedEvent(object sender, BlockRemovedEventArgs args);

		#endregion

		private const int CacheDistance = 5;
		private const int RenderCacheDistance = 5;
		private const int RenderDistance = 5;

		private static readonly IntVector3[] NeighborhoodDeltas = new[]
		{
			IntVector3.Left, IntVector3.Right, IntVector3.Down, IntVector3.Up, IntVector3.Forward, IntVector3.Backward
		};

		private readonly ICamera _camera;

		private readonly IChunkGenerator _chunkGenerator;
		private readonly List<IntVector3> _chunkKeysToRemove = new List<IntVector3>();
		private readonly IChunkStore _chunkStore;

		private readonly Dictionary<IntVector3, Chunk> _chunks = new Dictionary<IntVector3, Chunk>();
		private readonly List<Chunk> _chunksToRender = new List<Chunk>();
		private readonly Queue<Chunk> _syncQueue = new Queue<Chunk>();
		private readonly ITaskManager _taskManager;
		private IntVector3 _destructibleCoords;

		private IntVector3 _focusChunkPosition;
		private IntVector3 _placeableCoord;

		public ChunkManager(Game game, IChunkGenerator chunkGenerator, IChunkStore chunkStore, ITaskManager taskManager, ICamera camera) : base(game)
		{
			_chunkGenerator = chunkGenerator;
			_chunkStore = chunkStore;
			_taskManager = taskManager;

			_camera = camera;

			ActiveBlockType = BlockType.Solid1;
		}

		public IEnumerable<Chunk> NewChunks
		{
			get { return _chunks.Values.Where(chunk => chunk.State == ChunkState.New); }
		}

		public IEnumerable<Chunk> RenderableChunks
		{
			get { return _chunks.Values.Where(chunk => (chunk.State == ChunkState.DataInSync || chunk.State == ChunkState.DataOutOfSync) && IsChunkInRange(chunk.Position, RenderDistance)); }
		}

		public bool ShouldPlaceBlock { get; set; }
		public bool ShouldRemoveBlock { get; set; }

		#region IChunkManager Members

		public Ray? PickingRay { get; set; }

		public IEnumerable<Chunk> RenderCacheableChunks
		{
			get { return _chunks.Values.Where(chunk => (chunk.State == ChunkState.DataInSync || chunk.State == ChunkState.DataOutOfSync) && IsChunkInRange(chunk.Position, RenderCacheDistance)); }
		}

		public IEnumerable<Chunk> ChunksToRender
		{
			get { return _chunksToRender; }
		}

		public bool HasPickedBlock { get; private set; }

		public IntVector3 DestructibleCoords
		{
			get { return _destructibleCoords; }
		}

		public IntVector3 PlaceableCoord
		{
			get { return _placeableCoord; }
		}

		#endregion

		private void SetFocusChunk(IntVector3 focusPosition)
		{
			if (focusPosition != _focusChunkPosition)
			{
				RemoveChunksOutOfCachingRange(focusPosition);
				DisposeOfChunksGraphicBuffers();
				ReallocateChunks(focusPosition);

				_focusChunkPosition = focusPosition;
			}
		}

		public override void Initialize()
		{
			Chunk.TranslateWorldToChunkCoord((IntVector3) _camera.Position, out _focusChunkPosition);

			IntVector3 position;
			for (position.X = _focusChunkPosition.X - CacheDistance; position.X <= _focusChunkPosition.X + CacheDistance; position.X++)
			{
				for (position.Y = _focusChunkPosition.Y - CacheDistance; position.Y <= _focusChunkPosition.Y + CacheDistance; position.Y++)
				{
					for (position.Z = _focusChunkPosition.Z - CacheDistance; position.Z <= _focusChunkPosition.Z + CacheDistance; position.Z++)
					{
						if (IsChunkInRange(position, CacheDistance))
						{
							AddChunk(new Chunk(position));
						}
					}
				}
			}

			base.Initialize();
		}

		private void AddChunk(Chunk chunk)
		{
			_chunks.Add(chunk.Position, chunk);
		}

		public override void Update(GameTime gameTime)
		{
			IntVector3 chunkCoord, blockCoord;
			Chunk.TranslateWorldToChunkBlockCoord((IntVector3) _camera.Position, out chunkCoord, out blockCoord);

			SaveDirtyChunks();
			SetFocusChunk(chunkCoord);
			AttachReadyChunks();
			EnqueueNewChunksForLoading();
			SortChunksForRenderer();
			PerformPicking(20.0f);
			PerformPlacementAndDestruction();

			ShouldPlaceBlock = false;
			ShouldRemoveBlock = false;

			base.Update(gameTime);
		}

		public BlockType ActiveBlockType { get; set; }

		private void PerformPlacementAndDestruction()
		{
			IntVector3 blockCoord;
			IntVector3 chunkCoord;
			if (HasPickedBlock && ShouldPlaceBlock)
			{
				Chunk.TranslateWorldToChunkBlockCoord(PlaceableCoord, out chunkCoord, out blockCoord);
				Chunk chunk;
				if (_chunks.TryGetValue(chunkCoord, out chunk))
				{
					chunk.SetBlock(blockCoord.X, blockCoord.Y, blockCoord.Z, ActiveBlockType);
				}
			}
			if (HasPickedBlock && ShouldRemoveBlock)
			{
				Chunk.TranslateWorldToChunkBlockCoord(DestructibleCoords, out chunkCoord, out blockCoord);
				Chunk chunk;
				if (_chunks.TryGetValue(chunkCoord, out chunk))
				{
					Block block;
					chunk.TryGetBlock(blockCoord, out block);
					chunk.SetBlock(blockCoord.X, blockCoord.Y, blockCoord.Z, BlockType.Empty);
					OnBlockRemoved(block, blockCoord);
				}
			}
		}

		private void OnBlockRemoved(Block block, IntVector3 blockCoord)
		{
			if (BlockRemoved != null)
			{
				BlockRemoved(this, new BlockRemovedEventArgs(block, blockCoord));
			}
		}

		public event BlockRemovedEvent BlockRemoved;

		private void PerformPicking(float maxDistance)
		{
			Ray ray = PickingRay.HasValue ? PickingRay.Value : new Ray(_camera.Position, _camera.Direction);

			var traverser = new VoxelTraverser(ray, maxDistance);

			if (traverser.Initialize(out _placeableCoord))
			{
				_destructibleCoords = _placeableCoord;
				do
				{
					Block block;
					if (TryGetBlock(_destructibleCoords, out block))
					{
						if (block.IsCollidable)
						{
							HasPickedBlock = true;
							return;
						}
					}
					_placeableCoord = _destructibleCoords;
				} while (traverser.Traverse(out _destructibleCoords));

				HasPickedBlock = false;
				return;
			}

			throw new InvalidOperationException("Should never happen");
		}

		public bool TryGetBlock(IntVector3 worldCoord, out Block result)
		{
			IntVector3 chunkCoordinate, blockCoordinate;
			Chunk.TranslateWorldToChunkBlockCoord(worldCoord, out chunkCoordinate, out blockCoordinate);
			Chunk chunk;
			if (_chunks.TryGetValue(chunkCoordinate, out chunk))
			{
				if (chunk.TryGetBlock(blockCoordinate, out result))
				{
					return true;
				}
			}

			result = default (Block);
			return false;
		}

		private void SortChunksForRenderer()
		{
			_chunksToRender.Clear();
			_chunksToRender.AddRange(RenderableChunks.Where(chunk => chunk.BoundingBox.Intersects(_camera.Frustum)));
			_chunksToRender.Sort((c1, c2) => Vector3.Distance(c1.Center, _camera.Position).CompareTo(Vector3.Distance(c2.Center, _camera.Position)));
		}

		private void EnqueueNewChunksForLoading()
		{
			List<Chunk> chunks = NewChunks.ToList();
			foreach (Chunk chunk in chunks)
			{
				Chunk copy = chunk;
				copy.State = ChunkState.Preparing;
				_taskManager.Schedule(() => GenerateChunk(copy), () => GetChunkPriority(copy));
			}
		}

		private void AttachReadyChunks()
		{
			lock (_syncQueue)
			{
				while (_syncQueue.Count > 0)
				{
					Chunk chunk = _syncQueue.Dequeue();
					if (chunk.State != ChunkState.Deleting)
					{
						foreach (IntVector3 delta in NeighborhoodDeltas)
						{
							Chunk neighbor;
							if (_chunks.TryGetValue(chunk.Position + delta, out neighbor))
							{
								chunk.Connect(neighbor);
							}
						}
						chunk.State = ChunkState.DataInSync;
					}
				}
			}
		}

		private void SaveDirtyChunks()
		{
			foreach (Chunk chunk in _chunks.Values.Where(x => x.State == ChunkState.DataOutOfSync))
			{
				_chunkStore.SaveChunkData(chunk);
			}
		}

		private void GenerateChunk(Chunk chunk)
		{
			if (chunk.State == ChunkState.Deleting)
			{
				return;
			}

			chunk.Blocks = Chunk.AllocateBlocks();

			if (_chunkStore.ContainsChunk(chunk))
			{
				_chunkStore.LoadChunkData(chunk);
			}
			else
			{
				_chunkGenerator.GenerateBlocks(chunk.Position, chunk.Blocks);
				_chunkStore.SaveChunkData(chunk);
			}

			lock (_syncQueue)
			{
				_syncQueue.Enqueue(chunk);
			}
		}

		private float GetChunkPriority(Chunk chunk)
		{
			if (chunk.State == ChunkState.Deleting)
			{
				// Any negative priority means that task is to be canceled by the TaskManager
				return -1.0f;
			}

			return Vector3.Distance(_camera.Position, chunk.Center);
		}

		private bool IsChunkInRange(IntVector3 position, int range)
		{
			return IsChunkInRange(position, _focusChunkPosition, range);
		}

		private bool IsChunkInRange(IntVector3 position, IntVector3 focus, int range)
		{
			IntVector3 delta = position - focus;

			return delta.LengthSquared <= range * range;
		}

		private void RemoveChunks(Predicate<Chunk> predicate)
		{
			_chunkKeysToRemove.Clear();

			foreach (Chunk chunk in _chunks.Values.Where(chunk => predicate(chunk)))
			{
				_chunkKeysToRemove.Add(chunk.Position);
			}

			foreach (IntVector3 key in _chunkKeysToRemove)
			{
				_chunks.Remove(key);
			}
		}

		private void ReallocateChunks(IntVector3 newCenter)
		{
			IntVector3 position;
			for (position.X = newCenter.X - CacheDistance; position.X <= newCenter.X + CacheDistance; position.X++)
			{
				for (position.Y = newCenter.Y - CacheDistance; position.Y <= newCenter.Y + CacheDistance; position.Y++)
				{
					for (position.Z = newCenter.Z - CacheDistance; position.Z <= newCenter.Z + CacheDistance; position.Z++)
					{
						if (IsChunkInRange(position, newCenter, CacheDistance) && !IsChunkInRange(position, _focusChunkPosition, CacheDistance))
						{
							AddChunk(new Chunk(position));
						}
					}
				}
			}
		}

		private void DisposeOfChunksGraphicBuffers()
		{
			foreach (Chunk chunk in _chunks.Values)
			{
				if (chunk.State == ChunkState.DataInSync || chunk.State == ChunkState.DataOutOfSync)
				{
					if (!IsChunkInRange(chunk.Position, RenderCacheDistance))
					{
						chunk.DisposeBuffers();
					}
				}
			}
		}

		private void RemoveChunksOutOfCachingRange(IntVector3 newFocusChunkPosition)
		{
			RemoveChunks(c =>
			{
				if (IsChunkInRange(c.Position, newFocusChunkPosition, CacheDistance))
				{
					return false;
				}
				c.Disconnect();
				c.Dispose();
				c.State = ChunkState.Deleting;
				return true;
			});
		}

		#region Nested type: BlockRemovedEventArgs

		public class BlockRemovedEventArgs
		{
			public BlockRemovedEventArgs(Block block, IntVector3 blockCoord)
			{
				Block = block;
				BlockCoord = blockCoord;
			}

			public Block Block { get; private set; }
			public IntVector3 BlockCoord { get; private set; }
		}

		#endregion
	}
}
