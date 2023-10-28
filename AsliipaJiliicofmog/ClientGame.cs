using AsliipaJiliicofmog;
using AsliipaJiliicofmog.Data;
using AsliipaJiliicofmog.Env;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog
{
	class Client : Game
	{
		public GraphicsDeviceManager Graphics;
		public SpriteBatch SB;
		public Chunk c;
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
			c = new(Registry.Tiles["Grass"]);
		}
		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			
		}
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			SB.Begin();
			c.Render(SB, Vector2.Zero);
			SB.End();
			
			base.Draw(gameTime);
		}
	}
}
