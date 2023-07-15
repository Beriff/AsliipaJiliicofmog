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
		public Animator Animator;
		public Chunk JEREMY;
		public static Random GameRandom;
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
			GameRandom = new();
			Control = new(UIColorPalette.Default(), SB, Content.Load<SpriteFont>("mplus"), Window);
			Animator = new();
			Loader.Init(Content, SB);
			JEREMY = Chunk.FillChunk(Loader.Tiles["dirt"]);
		}
		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			Control.Update(gameTime);
			Animator.Update();
		}
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			SB.Begin();
			JEREMY.Render(Vector2.Zero, SB);
			Control.Render(SB, gameTime);
			SB.End();
			base.Draw(gameTime);
		}
	}
}
