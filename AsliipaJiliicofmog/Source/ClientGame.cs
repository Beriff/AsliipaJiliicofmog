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
			UIElement.Initialize(SB);

			World = new World(SB, 1);
			World.Entities.Add(Registry.Entities["Crate"]);
			World.Entities.Add(Registry.Entities["Tree"]);
			Fonter = new(Registry.Textures["font"]);
		}

		protected override void Update(GameTime gameTime)
		{
			Registry.UI.Update();
			InputManager.Update();
			World.Update();
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			World.Render(SB, gameTime);
			Registry.UI.Render(SB);
			base.Draw(gameTime);
		}
	}
}
