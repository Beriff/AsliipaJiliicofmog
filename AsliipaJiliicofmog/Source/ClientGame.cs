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

			var f = new ScrollFrame(new(20), new(200));
			f.AddElement(new Button(null, () => { }, Vector2.Zero, new(1, 0.1f)) { Label = "h"} );
			f.AddElement(new Button(null, () => { }, new(0, 440), new(1, 0.1f)) { Label = "h" });

			MainUI.Add(f);
			
		}
		protected override void Update(GameTime gameTime)
		{
			InputManager.Update();
			w.Update();
			MainUI.Update();
			base.Update(gameTime);

		}
		protected override void Draw(GameTime gameTime)
		{
			w.Render(SB, gameTime);
			MainUI.Render(SB);
			base.Draw(gameTime);
		}
	}
}
