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

			Registry.MainUI.AddGroup(new("menu"));
			Registry.MainUI["menu"].Add(
				new Frame(DimUI.Global(new(.5f, .5f), new(.5f, .5f)), "testframe")
				{ Pivot = new(.5f, .5f) }
				);
			(Registry.MainUI["menu"]["testframe"] as ContainerUI).Add(
				new Frame(DimUI.Global(new(.5f, .5f), new(.5f, .5f)), "john")
				{ Pivot = new(.5f, .5f), BaseColor = Color.Orange }
				);
			((Registry.MainUI["menu"]["testframe"] as ContainerUI).GetChild("john") as ContainerUI)
				.Add(
				new Frame(DimUI.Global(new(.5f, .5f), new(.5f, .5f)), "matt")
				{ Pivot = new(.5f, .5f), BaseColor = Color.Blue }
				);
		}

		protected override void Update(GameTime gameTime)
		{
			InputManager.Update();
			World.Update();
			Registry.MainUI.Update();
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
