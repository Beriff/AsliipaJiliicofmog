using AsliipaJiliicofmog.Env;
using AsliipaJiliicofmog.Interactive;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace AsliipaJiliicofmog.Data
{
	internal static class Registry
	{
		public static Dictionary<string, Texture2D> Textures = new();
		public static DoubleKeyDict<string, ulong, Tile> Tiles = new();
		public static Dictionary<string, Biome> Biomes = new();
		public static Dictionary<string, Entity> Entities = new();

		private static ulong TileIDCounter = 0;

		private static void GenerateBiomes()
		{
			Biomes.Add("beach", new(
				(w, pos) =>
				{
					return Tiles["sand"];
				},
				(w, pos) =>
				{
					var val = w.Heightmap.Noise(pos);
					return val < .5f && val > .4f;
				},
				"beach"
			));
			Biomes.Add("ocean", new(
				(w, pos) =>
				{
					return Tiles["water"];
				},
				(w, pos) =>
				{
					var val = w.Heightmap.Noise(pos);
					return val < .4f;
				},
				"ocean"
			));
			Biomes.Add("forest", new(
				(w, pos) =>
				{
					return Tiles["Grass"];
				},
				(w, pos) =>
				{
					var val1 = w.Heightmap.Noise(pos);
					var val2 = w.HumidityMap.Noise(pos);
					return val1 > .5f && val2 > .4f;
				},
				"forest"
			));
			Biomes.Add("hills", new(
				(w, pos) =>
				{
					return Tiles["gravel"];
				},
				(w, pos) =>
				{
					var val = w.Heightmap.Noise(pos);
					var val2 = w.HumidityMap.Noise(pos);
					return val > .5f && val2 < .4f;
				},
				"hills"
			));
		}
		private static void GenerateEntities()
		{
			Entities["Carlos"] = new("carlos", Textures["carlos"]);
		}

		public static void Initialize(ContentManager content, GraphicsDevice graphicsDevice)
		{
			string path = Path.Combine(content.RootDirectory, "Textures");
			foreach(var name in Directory.GetFiles(path))
			{
				var filename = Path.GetFileNameWithoutExtension(name);
				var filepath = Path.Combine("Textures", filename);
				Textures[filename] = content.Load<Texture2D>(filepath);
			}

			//load json data
			path = Path.Combine(content.RootDirectory, "Data", "Tiles");

			foreach (var name in Directory.GetFiles(path))
			{
				Tile t = Tile.Deserialize(File.ReadAllText(name));
				Tiles.Add(t.Name, TileIDCounter++, t);
			}

			GenerateBiomes();
			GenerateEntities();
		}
	}
}
