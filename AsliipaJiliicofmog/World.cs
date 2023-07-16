using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog
{
	class ValueNoise
	{
		//Noise settings
		public (int a, int b) Range;
		public int Frequency = 15;
		public Dictionary<Vector2, float> LatticeNodes;
		public Random NoiseSeed;
		public ValueNoise((int,int) range, int freq, Random r)
		{
			LatticeNodes = new();
			Frequency = freq;
			Range = range;
			NoiseSeed = r;
		}
		public float GetValue(Vector2 pos)
		{
				if (LatticeNodes.ContainsKey(pos)) { return LatticeNodes[pos]; }
				else { return LatticeNodes[pos] = NoiseSeed.Next(Range.a, Range.b); }
		}
		public virtual float Noise(Vector2 point)
		{
			Func<float,float,float> mod = NumExtend.Mod;

			Vector2 topleft = new(MathF.Floor(point.X / Frequency) * Frequency, MathF.Floor(point.Y / Frequency) * Frequency);
			Vector2 topright = topleft + new Vector2(Frequency, 0);
			Vector2 bottomleft = topleft + new Vector2(0, Frequency);
			Vector2 bottomright = topleft + new Vector2(Frequency, Frequency);
			float tx = (mod(point.X, Frequency) / Frequency);
			float ty = (mod(point.Y, Frequency) / Frequency);
			return NumExtend.Bilerp(
				GetValue(topleft), GetValue(topright), GetValue(bottomleft), GetValue(bottomright), tx, ty
				);
		}
	}
	class OctaveValueNoise
	{
		public List<ValueNoise> Octaves;
		public int NormalizationValue = 0;
		public OctaveValueNoise(Random r, params ((int,int) range, int freq)[] octaves )
		{
			Octaves = new();
			foreach(var octave in octaves)
			{
				NormalizationValue += octave.range.Item2;
				Octaves.Add(new ValueNoise(octave.range, octave.freq, r));
			}
		}
		public float Noise(Vector2 point)
		{
			float val = 0;
			foreach(var noise in Octaves)
			{
				val += noise.Noise(point);
			}
			return val / NormalizationValue;
		}
		public static OctaveValueNoise WorldNoise(Random r) => new OctaveValueNoise(r,
				((0, 255), 70), ((0, 128), 50), ((0, 64), 10), ((0, 32), 5), ((0, 16), 2)
				);
		public static OctaveValueNoise AuxiliaryNoise(Random r) => new OctaveValueNoise(r,
				((0, 128), 40), ((0, 64), 10), ((0, 32), 5), ((0, 16), 2)
				);
	}
	class Biome
	{
		public delegate bool BiomeValidator(World world, Vector2 worldpos);
		public static BiomeValidator RequiredTemperature(float min, float max)
		{
			return (world, worldpos) => { var t = world.TemperatureAt(worldpos); return (t >= min && t <= max); };
		}
		public static BiomeValidator SpawnAlways() => (_,_) => true;
		
		public string Name;
		public Vector2 HeightBounds;
		public BiomeValidator Validator;

	}
	class World
	{
		public Dictionary<Vector2, Chunk> WorldMap;
		public const float MinTemperature = -273.15f; //hi absolute zero
		public const float MaxTemperature = 1e5f; //ten thousand

		protected OctaveValueNoise RawTemperatureMap;
		protected OctaveValueNoise RawHeightmap;
		protected OctaveValueNoise RawBiomeMap;
		public float TemperatureAt(Vector2 pos)
		{
			var temp = RawTemperatureMap.Noise(pos) + pos.Y / 500f;
			temp = NumExtend.Lerp(MinTemperature, MaxTemperature, temp);
			return Math.Clamp(temp, MinTemperature, MaxTemperature);
		}
	}
}
