using AsliipaJiliicofmog.Data;
using AsliipaJiliicofmog.Env;
using AsliipaJiliicofmog.Input;
using AsliipaJiliicofmog.Rendering;
using AsliipaJiliicofmog.Rendering.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Newtonsoft.Json;

namespace AsliipaJiliicofmog
{
	class Client : Game
	{
		public GraphicsDeviceManager Graphics;
		public SpriteBatch SB;
		public World World;
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

			World = new World(SB, 1);
			World.Entities.Add(Registry.Entities["Crate"]);
			World.Entities.Add(Registry.Entities["Tree"]);
			Fonter = new(Registry.Textures["font"]);
			
		}

		protected override void Update(GameTime gameTime)
		{
			InputManager.Update();
			World.Update();
			Registry.MainUI.Update();
			ElementUI.EventsUI.Update();
			base.Update(gameTime);
        }

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);
			
			World.Render(SB, gameTime);
			SB.Begin();
			Registry.MainUI.Render(SB);
			
			
			SB.End();
			base.Draw(gameTime);
		}
	}
}
