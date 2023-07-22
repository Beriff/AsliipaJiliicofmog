using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Xna.Framework.Content;

namespace AsliipaJiliicofmog
{
	static class Registry
	{
		public static Dictionary<string, Texture2D> Textures;
		public static Dictionary<string, Material> Materials;
		public static Dictionary<string, Tile> Tiles;
		public static Dictionary<string, Biome> Biomes;
	}
	static class Loader
	{
		public static ContentManager Content;
		public static SpriteBatch Sb;
		public static string Path;

		
		public static void Init(ContentManager content, SpriteBatch sb)
		{
			Content = content;
			Registry.Textures = new();
			Registry.Materials = new();
			Registry.Tiles = new();
			Registry.Biomes = new();
			Sb = sb;
			Path = content.RootDirectory + @"\AsliipaData";
			LoadTextures();
			LoadMaterials();
			LoadTiles();
			LoadBiomes();
		}
		public static void LoadTextures()
		{
			foreach(var filename in Directory.GetFiles(Path + @"\Textures\"))
			{
				var fi = new FileInfo(filename);
				Registry.Textures[fi.Name.Replace(fi.Extension, "")] = Texture2D.FromStream(Sb.GraphicsDevice, new FileStream(filename, FileMode.Open));
			}
		}
		public static void LoadMaterials()
		{
			foreach (var filename in Directory.GetFiles(Path + @"\Data\Materials"))
			{
				var fi = new FileInfo(filename);
				string jsontext = File.ReadAllText(filename);
				dynamic data = JsonConvert.DeserializeObject(jsontext);

				MaterialType type = data.material_type.ToString() switch
				{
					"soil" => MaterialType.Soil,
					"organic" => MaterialType.Organic,
					"metal" => MaterialType.Metal,
					_ => MaterialType.Soil
				};

				Material material = new Material()
				{
					Name = data.name.ToString(), Hardness = float.Parse(data.hardness.ToString()), 
					Density = int.Parse(data.density.ToString()), Flammable = bool.Parse(data.flammable.ToString()), 
					Sandlike = bool.Parse(data.sandlike.ToString()), Type = type
				};
				Registry.Materials[material.Name] = material;
			}
		}
		public static void LoadTiles()
		{
			foreach (var filename in Directory.GetFiles(Path + @"\Data\Tiles"))
			{
				string jsontext = File.ReadAllText(filename);
				dynamic data = JsonConvert.DeserializeObject(jsontext);
				RenderMask mask = new RenderMask()
				{
					RandomRotation = bool.Parse(data.rendermask_rotation.ToString()),
					RandomTint = bool.Parse(data.rendermask_tint.ToString())
				};
				Tile tile = new Tile(
					Registry.Textures[data.texture.ToString()], data.name.ToString(), bool.Parse(data.solid.ToString()),
					Registry.Materials[data.material.ToString()], float.Parse(data.material_amount.ToString()),
					mask
					);
				Registry.Tiles[tile.Name] = tile;
			}
		}
		public static void LoadBiomes()
		{
			foreach(var filename in Directory.GetFiles(Path + @"\Data\Biomes"))
			{
				string jsontext = File.ReadAllText(filename);
				dynamic data = JsonConvert.DeserializeObject(jsontext);
				List<string> reqs = new();
				foreach (var req in data.generation_reqs)
					reqs.Add(req.ToString());
				string name = data.name.ToString();
				string pattern = data.generation_pattern.ToString();
				(float, float) range = (float.Parse(data.generation_height[0].ToString()), float.Parse(data.generation_height[1].ToString()));
				Biome biome = new Biome(name, range, reqs, pattern);
				Registry.Biomes.Add(biome.Name, biome);
			}
		}
	}
}
