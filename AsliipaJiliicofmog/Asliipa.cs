using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using AsliipaJiliicofmog.VisualElements;

using System;
using Microsoft.Xna.Framework.Audio;

namespace AsliipaJiliicofmog
{
	public class Asliipa : Game
	{
		public static Random Random = new();
		public const string ASSETSPATH = @"C:\Users\Maxim\Desktop\assets\";

		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		public Scene scene;
		public Vector2 offset;
		public GameClient Client;

		public GUI ClientGUI = new();
		public TextPiece fps;

		public static Color MainGUIColor = new(32, 32, 32, 137);

		public static SpriteFont GameFont;
		public static Font MainFont;

		//when UI is focused, dont move camera with mouse drag, apply drag to sliders and other UI elements instead
		public static bool UIFocused = false;

		public static void SetUIFocus(bool val)
		{
			UIFocused = val;
			if (!val)
				InputHandler.MouseDragOffset = new(0);
		}
		public Asliipa()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			UIFocused = false;
		}

		protected override void Initialize()
		{
			

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			GUI.Init(GraphicsDevice);

			GameFont = Content.Load<SpriteFont>("GameFont");

			Registry.LoadTextureDirectory(ASSETSPATH + "textures", GraphicsDevice);
			Registry.LoadFoodDirectory(ASSETSPATH + @"gamedata\food");
			Registry.LoadCreatureDirectory(ASSETSPATH + @"gamedata\creatures");
			MainFont = new Font(new SpriteAtlas(Registry.TextureRegistry["font2"]));
			fps = new TextPiece(MainFont, "60 fps");

			GameAudio.SFX["click"] = Content.Load<SoundEffect>("click");


			scene = new Scene(new Tile("dirt", Registry.TextureRegistry["dirt"]));
			Client = new GameClient(scene, _spriteBatch);
			Registry.LoadPropDirectory(ASSETSPATH + @"gamedata\props", Client);
			var grass = Registry.GetProp("grassland commonweed");
			grass.AddToRender(Client);
			grass.Move(Client, new(15));

			ParticleEmitter.Circle(new(25), 300, 10).AddToRender(Client);
			new Creature(Registry.TextureRegistry["crate"], "crate", pos: new(150)).AddToRender(Client);

			var campfire = new Campfire(Client);
			campfire.AddToRender(Client);
			campfire.Move(Client, new(50));
		}

		protected override void Update(GameTime gameTime)
		{
			if (InputHandler.GetKeyState(Keys.Z) == KeyStates.JPressed)
			{
				GUI.GUIDEBUG = !GUI.GUIDEBUG;
				Client.GameChat.Announce("toggled GUI debug mode");
			}
				
			if (Client.ExitRequested)
				Exit();

			
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			_spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			Client.Update(_spriteBatch, gameTime);
			fps.Text = Math.Ceiling(1d / gameTime.ElapsedGameTime.TotalSeconds).ToString() + " fps";
			fps.Render(_spriteBatch, Vector2.One);
			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
