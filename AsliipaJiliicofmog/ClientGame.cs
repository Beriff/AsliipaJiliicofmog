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
		public Frame f;
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

			f = new Frame(new(200, 200), new(15, 15), Control);
			f.AddElement(new Textbox(new RelativePosition(f, (1, 1), (0, 0)), Control,
				"test"
				));
			f.AddElement(new ProgressBar(new RelativePosition(f, (1, .5f), (0, .25f)), Control, 100) { Progress = 50 });
			var b = new Slider(new(200, 30), new(150, 150), Control);

		}
		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			Control.Update(gameTime);
			f.Position = 50*new Vector2((float)Math.Cos(gameTime.TotalGameTime.TotalSeconds), (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds));
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
