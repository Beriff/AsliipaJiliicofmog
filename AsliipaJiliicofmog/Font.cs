using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog
{
	public class Font
	{
		public SpriteAtlas FontTexture;
		readonly public Dictionary<char, Point> CharDict;
		readonly char[] FontCharacters = new char[]
		{
			'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j',
			'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't',
			'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D',
			'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',
			'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
			'Y', 'Z', '!', '@', '#', '$', '%', '^', '&', '*',
			'(', ')', '1', '2', '3', '4', '5', '6', '7', '8',
			'9', '0', '`', '~', ':', ';', '\\', '|', '/', '.',
			',', '\'', '"', '-', '_', '+', '=', '[', ']', '{',
			'}', '☺', '♥', '♦', ' ', '°', '•', '░', '▒', '▓'
		};

		public Font(SpriteAtlas texture)
		{
			FontTexture = texture;
			CharDict = new();
			for(int i = 0; i < FontCharacters.Length; i++)
			{
				char c = FontCharacters[i];
				int x = i % FontTexture.RowSize;
				int y = (int)Math.Floor(i / (float)FontTexture.RowSize);
				CharDict[c] = new Point(x,y);
			}
		}
		public Vector2 MeasureString(string str)
		{
			var res = Vector2.Zero;
			var index_x = 0;
			foreach(char c in str)
			{
				if (c == '\n')
				{
					if (index_x * FontTexture.CellSize > res.X)
						res.X = index_x * FontTexture.CellSize;
					res.Y += FontTexture.CellSize;
				}
				else
					index_x += 1;
			}
			if (index_x * FontTexture.CellSize > res.X)
				res.X = index_x * FontTexture.CellSize;
			return res;
		}
		public void RenderChar(SpriteBatch sb, Vector2 offset, char c, Color color)
		{
			FontTexture.RenderCell(sb, offset, color, CharDict[c]);
		}
	}
	public class TextPiece
	{
		public Color TextColor;
		public int Scale;
		public Font FontRef;
		public string Text;

		public Vector2 Measure()
		{
			return FontRef.MeasureString(Text);
		}

		public void Render(SpriteBatch sb, Vector2 offset)
		{
			Vector2 textpos = Vector2.Zero;
			for(int i = 0; i < Text.Length; i++)
			{
				char c = Text[i];
				if (!FontRef.CharDict.ContainsKey(c))
					c = '▓';
				if(c == '\n')
				{
					textpos.X = 0;
					textpos.Y += 1;
					continue;
				}
				textpos.X += 1;
				FontRef.RenderChar(sb, offset + textpos * FontRef.FontTexture.CellSize, c, TextColor);
			}
		}
		public TextPiece(Font font, string text)
		{
			Text = text;
			FontRef = font;
			Scale = 1;
			TextColor = Color.White;
		}
	}
	public class ChatMessage
	{
		public TextPiece? Author;
		public TextPiece Contents;
		public void Render(SpriteBatch sb, Vector2 offset)
		{
			var cellsize = Author.FontRef.FontTexture.CellSize;
			int combined_length = (int)(Author.Measure().X + cellsize * 5 + Contents.Measure().X);
			int author_length = (int)(Author.Measure().X);

			sb.Draw(GUI.Flatcolor, offset, new Rectangle(Point.Zero, new Point(combined_length, cellsize)), Asliipa.MainGUIColor);
			new TextPiece(Author.FontRef, "[").Render(sb, offset);
			Author?.Render(sb, offset + new Vector2(cellsize, 0));
			new TextPiece(Author.FontRef, "]: ").Render(sb, offset + new Vector2(author_length + cellsize, 0));
			Contents.Render(sb, offset + new Vector2(author_length + cellsize * 4, 0));
		}
		public ChatMessage(string author, string s)
		{
			Author = new TextPiece(Asliipa.MainFont, author);
			Contents = new TextPiece(Asliipa.MainFont, s);
		}
		public ChatMessage(string author, string s, Color msg_color)
		{
			Author = new TextPiece(Asliipa.MainFont, author);
			Contents = new TextPiece(Asliipa.MainFont, s);
			Contents.TextColor = msg_color;
		}
		public ChatMessage(string author, string s, Color msg_color, Color author_color)
		{
			Author = new TextPiece(Asliipa.MainFont, author);
			Contents = new TextPiece(Asliipa.MainFont, s);
			Contents.TextColor = msg_color;
			Author.TextColor = author_color;
		}
	}
	public class Chat
	{
		public List<ChatMessage> Messages;
		public int MaxMessageRender = 5;

		public Chat()
		{
			Messages = new();
		}
		public void Render(SpriteBatch sb, Vector2? pos = null)
		{
			Vector2 position;
			if (pos == null)
			{
				var viewportsize = sb.GraphicsDevice.Viewport.Bounds.Size;
				viewportsize.Y -= 20;
				viewportsize.X = 5;
				position = viewportsize.ToVector2();
			} else { position = (Vector2)pos; }
			for(int i = 0; i < Math.Min(MaxMessageRender, Messages.Count); i++)
			{
				var message = Messages[^(i+1)];
				message.Render(sb, new Vector2(position.X, position.Y - i * 8));
			}
		}
		public void SendMessage(Entity e, string s)
		{
			Messages.Add(new ChatMessage(e.Name, s));
		}
		public void Announce(string s)
		{
			Messages.Add(new ChatMessage("*", s, Color.White, Color.Red));
		}
	}
}
