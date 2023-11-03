using System;
using AsliipaJiliicofmog.Data;
using AsliipaJiliicofmog.Env;
using AsliipaJiliicofmog.Input;
using AsliipaJiliicofmog.Rendering;
using AsliipaJiliicofmog.Interactive;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using AsliipaJiliicofmog.Source.Rendering.UI;

namespace AsliipaJiliicofmog
{
	class Client : Game
	{
		public GraphicsDeviceManager Graphics;
		public SpriteBatch SB;

		public World w;
		public UIGroup MainUI = new("main");
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
			w.Entities.Add(new Player());
			w.Entities.Add(Registry.Entities["Crate"]);
			w.Emitters.Add(new Emitter(new Vector2(0, 0), Registry.Textures["fire"]));

			MainUI.Add( new HorizontalBar(Vector2.Zero, new(10)) );
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
			MainUI.Render(SB, MainUI.Palette);
			base.Draw(gameTime);
		}
	}
}
