
using AsliipaJiliicofmog.Data;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace AsliipaJiliicofmog.Env
{
	internal class Biome
	{
		public static List<Biome> Biomes { get; set; } = new();
		public static readonly Biome Fallback = 
			new((_,_) => Registry.Tiles["Grass"], (_,_) => true, "unknown", 0.01f);

		public Func<World, Vector2, Tile> GetTile;
		public Func<World, Vector2, bool> TestTile;
		public float Weight;
		public string Name;

		public Biome
			(Func<World, Vector2, Tile> getTile, Func<World, Vector2, bool> testTile,
			string name, float weight = 1f )
		{
			GetTile = getTile;
			TestTile = testTile;
			Weight = weight;
			Name = name;

			Biomes.Add(this);
			Biomes.Sort((x, y) => x.Weight.CompareTo(y.Weight));
		}
	}
}
