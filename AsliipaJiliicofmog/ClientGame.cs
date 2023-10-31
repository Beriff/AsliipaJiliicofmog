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
		public World w;
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
            w = new World(1);
            w.Entities.Add(Registry.Entities["Carlos"]);
		}
		protected override void Update(GameTime gameTime)
		{
			if(Keyboard.GetState().IsKeyDown(Keys.D))
			{
				w.Camera.Position += Vector2.UnitX;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.A))
			{
				w.Camera.Position -= Vector2.UnitX;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.W))
			{
				w.Camera.Position -= Vector2.UnitY;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.S))
			{
				w.Camera.Position += Vector2.UnitY;
			}

			base.Update(gameTime);
			
		}
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			SB.Begin();
			w.Render(SB);
			SB.End();
			
			base.Draw(gameTime);
		}
	}
}
