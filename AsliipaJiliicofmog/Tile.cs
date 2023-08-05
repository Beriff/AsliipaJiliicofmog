using Asliipa;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog
{
	enum MaterialType
	{
		Soil,
		Metal,
		Organic
	}
	struct Material
	{
		public MaterialType Type;
		public string Name;
		public float Hardness;
		public bool Sandlike; //causes the tiles of that material to crumble into nearest lower levels
		public float Density;
		public bool Flammable;
	}
	struct RenderMask
	{
		public bool RandomRotation;
		public bool RandomTint;

		public static RenderMask DefaultMask()
		{
			var mask = new RenderMask()
			{
				RandomRotation = false,
				RandomTint = false
			};
			return mask;
		}
		public Texture2D GetTexture(Random r, Texture2D texture)
		{
			Texture2D newtexture = texture.Copy();

			if (RandomRotation)
			{
				newtexture = newtexture.Rotate((Orientation)r.Next(0, 3));
			}
			if (RandomTint)
			{
				var val = r.Next(30, 50);
				newtexture = newtexture.Tint(new Color(val, val, val));
			}
			return newtexture;
		}
	}
	class Tile
	{
		public const int TILESIZE = 32;

		public Texture2D TileTexture;

		public string Name;
		public ushort ID;

		public bool Solid;
		public Material TileMaterial;
		public float MaterialAmount; //How much material is in tile, default is 100
		public RenderMask RenderMask;

		public Tile(Texture2D texture, string name, bool solid, Material tmaterial, float materialamount, RenderMask rm, ushort id)
		{
			TileTexture = texture;
			Name = name;
			Solid = solid;
			TileMaterial = tmaterial;
			MaterialAmount = materialamount;
			RenderMask = rm;
			ID = id;
		}
		public Tile GetInstance() => new Tile(TileTexture, Name, Solid, TileMaterial, MaterialAmount, RenderMask, ID);
		public void Render(Vector2 screen_coords, SpriteBatch sb)
		{
			sb.Draw(TileTexture, screen_coords, Color.White);
		}
	}
	class Chunk
	{
		public const int CHUNKSIZE = 10;
		public const int CHUNKDEPTH = 10;
		public const int CHUNKSIZEPX = CHUNKSIZE * Tile.TILESIZE;

		public Tile[,,] Grid;
		public Dictionary<Vector3, Tile> Changes;
		public static Chunk FillChunk(Tile t)
		{
			Chunk c = new();
			c.Grid = new Tile[CHUNKSIZE, CHUNKSIZE, CHUNKDEPTH];
			NumExtend.XYZ(CHUNKSIZE, CHUNKSIZE, CHUNKDEPTH, (x,y,z) =>
			{
				c.Grid[x, y, z] = t.GetInstance();
			});
			return c;
		}
		public void Render(Vector2 screen_coords, int z_level, SpriteBatch sb)
		{
			NumExtend.XY(CHUNKSIZE, CHUNKSIZE, (x, y) =>
			{
				Grid[x, y, z_level].Render(screen_coords + new Vector2(x * Tile.TILESIZE, y * Tile.TILESIZE), sb);
			});
		}
		public Chunk()
		{
			Grid = new Tile[CHUNKSIZE, CHUNKSIZE, CHUNKDEPTH];
			Changes = new();
		}
		public Chunk(Tile[,,] grid)
		{
			Grid = grid;
			Changes = new();
		}
	}
}
