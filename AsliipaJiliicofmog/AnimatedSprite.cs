using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AsliipaJiliicofmog
{
	class AnimatedSprite
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

		public void Render(SpriteBatch sb, Vector2 offset, GameTime gametime)
		{
			if (gametime.TotalGameTime.TotalSeconds % Framerate == 0)
				CurrentCell = (CurrentCell + 1) % TotalFrames;

			sb.Draw(TextureAtlasStrip, offset,
				new Rectangle(new Point(CurrentCell * CellSize, 0), new Point(CellSize, CellSize)), Color.White);
		}

		public static AnimatedSprite LoadFromFile(string path, GraphicsDevice gd, int framerate = 20, int cellsize = 32)
		{
			var texture = Texture2D.FromStream(gd, new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None));
			return new(texture, framerate, cellsize);
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
