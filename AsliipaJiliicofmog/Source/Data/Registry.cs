﻿using AsliipaJiliicofmog.Env;
using AsliipaJiliicofmog.Env.Items;
using AsliipaJiliicofmog.Input;
using AsliipaJiliicofmog.Interactive;
using AsliipaJiliicofmog.Rendering;
using AsliipaJiliicofmog.Rendering.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AsliipaJiliicofmog.Data
{
	public static class Registry
	{
		public static Dictionary<string, Texture2D> Textures { get; set; } = new();
		public static DoubleKeyDict<string, ulong, Tile> Tiles { get; set; } = new();
		public static Dictionary<string, Biome> Biomes { get; set; } = new();
		public static Dictionary<string, Entity> Entities { get; set; } = new();
		public static Dictionary<string, Material> Materials { get; set; } = new();
		public static Dictionary<string, Item> Items { get; set; } = new();

		public static RenderTarget2D? CurrentRenderTarget = null;

		public static UI MainUI { get; set; } = new();

		public static SpriteFont DefaultFont { get; set; }

		private static ulong TileIDCounter = 0;

		private static void GenerateItems(GraphicsDevice gd)
		{
			Items["stick"] = new("stick", Textures["arrow_down"], Materials["Wood"]);
			Inventory inv = new();
			inv.Add(Items["stick"]);
			inv.Add(Items["stick"]);
		}
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
		private static void GenerateEntities(GraphicsDevice gd)
		{
			Entity.BlankTexture = new(gd, 1, 1);
			Entity.BlankTexture.SetData(new Color[] { Color.White });

			Entities["Crate"] = new PhysicalEntity("Crate", new GameTexture(Textures["crate"]))
			{
				Position = new(25),
			}.SetBottomHitbox();

			Entities["Tree"] = new PhysicalEntity("Tree", new GameTexture(Textures["tree"]))
			{
				Position = new(64),
				TextureOffsetPivot = new(0, 1),
				HitboxScale = new(.5f, .2f),
			};
		}
		private static void GenerateUI(GraphicsDevice gd)
		{
			MainUI.AddGroup(new("menus"));

			//create main menu tab
			var mainwindow =
				Frame.Window("Main Menu", DimUI.Global(new(.5f, .5f), new(.5f, .5f)));
			mainwindow.Pivot = new(.5f, .5f);
			mainwindow.Add(new HorizontalScrollbar(new(new(0,.5f), new(0), new(0, 20), new(1, 0))));
			MainUI["menus"].Add(mainwindow);
			//make UI toggleable on esc key
			ElementUI.Input.AddListener(
				input =>
				{
					if (input.GetKeyState(Keys.Escape) == PressType.Pressed)
						MainUI["menus"].Toggle();
				});

		}

		public static void Initialize(ContentManager content, GraphicsDevice graphicsDevice)
		{
			string path = Path.Combine(content.RootDirectory, "Textures");
			foreach (var name in Directory.GetFiles(path))
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

			path = Path.Combine(content.RootDirectory, "Data", "Materials");

			foreach (var name in Directory.GetFiles(path))
			{
				Material m = Material.Deserialize(File.ReadAllText(name));
                Materials.Add(m.Name, m);
			}

			DefaultFont = content.Load<SpriteFont>("defaultfont");

			GenerateBiomes();
			GenerateEntities(graphicsDevice);
			GenerateItems(graphicsDevice);
			ElementUI.Initialize(graphicsDevice);
			GenerateUI(graphicsDevice);
		}
	}
}
