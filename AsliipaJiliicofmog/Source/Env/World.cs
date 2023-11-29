using AsliipaJiliicofmog.Event;
using AsliipaJiliicofmog.Input;
using AsliipaJiliicofmog.Interactive;
using AsliipaJiliicofmog.Math;
using AsliipaJiliicofmog.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Env
{
	/// <summary>
	/// A class that generates value noise given a seed
	/// </summary>
	public class ValueNoise
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

	/// <summary>
	/// A class that supports multiple layers of <c cref="ValueNoise">ValueNoise</c> for a more natural texture
	/// </summary>
	public class OctaveValueNoise
	{
		public List<ValueNoise> Octaves;
		public int NormalizationValue = 1;
		public OctaveValueNoise(int seed, params ((int, int) range, int freq)[] octaves)
		{
			Octaves = new();
			foreach (var (range, freq) in octaves)
			{
				NormalizationValue += range.Item2;
				Octaves.Add(new ValueNoise(range, freq, seed));
			}
		}
		/// <summary>
		/// Get a noise float value
		/// </summary>
		/// <param name="point">a 2D point where the value should be sampled</param>
		/// <returns>a float value at the given point</returns>
		public float Noise(Vector2 point)
		{
			float val = 0;
			foreach (var noise in Octaves)
			{
				val += noise.Noise(point);
			}
			return val / NormalizationValue;
		}
		public static OctaveValueNoise WorldNoise(int seed) => new(seed,
				((0, 255), 70), ((0, 128), 50), ((0, 64), 10), ((0, 32), 5), ((0, 16), 2)
				);
		public static OctaveValueNoise AuxiliaryNoise(int seed) => new(seed,
				((0, 128), 40), ((0, 64), 10), ((0, 32), 5), ((0, 16), 2)
				);
	}
	public struct Camera
	{
		public Vector2 Position;
		public int RenderDistance;
		public float Scale;
	}
	/// <summary>
	/// A global state that represents the game world
	/// </summary>
	public class World
	{
		public OctaveValueNoise Heightmap;
		public OctaveValueNoise TemperatureMap;
		public OctaveValueNoise HumidityMap;

		public Dictionary<Vector2, Chunk> Chunks;
		public List<Entity> Entities;
		public List<Emitter> Emitters = new();
		public EventManager WorldEvents;

		public RenderTarget2D RenderTexture;

		public Camera Camera;

		public Player Player;

		public World(SpriteBatch sb, int seed)
		{
			Heightmap = OctaveValueNoise.WorldNoise(seed);
			TemperatureMap = OctaveValueNoise.AuxiliaryNoise(seed);
			HumidityMap = OctaveValueNoise.AuxiliaryNoise(seed + 1);
			Chunks = new();
			Entities = new();
			Camera = new() { Position = Vector2.Zero, RenderDistance = 3, Scale = 1 };
			WorldEvents = new();
			RenderTexture = new(sb.GraphicsDevice, sb.GraphicsDevice.Viewport.Width, sb.GraphicsDevice.Viewport.Height);

			Player = new();
			Camera.Position += Player.Texture.Size.ToVector2() / 2;
			Entities.Add(Player);
		}

		public Entity? FindFirstEntity<T>() where T : Entity
		{
			foreach (var e in Entities)
			{
				if (e.GetType() == typeof(T))
					return e;
			}
			return null;
		}

		public Vector2 GetWorldPosition(Vector2 p, SpriteBatch sb) => p - Camera.Position + sb.GraphicsDevice.Viewport.Bounds.Size.ToVector2() / 2;
		/// <summary>
		/// Generate the tile at the given position
		/// </summary>
		/// <remarks>Returns the singleton tile object. For modifiable tile use <c>GenerateTile().Copy()</c></remarks>
		public Tile GenerateTile(Vector2 position)
		{
			foreach (var biome in Biome.Biomes)
			{
				if (biome.TestTile(this, position))
				{
					return biome.GetTile(this, position);
				}
			}
			return Biome.Fallback.GetTile(this, position);
		}

		/// <summary>
		/// Generate a chunk and add it to <c>World.Chunks</c>. Regenerates a chunk if it has been generated before.
		/// </summary>
		/// <param name="topleft">Origin (top left corner) of the chunk. Measured in pixels.</param>
		public Chunk GenerateChunk(Vector2 topleft)
		{
			Console.WriteLine($"\u001b[32m[Debug]\u001b[0m ChunkGen \u001b[30;1m{topleft / Chunk.SizePx}\u001b[0m");
			Chunk chunk = new();
			Vector2 tilepos = topleft / Tile.Size;
			for (int x = 0; x < Chunk.Width; x++)
			{
				for (int y = 0; y < Chunk.Height; y++)
				{
					chunk.Grid[x, y] = GenerateTile(tilepos + new Vector2(x, y)).Clone() as Tile;
				}
			}
			Chunks[topleft] = chunk;
			return chunk;
		}

		/// <summary>
		/// Get a chunk at requested chunk origin position, or generate one if there's none
		/// </summary>
		public Chunk RequestChunk(Vector2 topleft)
		{
			if (Chunks.ContainsKey(topleft)) { return Chunks[topleft]; }
			else { return GenerateChunk(topleft); }
		}

		/// <summary>
		/// Get a list of chunk origin positions that will be rendered
		/// </summary>
		public List<Vector2> RenderedChunks()
		{
			List<Vector2> list = new();
			var main_chunk_coords = Chunk.Modulo(Camera.Position);
			var rendered_px = Chunk.SizePx * Camera.RenderDistance;
			for (float x = main_chunk_coords.X - rendered_px.X; x < main_chunk_coords.X + rendered_px.X; x += rendered_px.X)
			{
				for (float y = main_chunk_coords.Y - rendered_px.Y; y < main_chunk_coords.Y + rendered_px.Y; y += rendered_px.Y)
				{
					list.Add(new(x, y));
				}
			}
			return list;
		}

		/// <summary>
		/// Render the world, as viewed from World.Camera
		/// </summary>
		/// <param name="sb"></param>
		public void Render(SpriteBatch sb, GameTime gt)
		{
			sb.GraphicsDevice.Clear(Color.Black);
			sb.GraphicsDevice.SetRenderTarget(RenderTexture);
			sb.Begin();

			(int x, int y) vp = new(sb.GraphicsDevice.Viewport.Width, sb.GraphicsDevice.Viewport.Height);
			Vector2 middle_px = new(vp.x / 2, vp.y / 2);
			Vector2 middle_chunk_origin_px = middle_px - Chunk.SizePx / 2;
			Vector2 tl_chunk_origin_px = middle_chunk_origin_px - Chunk.SizePx * Camera.RenderDistance;
			Vector2 tl_chunk_coords = Chunk.Modulo(Camera.Position) - Chunk.SizePx * Camera.RenderDistance;
			Vector2 br_chunk_coords = Chunk.Modulo(Camera.Position) + Chunk.SizePx * Camera.RenderDistance;
			Vector2 CameraShift = Camera.Position.Mod(Chunk.SizePx);

			for (int x = (int)tl_chunk_coords.X; x < br_chunk_coords.X; x += (int)Chunk.SizePx.X)
			{
				for (int y = (int)tl_chunk_coords.Y; y < br_chunk_coords.Y; y += (int)Chunk.SizePx.Y)
				{
					int steps_x = (int)(x - tl_chunk_coords.X) / (int)Chunk.SizePx.X;
					int steps_y = (int)(y - tl_chunk_coords.Y) / (int)Chunk.SizePx.Y;

					RequestChunk(new(x, y)).Render(sb,
						tl_chunk_origin_px + Chunk.SizePx * new Vector2(steps_x, steps_y) - CameraShift);
				}
			}

			//Render entities
			foreach (var e in Entities)
			{
				e.RenderInWorld(sb, gt, this);
			}
			foreach (Emitter x in Emitters) x.Render(sb, this);

			sb.End();
			sb.GraphicsDevice.SetRenderTarget(null);
			sb.Begin(samplerState: SamplerState.PointWrap);


			sb.Draw(RenderTexture,
				middle_px, null, Color.White, 0f, middle_px,
				Camera.Scale, SpriteEffects.None, 0);
			sb.End();
		}

		public void Update()
		{
			Camera.Scale =
				MathHelper.Clamp(
					Camera.Scale + InputManager.GetConsumer("Gameplay").GetScrollDelta() / 1000f,
					1f, 3f);

			Entities.Sort((e1, e2) => e1.Position.Y.CompareTo(e2.Position.Y));
			foreach (var e in Entities) { e.Update(this); }
			foreach (Emitter x in Emitters) x.Update();
			WorldEvents.Update();
			//TODO async IO to offload the chunk info
		}
	}
}
