using System;
using System.Diagnostics.Contracts;
using Microsoft.Xna.Framework;
using Xu.Core;
using Xu.Extensions;

namespace Xu.Generators
{
	[Serializable]
	public sealed class PerlinNoise : RandomGenerator
	{
		private const int CacheSize = 256;
		private const int CacheMask = 0xff;

		private readonly Vector2[] _grid2 = new Vector2[CacheSize];
		private readonly Vector3[] _grid3 = new Vector3[CacheSize];

		public int OctaveCount
		{
			get { return _octaveCount; }
		}

		private int _octaveCount;
		private float[] _octaveFrequencies;
		private float[] _octaveScales;

		private int[] _permutations = new int[CacheSize];

		public PerlinNoise()
		{
			Initialize();
		}

		public PerlinNoise(int seed) : base(seed)
		{
			Initialize();
		}

		public PerlinNoise(Random random) : base(random)
		{
			Contract.Requires(random != null);
			Contract.EndContractBlock();

			Initialize();
		}

		private void Initialize()
		{
			_permutations = IntegerGenerator.GenerateSequence(0, CacheSize);
			_permutations.Shuffle(Random);

			var vectorGenerator = new RandomVectorGenerator(Random);
			vectorGenerator.FillVector2(_grid2);
			vectorGenerator.FillVector3(_grid3);
		}

		public void AllocateOctaves(int octaves)
		{
			Contract.Requires(octaves > 0);
			Contract.EndContractBlock();

			_octaveCount = octaves;

			_octaveScales = new float[octaves];
			_octaveFrequencies = new float[octaves];
		}

		public void GenerateOctaves(int octaves, float scale, float scaleMultiplier, float frequencyMultiplier)
		{
			Contract.Requires(octaves > 0);
			Contract.EndContractBlock();

			AllocateOctaves(octaves);

			float frequency = 1.0f;

			for (int i = 0; i < octaves; i++)
			{
				_octaveScales[i] = scale;
				_octaveFrequencies[i] = frequency;
				scale *= scaleMultiplier;
				frequency *= frequencyMultiplier;
			}
		}

		public void SetOctave(int octave, float scale, float frequency)
		{
			Contract.Requires(octave < OctaveCount);
			Contract.EndContractBlock();

			_octaveScales[octave] = scale;
			_octaveFrequencies[octave] = frequency;
		}

		public float Sample(float x, float y, int octave)
		{
			Contract.Requires(octave < OctaveCount);
			Contract.EndContractBlock();

			float octaveFrequency = _octaveFrequencies[octave];
			return _octaveScales[octave] * RawSample(octaveFrequency * x, octaveFrequency * y);
		}

		public float Sample(float x, float y, float z, int octave)
		{
			Contract.Requires(octave < OctaveCount);
			Contract.EndContractBlock();

			float octaveFrequency = _octaveFrequencies[octave];
			return _octaveScales[octave] * RawSample(octaveFrequency * x, octaveFrequency * y, octaveFrequency * z);
		}

		public float Sample(float x, float y)
		{
			float sum = 0.0f;

			for (int octave = 0; octave < _octaveCount; octave++)
			{
				sum += Sample(x, y, octave);
			}

			return sum;
		}

		public float Sample(float x, float y, float z)
		{
			float sum = 0.0f;

			for (int octave = 0; octave < _octaveCount; octave++)
			{
				sum += Sample(x, y, z, octave);
			}

			return sum;
		}

		private static float Interpolate(float a, float b, float t)
		{
			// TODO/SETTING: you can change the interpolation type for smoother or faster results
			return XuMath.InterpolateLinear(a, b, t);
		}

		public float RawSample(float x, float y)
		{
			int x0 = XuMath.FloorToInt(x);
			int y0 = XuMath.FloorToInt(y);
			int x1 = x0 + 1;
			int y1 = y0 + 1;

			float xx0 = x - x0;
			float yy0 = y - y0;
			float xx1 = x - x1;
			float yy1 = y - y1;

			int py0 = _permutations[y0 & CacheMask];
			int py1 = _permutations[y1 & CacheMask];

			float s = XuMath.DotProduct(ref _grid2[(x0 + py0) & CacheMask], xx0, yy0);
			float t = XuMath.DotProduct(ref _grid2[(x1 + py0) & CacheMask], xx1, yy0);
			float u = XuMath.DotProduct(ref _grid2[(x0 + py1) & CacheMask], xx0, yy1);
			float v = XuMath.DotProduct(ref _grid2[(x1 + py1) & CacheMask], xx1, yy1);

			float st = Interpolate(s, t, xx0);
			float uv = Interpolate(u, v, xx0);

			return Interpolate(st, uv, yy0);
		}

		public float RawSample(float x, float y, float z)
		{
			int x0 = XuMath.FloorToInt(x);
			int y0 = XuMath.FloorToInt(y);
			int z0 = XuMath.FloorToInt(z);
			int x1 = x0 + 1;
			int y1 = y0 + 1;
			int z1 = z0 + 1;

			float xx0 = x - x0;
			float yy0 = y - y0;
			float zz0 = z - z0;
			float xx1 = x - x1;
			float yy1 = y - y1;
			float zz1 = z - z1;

			int pz0 = _permutations[z0 & CacheMask];
			int pz1 = _permutations[z1 & CacheMask];
			int pyz00 = _permutations[(y0 + pz0) & CacheMask];
			int pyz01 = _permutations[(y0 + pz1) & CacheMask];
			int pyz10 = _permutations[(y1 + pz0) & CacheMask];
			int pyz11 = _permutations[(y1 + pz1) & CacheMask];

			float s0 = XuMath.DotProduct(ref _grid3[(x0 + pyz00) & CacheMask], xx0, yy0, zz0);
			float s1 = XuMath.DotProduct(ref _grid3[(x0 + pyz01) & CacheMask], xx0, yy0, zz1);
			float t0 = XuMath.DotProduct(ref _grid3[(x1 + pyz00) & CacheMask], xx1, yy0, zz0);
			float t1 = XuMath.DotProduct(ref _grid3[(x1 + pyz01) & CacheMask], xx1, yy0, zz1);
			float u0 = XuMath.DotProduct(ref _grid3[(x0 + pyz10) & CacheMask], xx0, yy1, zz0);
			float u1 = XuMath.DotProduct(ref _grid3[(x0 + pyz11) & CacheMask], xx0, yy1, zz1);
			float v0 = XuMath.DotProduct(ref _grid3[(x1 + pyz10) & CacheMask], xx1, yy1, zz0);
			float v1 = XuMath.DotProduct(ref _grid3[(x1 + pyz11) & CacheMask], xx1, yy1, zz1);

			float s = Interpolate(s0, s1, zz0);
			float t = Interpolate(t0, t1, zz0);
			float u = Interpolate(u0, u1, zz0);
			float v = Interpolate(v0, v1, zz0);

			float st = Interpolate(s, t, xx0);
			float uv = Interpolate(u, v, xx0);

			return Interpolate(st, uv, yy0);
		}
	}
}
