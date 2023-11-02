using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog.Rendering
{
	internal interface IGameTexture
	{
		public void Render(SpriteBatch sb, GameTime gt, Vector2 position, Color color);
	}

	internal class GameTexture : IGameTexture
	{
		public readonly Texture2D Texture;

		public GameTexture(Texture2D texture)
		{
			Texture = texture;
		}

		public void Render(SpriteBatch sb, GameTime gt, Vector2 position, Color color)
		{
			sb.Draw(Texture, position, color);
		}

		public static implicit operator Texture2D(GameTexture gt) => gt.Texture;
	}

	internal class AnimatedTexture : IGameTexture
	{
		private readonly Texture2D Frames;
		private int Counter = 0;

		public float Framerate;
		public int FrameCount;
		public int FrameWidth;

		public void Render(SpriteBatch sb, GameTime gt, Vector2 position, Color color)
		{

		}
	}
}
