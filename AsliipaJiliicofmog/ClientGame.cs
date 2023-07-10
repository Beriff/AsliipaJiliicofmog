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
		public Button b1;
		public Button b2;
		public MenuLink nl;
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
			Control = new(UIColorPalette.Default(), SB, Content.Load<SpriteFont>("mplus"), Window);
			Animator = new();
			var p = new ProgressBar(new(60, 30), new(40, 40), Control, 50, UIMask.Def());
			Animator.Add(
				new Animation(60, (t, d) => { p.Position = new(Easing.OutBack(t)*40, 40); }, data: 40, repeat: false));
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

			Control.Render(SB, gameTime);
			SB.End();
			base.Draw(gameTime);
		}
	}
}
