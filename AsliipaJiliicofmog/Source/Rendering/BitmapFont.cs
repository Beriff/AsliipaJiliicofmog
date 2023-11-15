using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Rendering
{
	/// <summary>
	/// Represents monospaced font constructed from a texture. Supports scaling and color codes.
	/// </summary>
	public class BitmapFont
	{
		public Texture2D Bitmap;
		public int CellWidth;
		public int CellHeight;
		public Vector2 CharSize { get => new(CellWidth, CellHeight); }

		private static readonly char[] CharMap = new char[]
		{
			'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k',
			'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
			'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G',
			'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R',
			'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '1', '2', '3',
			'4', '5', '6', '7', '8', '9', '-', '=', '!', '@', '#',
			'$', '%', '&', '*', '(', ')', '_', '+', '~', '`',
			'[', ']', '{', '}', ':', ';', '\'', '"', '\\', '|', ',',
			'.', '/', '<', '>', '?', '♥', '♣', '♠', '•', '○', '◙',
			'♪', '☼', '►', '◄', '¶', '§', '▲', '▼', '░', '▒', '▓',
			' '
		};

		private static readonly Dictionary<char, int> CharIndices = new();

		static BitmapFont()
		{
			for (int i = 0; i < CharMap.Length; i++) { CharIndices[CharMap[i]] = i; }
		}

		public void RenderChar(SpriteBatch sb, float scale, char c, Vector2 position, Color color)
		{
			sb.Draw(Bitmap, position,
				new Rectangle(new(CharIndices[c] * CellWidth, 0), CharSize.ToPoint()), color,
				0f, Vector2.Zero, scale, SpriteEffects.None, 0);
		}
		/// <summary>
		/// Renders a string, provided all the characters are renderable. Doesn't support color codes
		/// (use RenderColorcoded instead)
		/// </summary>
		public void RenderString(SpriteBatch sb, float scale, string s, Vector2 position)
		{
			Vector2 charpos = position;
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] == '\n') { charpos.Y += CellHeight * scale; charpos.X = position.X; continue; }
				else { charpos.X += CellHeight * scale; }
				RenderChar(sb, scale, s[i], charpos, Color.White);
			}
		}
		/// <summary>
		/// Renders a string and colors it depending on the colorcodes
		/// Ex. ^255000000 is red, ^255000255 is magenta
		/// </summary>
		public void RenderColorcoded(SpriteBatch sb, float scale, string s, Vector2 position)
		{
			Vector2 charpos = position;
			Color c = Color.White;
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] == '\n') { charpos.Y += CellHeight * scale; charpos.X = position.X; continue; }
				else if (s[i] == '^')
				{
					i++;
					int n1 = int.Parse(new(new char[] { s[i], s[i + 1], s[i + 2] }));
					int n2 = int.Parse(new(new char[] { s[i + 3], s[i + 4], s[i + 5] }));
					int n3 = int.Parse(new(new char[] { s[i + 6], s[i + 7], s[i + 8] }));
					i += 8;
					c = new(n1, n2, n3);
					continue;
				}
				else { charpos.X += CellHeight * scale; }
				RenderChar(sb, scale, s[i], charpos, c);
			}
		}

		public BitmapFont(Texture2D texture)
		{
			Bitmap = texture;
			CellWidth = texture.Width / CharMap.Length;
			CellHeight = texture.Height;
		}
	}
}
