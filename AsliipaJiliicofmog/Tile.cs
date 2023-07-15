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

	class Tile
	{
		public Texture2D TileTexture;
		public string Name;
		public bool Solid;
		public Material TileMaterial;
		public float MaterialAmount; //How much material is in tile, default is 100

		public Tile(Texture2D texture, string name, bool solid, Material tmaterial, float materialamount)
		{
			TileTexture = texture;
			Name = name;
			Solid = solid;
			TileMaterial = tmaterial;
			MaterialAmount = materialamount;
		}
	}
}
