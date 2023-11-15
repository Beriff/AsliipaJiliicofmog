using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Env
{
	public class Chunk
	{
		public const int Width = 16;
		public const int Height = 16;
		/// <summary>
		/// Chunk size, measured in tiles. Generally, you want to use <c>Chunk.SizePx</c>
		/// </summary>
		public static Vector2 Size { get => new(Width, Height); }
		/// <summary>
		/// Chunk size, measured in pixels.
		/// </summary>
		public static Vector2 SizePx { get => new Vector2(Width, Height) * Tile.Size; }

		public Tile[,] Grid = new Tile[Width, Height];

		public Chunk() { }
		public Chunk(Tile tile)
		{
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					Grid[x, y] = tile.Clone() as Tile;
				}
			}

		}

		public void Render(SpriteBatch sb, Vector2 position)
		{
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					Grid[x, y].Render(sb, position + new Vector2(x, y) * Tile.Size);
				}
			}


		}

		/// <summary>
		/// Given a world position (in pixels), return the origin position of the chunk containing that position
		/// </summary>
		public static Vector2 Modulo(Vector2 position)
		{
			return new Vector2(
				MathF.Floor(position.X / (Width * Tile.Width)),
				MathF.Floor(position.Y / (Height * Tile.Height))
				) * SizePx;
		}
	}
}
