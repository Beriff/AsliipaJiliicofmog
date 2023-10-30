using AsliipaJiliicofmog.Env;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;
using System.IO;

namespace AsliipaJiliicofmog.Data
{
	internal static class Registry
	{
		public static Dictionary<string, Texture2D> Textures = new();
		public static DoubleKeyDict<string, ulong, Tile> Tiles = new();

		private static ulong TileIDCounter = 0;

		private static void GenerateBiomes()
		{

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
		}
	}
}
