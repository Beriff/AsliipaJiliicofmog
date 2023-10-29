using AsliipaJiliicofmog.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog.Env
{
	class ValueNoise
	{
		//Noise settings
		public (int a, int b) Range;
		public int Frequency = 15;
		public Dictionary<Vector2, float> LatticeNodes;
		public Random NoiseSeed;
		public int Seed;
		public ValueNoise((int, int) range, int freq, int seed)
		{
			LatticeNodes = new();
			Frequency = freq;
			Range = range;
			NoiseSeed = new Random(seed);
		}
		public float GetValue(Vector2 pos)
		{
			if (LatticeNodes.ContainsKey(pos)) { return LatticeNodes[pos]; }
			else
			{
				return LatticeNodes[pos] =
				new Random(NumHelper.Pairing(Seed, NumHelper.Pairing((int)pos.X, (int)pos.Y))).Next(Range.a, Range.b);
			}
		}
		public virtual float Noise(Vector2 point)
		{
			Func<float, float, float> mod = NumHelper.Mod;

			Vector2 topleft = new(MathF.Floor(point.X / Frequency) * Frequency, MathF.Floor(point.Y / Frequency) * Frequency);
			Vector2 topright = topleft + new Vector2(Frequency, 0);
			Vector2 bottomleft = topleft + new Vector2(0, Frequency);
			Vector2 bottomright = topleft + new Vector2(Frequency, Frequency);
			float tx = (mod(point.X, Frequency) / Frequency);
			float ty = (mod(point.Y, Frequency) / Frequency);
			return NumHelper.Bilerp(
				GetValue(topleft), GetValue(topright), GetValue(bottomleft), GetValue(bottomright), tx, ty
				);
		}
	}
	class OctaveValueNoise
	{
		public List<ValueNoise> Octaves;
		public int NormalizationValue = 0;
		public OctaveValueNoise(int seed, params ((int, int) range, int freq)[] octaves)
		{
			Octaves = new();
			foreach (var (range, freq) in octaves)
			{
				NormalizationValue += range.Item2;
				Octaves.Add(new ValueNoise(range, freq, seed));
			}
		}
		public float Noise(Vector2 point)
		{
			float val = 0;
			foreach (var noise in Octaves)
			{
				val += noise.Noise(point);
			}
			return val / NormalizationValue;
		}
		public static OctaveValueNoise WorldNoise(int seed) => new (seed,
				((0, 255), 70), ((0, 128), 50), ((0, 64), 10), ((0, 32), 5), ((0, 16), 2)
				);
		public static OctaveValueNoise AuxiliaryNoise(int seed) => new (seed,
				((0, 128), 40), ((0, 64), 10), ((0, 32), 5), ((0, 16), 2)
				);
	}
	internal struct WorldView
	{
		public Vector2 Position;
		public int RenderDistance;
		public float Scale;
	}
	internal class World
	{
		public OctaveValueNoise Heightmap;
		public OctaveValueNoise TemperatureMap;
		public OctaveValueNoise HumidityMap;

		public Dictionary<Vector2, Chunk> Chunks;

		WorldView View;

		public World(int seed)
		{
			Heightmap = OctaveValueNoise.WorldNoise(seed);
			TemperatureMap = OctaveValueNoise.AuxiliaryNoise(seed);
			HumidityMap = OctaveValueNoise.AuxiliaryNoise(seed + 1);
			Chunks = new();
			View = new() { Position = Vector2.Zero, RenderDistance = 3, Scale = 1 };
		}
		public Tile GenerateTile(Vector2 position)
		{
			foreach(var biome in Biome.Biomes) {
				if(biome.TestTile(this,position))
				{
					return biome.GetTile(this, position);
				}
			}
			return Biome.Fallback.GetTile(this, position);
		}

		public Chunk GenerateChunk(Vector2 topleft)
		{
            Console.WriteLine($"[Debug] Requested chunk gen at {topleft}");
            Chunk chunk = new();
			for(int x = 0; x < Chunk.Width; x++)
			{
				for(int y = 0; y < Chunk.Height; y++)
				{
					chunk.Grid[x, y] = GenerateTile(topleft + new Vector2(x,y) );
				}
			}
			Chunks[topleft] = chunk;
			return chunk;
		}
		public Chunk RequestChunk(Vector2 topleft)
		{
			if (Chunks.ContainsKey(topleft)) { return Chunks[topleft]; }
			else { return GenerateChunk(topleft); }
		}

		public void Render(SpriteBatch sb)
		{
			(int x, int y) vp = new(sb.GraphicsDevice.Viewport.Width, sb.GraphicsDevice.Viewport.Height);
			Vector2 middle_px = new(vp.x / 2, vp.y / 2);
			Vector2 middle_chunk_origin_px = middle_px - Chunk.SizePx / 2;
			Vector2 tl_chunk_origin_px = middle_chunk_origin_px - Chunk.SizePx * View.RenderDistance;
			Vector2 tl_chunk_coords = Chunk.Modulo(View.Position) - Chunk.SizePx * View.RenderDistance;
			Vector2 br_chunk_coords = Chunk.Modulo(View.Position) + Chunk.SizePx * View.RenderDistance;

			for (int x = (int)tl_chunk_coords.X; x < br_chunk_coords.X; x += (int)Chunk.SizePx.X) 
			{
				for (int y = (int)tl_chunk_coords.Y; y < br_chunk_coords.Y; y += (int)Chunk.SizePx.Y)
				{
					int steps_x = (int)(x - tl_chunk_coords.X) / (int)Chunk.SizePx.X;
					int steps_y = (int)(y - tl_chunk_coords.Y) / (int)Chunk.SizePx.Y;
                    Vector2 chunk_pos = new(x, y);

					RequestChunk(chunk_pos).Render(sb,
						tl_chunk_origin_px + Chunk.SizePx * new Vector2(steps_x, steps_y));
				}
			}
		}
	}
}
