using Asliipa;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AsliipaJiliicofmog
{
	/* Coordinates Reference
	 * world_pos (WP) - position in world, in pixels
	 * chunk_pos (CHP) - chunk position (ex. {2,2} would be {2,2} * chunkSizePixels in world_pos)
	 * screen_pos (SP) - like world_pos but is bound to the screen (ex. {0,0} is always top left corner)
	 * tile_pos (TP) - tile position (ex. {2,2} would be {2,2} * tileSizePixels in world_pos)
	 * 
	 * Keys for World.WorldMap are measured in CHP
	 * World.RenderDistance is measured in CHP
	 */
	class ValueNoise
	{
		//Noise settings
		public (int a, int b) Range;
		public int Frequency = 15;
		public Dictionary<Vector2, float> LatticeNodes;
		public Random NoiseSeed;
		public int Seed;
		public ValueNoise((int,int) range, int freq, int seed)
		{
			LatticeNodes = new();
			Frequency = freq;
			Range = range;
			NoiseSeed = new Random(seed);
		}
		public float GetValue(Vector2 pos)
		{
				if (LatticeNodes.ContainsKey(pos)) { return LatticeNodes[pos]; }
				else { return LatticeNodes[pos] = 
					new Random(NumExtend.Pairing(Seed, NumExtend.Pairing((int)pos.X, (int)pos.Y) )).Next(Range.a, Range.b); }
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
		public OctaveValueNoise(int seed, params ((int,int) range, int freq)[] octaves )
		{
			Octaves = new();
			foreach(var octave in octaves)
			{
				NormalizationValue += octave.range.Item2;
				Octaves.Add(new ValueNoise(octave.range, octave.freq, seed));
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
		public static OctaveValueNoise WorldNoise(int seed) => new OctaveValueNoise(seed,
				((0, 255), 70), ((0, 128), 50), ((0, 64), 10), ((0, 32), 5), ((0, 16), 2)
				);
		public static OctaveValueNoise AuxiliaryNoise(int seed) => new OctaveValueNoise(seed,
				((0, 128), 40), ((0, 64), 10), ((0, 32), 5), ((0, 16), 2)
				);
	}
	class Biome
	{
		public delegate bool BiomeValidator(World world, Vector3 worldpos);
		public delegate Tile TerrainGenerator(World world, Vector3 worldpos);

		public static BiomeValidator RequiredTemperature(float min, float max)
		{
			return (world, worldpos) => { var t = world.TemperatureAt(worldpos.XY()); return (t >= min && t <= max); };
		}
		public static BiomeValidator SpawnAlways() => (_,_) => true;
		public static BiomeValidator RequiredHeight(float min, float max)
		{
			return (world, worldpos) => { var h = world.GetHeight(worldpos.XY()); return (h >= min && h <= max); };
		}
		public static TerrainGenerator MonotoneTerrain(Tile tile) => (_, _) => tile.GetInstance();

		public string Name;
		public Vector2 HeightBounds;
		public List<BiomeValidator> Validators;
		public TerrainGenerator Generator;

		public Biome(string name, (float a, float b) genheight, List<string> requirements, string genpattern)
		{
			Name = name;
			HeightBounds = new(genheight.a, genheight.b);
			Validators = new();
			Validators.Add(RequiredHeight(genheight.a, genheight.b));
			foreach(var req in requirements)
			{
				Validators.Add(req switch
				{
					"spawn_always" => SpawnAlways(),
					_ => (_, _) => false
				});
			}
			Generator = genpattern switch
			{
				"sea" => MonotoneTerrain(Registry.Tiles["water"]),
				"wasteland" => MonotoneTerrain(Registry.Tiles["dirt"]),
				_ => MonotoneTerrain(Registry.Tiles["dirt"])
			};
		}
		public bool Validate(World world, Vector3 worldpos)
		{
			foreach(var validator in Validators)
			{
				if(!validator(world, worldpos)) { return false; }
			}
			return true;
		}

	}

	struct WorldGenSettings
	{
		//Initial world size (in chunks)
		public int InitialWorldSize;
		public WorldGenSettings(int size)
		{
			InitialWorldSize = size;
		}
	}
	class World
	{
		public Dictionary<Vector2, Chunk> WorldMap;
		public string Name = "world1";

		public const float MinTemperature = -273.15f; //hi absolute zero
		public const float MaxTemperature = 10_000f;

		public const int RenderDistance = 8;
		public const int RenderRadius = (int)( (RenderDistance+1) * NumExtend.Sqrt2 * Chunk.CHUNKSIZEPX);

		public Vector2 Camera;
		public int ZLevel;

		protected OctaveValueNoise RawTemperatureMap;
		protected OctaveValueNoise RawHeightmap;
		public Chunk RequestChunk(Vector2 chunk_pos)
		{
			if(WorldMap.ContainsKey(chunk_pos))
			{
				return WorldMap[chunk_pos];
			}
			if(WorldSerializer.LoadChunk(this, chunk_pos)) { return WorldMap[chunk_pos]; }
			GenerateChunk(chunk_pos);
			return WorldMap[chunk_pos];
		}
		public float TemperatureAt(Vector2 pos)
		{
			var temp = RawTemperatureMap.Noise(pos) + pos.Y / 500f;
			temp = NumExtend.Lerp(MinTemperature, MaxTemperature, temp);
			return Math.Clamp(temp, MinTemperature, MaxTemperature);
		}
		public void GenerateChunk(Vector2 origin)
		{
			#if DEBUG
				Console.WriteLine($"[debug] generating chunk at {origin}");
			#endif

			Tile[,,] grid = new Tile[Chunk.CHUNKSIZE, Chunk.CHUNKSIZE, Chunk.CHUNKDEPTH];

			NumExtend.XYZ(Chunk.CHUNKSIZE, Chunk.CHUNKSIZE, Chunk.CHUNKDEPTH, (x, y, z) =>
			{
				Biome candidate = Registry.Biomes["wasteland"]; // <-- default biome
				Vector3 pos = new Vector3(x, y, z) + origin.Vec3();
				foreach (var biome in Registry.Biomes.Values)
				{
					if (biome.Validate(this, pos))
					{
						candidate = biome;
						break;
					}
				}
				grid[x, y, z] = candidate.Generator(this, pos);
			});
			WorldMap[origin] = new Chunk(grid);
		}
		public void GenerateWorld(WorldGenSettings settings)
		{
			RawHeightmap = OctaveValueNoise.WorldNoise(Client.Seed);
			RawTemperatureMap = OctaveValueNoise.AuxiliaryNoise(Client.Seed);
			NumExtend.XY(settings.InitialWorldSize, settings.InitialWorldSize, (x, y) =>
			{
				GenerateChunk(new Vector2(x, y) * Chunk.CHUNKSIZE);
			});
		}
		public float GetHeight(Vector2 worldpos)
		{
			return RawHeightmap.Noise(worldpos);
		}
		public void Render(SpriteBatch sb)
		{
			//unload chunks that are far away
			foreach(var pos in WorldMap.Keys)
			{
				if ((pos * Tile.TILESIZE).Distance(Camera) > RenderRadius)
				{

					WorldSerializer.UnloadChunk(this, WorldMap[pos], pos);
				}
			}

			Vector2 camera_chunk_origin = Camera - new Vector2(Camera.X % Chunk.CHUNKSIZEPX, Camera.Y % Chunk.CHUNKSIZEPX);
			camera_chunk_origin /= Tile.TILESIZE;
			NumExtend.XY(RenderDistance / 2, RenderDistance / 2, -RenderDistance / 2 , -RenderDistance / 2, (x, y) =>
			{
				Vector2 chunk_origin = camera_chunk_origin + new Vector2(x, y) * Chunk.CHUNKSIZE;
				RequestChunk(chunk_origin);
			});
			foreach(var pos in WorldMap.Keys)
			{
				Chunk chunk = WorldMap[pos];
				chunk.Render(pos * new Vector2(Tile.TILESIZE, Tile.TILESIZE) - Camera, ZLevel, sb);
			}
			
		}
		public World(WorldGenSettings settings)
		{
			WorldMap = new();
			GenerateWorld(settings);
			Camera = Vector2.Zero;
			ZLevel = 5;
		}
	}
}
