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
		public UserInterface GUI;

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
			GUI = new();
			var mainui_g = new UIGroup("main", Registry.DefaultFont);
			GUI.SetGroup(mainui_g);

			var window = Frame.Window(w.WorldEvents, new(10), new(100));
			window.MakeAppear(w.WorldEvents);

			mainui_g.Add(window);
			
		}
		protected override void Update(GameTime gameTime)
		{
			InputManager.Update();
			w.Update();
			GUI.Update();
			base.Update(gameTime);

		}
		protected override void Draw(GameTime gameTime)
		{
			w.Render(SB, gameTime);
			GUI.Render(SB);
			base.Draw(gameTime);
		}
	}
}
