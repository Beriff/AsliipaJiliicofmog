using AsliipaJiliicofmog.Env;
using AsliipaJiliicofmog.Env.Item;
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

		public static UserInterface UI { get; set; } = new();

		public static SpriteFont DefaultFont { get; set; }

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

		private static void GenerateUIs(GraphicsDevice gd)
		{
			var viewport = gd.Viewport.Bounds;
			var vp_size = viewport.Size.ToVector2();
			var half_vp = vp_size / 2;

			// Generate a container for main menus
			var menuGroup = new UIGroup("MenuGroup", DefaultFont);
			UI.SetGroup(menuGroup);
			// Make any main menu appear/disappear when Esc pressed
			UIElement.LocalInput.AddListener(
				input =>
				{ if (input.GetKeyState(Keys.Escape) == Input.PressType.Released) menuGroup.Toggle(); }
				);

			// A frame for "paused menu"
			var pauseMenu = Frame.Window("Game Paused", DefaultFont, half_vp, half_vp);
			pauseMenu.Pivot = new(.5f, .5f);
			pauseMenu.AddElement(new Button(null, () => { pauseMenu.MakeDisappear(); }, pauseMenu.Size / 2,
				new(.5f, .15f))
			{
				Label = "Resume",
				Name = $"pause_resumebtn",
				Pivot = new(.5f, .5f),
			}
				);

			menuGroup.Add(pauseMenu);
			menuGroup.Disable();
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
			GenerateUIs(graphicsDevice);
		}
	}
}
