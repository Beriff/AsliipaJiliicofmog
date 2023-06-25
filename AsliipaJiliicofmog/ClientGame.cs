using Asliipa;
using AsliipaJiliicofmog;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asliipa
{
	class Client : Game
	{
		public GraphicsDeviceManager Graphics;
		public SpriteBatch SB;
		public UIControl Control;
		public Scrollbox f;
		public Client()
		{
			Graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}
		protected override void Initialize()
		{
			Graphics.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
			base.Initialize();
		}
		protected override void LoadContent()
		{
			SB = new SpriteBatch(GraphicsDevice);

			
			Control = new(UIColorPalette.Default(), SB, Content.Load<SpriteFont>("mplus"));

			/*f = new Scrollbox(new(150, 150), new(30, 30), Control);
			f.AddElement(new ProgressBar(new RelativePosition(f, (1, .3f), (0, -.5f)), Control, 100));
			f.AddElement(new ProgressBar(new RelativePosition(f, (1, .3f), (0, .5f)), Control, 100));
			f.AddElement(new ProgressBar(new RelativePosition(f, (1, .3f), (0, 1)), Control, 100));*/
			new Combobox(new(100, 30), new(50, 50), Control, "1920x1080", "1600x700", "4K");
		}
		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			Control.Update(gameTime);
		}
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			SB.Begin();

			Control.Render(SB, gameTime);
			SB.End();
			base.Draw(gameTime);
		}
	}
}
