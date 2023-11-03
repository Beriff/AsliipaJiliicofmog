using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog.Rendering
{
	internal interface IGameTexture
	{
		public void Render(SpriteBatch sb, Vector2 position, Color color);
		public Point Size { get; }
	}

	internal class GameTexture : IGameTexture
	{
		public readonly Texture2D Texture;
		public Point Size { get => Texture.Bounds.Size; }

		public GameTexture(Texture2D texture)
		{
			Texture = texture;
		}

		public void Render(SpriteBatch sb, Vector2 position, Color color)
		{
			sb.Draw(Texture, position, color);
		}

		public static implicit operator Texture2D(GameTexture gt) => gt.Texture;
	}

	internal class AnimatedTexture : IGameTexture
	{
		private readonly Texture2D Frames;
		private int Counter = 0;
		private int RenderCallCounter = 0;
		private float RenderTimeStart = 0;

		public int Framerate;
		public int FrameCount;
		public int FrameWidth;
		public int FrameHeight;

		public Point Size { get => new(FrameWidth, FrameHeight); }

		private void IncrementCounter() { Counter++; if (Counter > FrameCount) { Counter = 0; } }

		public void Render(SpriteBatch sb, Vector2 position, Color color)
		{
			RenderCallCounter++;

			if (RenderCallCounter % Framerate == 0)
				IncrementCounter();

			sb.Draw(Frames,
				new Rectangle(position.ToPoint(), Size),
				new Rectangle(new(Counter * FrameWidth, 0), Size), 
				color);
		}

		public AnimatedTexture(Texture2D texture, int cellwidth, int framerate = 1)
		{
			Frames = texture;
			Framerate = framerate;
			FrameHeight = texture.Bounds.Height;
			FrameWidth = cellwidth;
			FrameCount = texture.Bounds.Width / cellwidth;
		}
	}
}
