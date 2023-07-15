using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Xna.Framework.Content;

namespace AsliipaJiliicofmog
{
	static class Loader
	{
		public static ContentManager Content;
		public static SpriteBatch Sb;
		public static string Path;

		public static Dictionary<string, Texture2D> Textures;
		public static Dictionary<string, Material> Materials;
		public static Dictionary<string, Tile> Tiles;
		public static void Init(ContentManager content, SpriteBatch sb)
		{
			Content = content;
			Textures = new();
			Materials = new();
			Tiles = new();
			Sb = sb;
			Path = content.RootDirectory + @"\AsliipaData";
			LoadTextures();
			LoadMaterials();
			LoadTiles();
		}
		public static void LoadTextures()
		{
			foreach(var filename in Directory.GetFiles(Path + @"\Textures\"))
			{
				var fi = new FileInfo(filename);
				Textures[fi.Name.Replace(fi.Extension, "")] = Texture2D.FromStream(Sb.GraphicsDevice, new FileStream(filename, FileMode.Open));
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
				Materials[material.Name] = material;
			}
		}
		public static void LoadTiles()
		{
			foreach (var filename in Directory.GetFiles(Path + @"\Data\Tiles"))
			{
				string jsontext = File.ReadAllText(filename);
				dynamic data = JsonConvert.DeserializeObject(jsontext);
				Tile tile = new Tile(
					Textures[data.texture.ToString()], data.name.ToString(), bool.Parse(data.solid.ToString()),
					Materials[data.material.ToString()], float.Parse(data.material_amount.ToString())
					);
				Tiles[tile.Name] = tile;
			}
		}
	}
}
