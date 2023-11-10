using System;
using AsliipaJiliicofmog.Data;
using AsliipaJiliicofmog.Env;
using AsliipaJiliicofmog.Input;
using AsliipaJiliicofmog.Rendering;
using AsliipaJiliicofmog.Rendering.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace AsliipaJiliicofmog
{
	class Client : Game
	{
		public GraphicsDeviceManager Graphics;
		public SpriteBatch SB;

		public World w;
		public UIGroup MainUI;
		public BitmapFont Fonter;
		public Client()
		{
			Graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}
		protected override void Initialize()
		{
			Graphics.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
			SB = new SpriteBatch(Graphics.GraphicsDevice);
			base.Initialize();
		}
		protected override void LoadContent()
		{
			Registry.Initialize(Content, GraphicsDevice);
			UIElement.Initialize(SB);

			w = new World(SB, 1);
			w.Entities.Add(Registry.Entities["Crate"]);
			w.Emitters.Add(new Emitter(w.Player.Position, Registry.Textures["fire"], (x) => { x.Origin = w.Player.Position; }));
			Fonter = new(Registry.Textures["font"]);
			MainUI = new("main", Content.Load<SpriteFont>("defaultfont"));

			var layout = new GridLayout(Vector2.Zero, new(100), new(10));
			layout.PlaceElement(new(1, 1), new Checkbox(null, new(0), new(1)));

			MainUI.Add(layout);

		}
		protected override void Update(GameTime gameTime)
		{
			InputManager.Update();
			w.Update();
			MainUI.Update();
			base.Update(gameTime);
			Console.WriteLine(Registry.Materials);
		}
		protected override void Draw(GameTime gameTime)
		{
			w.Render(SB, gameTime);
			MainUI.Render(SB);
			base.Draw(gameTime);
		}
	}
}
