using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AsliipaJiliicofmog
{
	public interface ISprite
	{
		public void Render(SpriteBatch sb, Vector2 offset, GameTime gametime, Color color);
		public int Width { get; }
		public int Height { get; }
		public Vector2 TextureDims { get => new(Width, Height); }
		public Texture2D Default { get; }
		public Texture2D FullTexture { get; }
	}
	class Sprite : ISprite
	{
		public Texture2D Texture;
		public Sprite(Texture2D texture)
		{
			Texture = texture;
		}
		public void Render(SpriteBatch sb, Vector2 offset, GameTime gametime, Color color)
		{
			sb.Draw(Texture, offset, color);
		}
		public int Width { get => Texture.Width; }
		public int Height { get => Texture.Height; }
		public Texture2D Default { get => Texture; }
		public Texture2D FullTexture { get => Default; }

	}
	class AnimatedSprite : ISprite
	{
		public readonly Texture2D TextureAtlasStrip;
		public int CellSize;
		public int Framerate; //animation frame is updated each Nth frame, where N is this variable
		public bool Loop = true;
		public readonly int TotalFrames;

		public int CurrentCell;

		public AnimatedSprite(Texture2D texture, int framerate = 20, int cellsize = 32)
		{
			TextureAtlasStrip = texture;
			Framerate = framerate;
			CellSize = cellsize;
			TotalFrames = texture.Width / cellsize;
			CurrentCell = 0;
		}

		public void Render(SpriteBatch sb, Vector2 offset, GameTime gametime, Color color)
		{
			if (gametime.TotalGameTime.TotalSeconds % Framerate == 0)
				CurrentCell = (CurrentCell + 1) % TotalFrames;

			sb.Draw(TextureAtlasStrip, offset,
				new Rectangle(new Point(CurrentCell * CellSize, 0), new Point(CellSize, CellSize)), color);
		}

		public static AnimatedSprite LoadFromFile(string path, GraphicsDevice gd, int framerate = 20, int cellsize = 32)
		{
			var texture = Texture2D.FromStream(gd, new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None));
			return new(texture, framerate, cellsize);
		}

		public int Width { get => CellSize; }
		public int Height { get => TextureAtlasStrip.Height; }
		public Texture2D Default { get => TextureAtlasStrip.Subtexture(Width, Height); }
		public Texture2D FullTexture { get => TextureAtlasStrip; }
	}
	class CombinedSprite : ISprite
	{
		public ISprite[] Textures;
		public Vector2[] TextureOffsets;

		public void Render(SpriteBatch sb, Vector2 offset, GameTime gametime, Color color)
		{
			for (int i = 0; i < Textures.Length; i++)
				Textures[i].Render(sb, offset + TextureOffsets[i], gametime, color);
		}

		public int Width { get => Textures[0].Width; }
		public int Height { get => Textures[0].Height; }
		public Texture2D Default { get => Textures[0].Default; }
		public Texture2D FullTexture { get => Textures[0].FullTexture; }

		public CombinedSprite(params ISprite[] sprites)
		{
			Textures = sprites;
		}

	}
	public class SpriteAtlas : ISprite
	{
		public Texture2D Atlas;
		public int CellSize;
		public int RowSize;
		public void Render(SpriteBatch sb, Vector2 offset, GameTime gametime, Color color)
		{
			sb.Draw(Atlas, offset, color);
		}
		public int Width { get => Atlas.Width; }
		public int Height { get => Atlas.Height; }
		public Texture2D Default { get => Atlas; }
		public Texture2D FullTexture { get => Atlas; }
		public void RenderCell(SpriteBatch sb, Vector2 offset, Color color, Point coords)
		{
			sb.Draw(Atlas, offset, new Rectangle(coords.Mult(CellSize), new Point(CellSize)), color);
		}
		public SpriteAtlas(Texture2D atlas, int cellsize = 8, int rowsize = 10)
		{
			Atlas = atlas;
			CellSize = cellsize;
			RowSize = rowsize;
		}
	}

	class StateMachine
	{
		public Dictionary<string, AnimatedSprite> States;
		public string CurrentState;

		public StateMachine(List<KeyValuePair<string, AnimatedSprite>> states)
		{
			States = new();
			CurrentState = states[0].Key;
			AddStates(states);
		}

		public void AddStates(List<KeyValuePair<string, AnimatedSprite>> states)
		{
			foreach(var state in states)
			{
				States[state.Key] = state.Value;
			}
		}

		public void AddState(string statename, AnimatedSprite sprite)
		{
			States[statename] = sprite;
		}

		public void ChangeState(string newstate)
		{
			if(States.ContainsKey(newstate))
			{
				GetCurrentSprite().CurrentCell = 0;
				CurrentState = newstate;
			} else
			{
				throw new Exception($"Given state '{newstate}' is not present in StateMachine.");
			}
		}

		public AnimatedSprite GetCurrentSprite()
		{
			return States[CurrentState];
		}
	}
}
