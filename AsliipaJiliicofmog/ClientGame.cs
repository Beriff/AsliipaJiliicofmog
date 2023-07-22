using Asliipa;
using AsliipaJiliicofmog;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
		public World MainWorld;
		public static Random GameRandom;
		public Texture2D noisetexture;
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
			MainWorld = new World(new WorldGenSettings(10));
			//JEREMY = Chunk.FillChunk(Registry.Tiles["dirt"]);


			/*noisetexture = new Texture2D(GraphicsDevice, 500, 500);
			var noise = OctaveValueNoise.WorldNoise(GameRandom);
			var data = new Color[500 * 500];
			NumExtend.XY(500, 500, (x, y) =>
			{
				float val = noise.Noise(new Vector2(x,y));
				Color col = Color.White;
				if (val < .375f)
					col = Color.Blue;
				else if (val < .4f)
					col = Color.Cyan;
				else if (val < .45f)
					col = Color.Beige;
				else if (val < .7f)
					col = Color.Green;
				else if (val < .75f)
					col = Color.Chocolate;
				else
					col = Color.White;
				data[NumExtend.Flatten(x, y, 500)] = col;
			});
			noisetexture.SetData(data);*/
		}
		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			Control.Update(gameTime);
			Animator.Update();
			if(Control.Input.GetState(Keys.D) == PressState.Pressed)
			{
				MainWorld.Camera += new Vector2(1, 0);
			}
			if (Control.Input.GetState(Keys.S) == PressState.Pressed)
			{
				MainWorld.Camera += new Vector2(0, 1);
			}
		}
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			SB.Begin();
			Control.Render(SB, gameTime);
			MainWorld.Render(SB);
			//SB.Draw(noisetexture, Vector2.Zero, Color.White);
			SB.DrawString(Control.Font, $"Asliipa Build 0.0.46 \n{MathF.Round(1/(float)gameTime.ElapsedGameTime.TotalSeconds, 2)} fps", Vector2.Zero, Color.White);
			SB.End();
			
			base.Draw(gameTime);
		}
	}
}
