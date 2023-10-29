using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog.Env
{
	internal class Chunk
	{
		public const int Width = 16;
		public const int Height = 16;
		public static Vector2 Size { get => new(Width, Height); }
		public static Vector2 SizePx { get => new Vector2(Width, Height) * Tile.Size; }

		public Tile[,] Grid = new Tile[Width, Height];

		public Chunk() { }
		public Chunk(Tile tile)
		{
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{ 
					Grid[x, y] = tile.Copy(); 
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

		//transform world position into parent chunk's origin position
		public static Vector2 Modulo(Vector2 position)
		{
			return new(MathF.Floor(position.X / Width), MathF.Floor(position.Y / Height));
		}
	}
}
