using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Rendering.UI
{
	public enum TextAlignX
	{
		Right,
		Middle,
		Left
	}
	public enum TextAlignY
	{
		Top,
		Middle,
		Bottom
	}
	public class Label : UIElement
	{
		public (TextAlignX X, TextAlignY Y) Alignment = (TextAlignX.Middle, TextAlignY.Middle);

		protected Vector2 TextSize;

		public string _Text;
		public string Text { get => _Text; }
		public Color Color = Color.White;

		public Label WithText(string text, SpriteFont font)
		{
			TextSize = font.MeasureString(text);
			_Text = text;
			return this;
		}

		public Label(UIElement? parent, Vector2 pos, Vector2 scale)
			: base(parent, pos, scale) { }

		public Label(Vector2 pos, Vector2 size)
			: base(pos, size) { }

		public override void RenderAt(SpriteBatch sb, UIGroup group, Vector2 p)
		{
			var textpos = Vector2.Zero;
			switch(Alignment.X)
			{
				case TextAlignX.Left:
					textpos.X = p.X; break;
				case TextAlignX.Middle:
					textpos.X = p.X + AbsoluteSize.X / 2 - TextSize.X / 2; break;
				case TextAlignX.Right:
					textpos.X = p.X + AbsoluteSize.X - TextSize.X; break;
			}
			switch (Alignment.Y)
			{
				case TextAlignY.Top:
					textpos.Y = p.Y; break;
				case TextAlignY.Middle:
					textpos.Y= p.Y + AbsoluteSize.Y / 2 - TextSize.Y / 2; break;
				case TextAlignY.Bottom:
					textpos.Y = p.Y + AbsoluteSize.Y - TextSize.Y; break;
			}
            sb.DrawString(group.Font, Text, textpos, Color);
		}

		public override void UpdateAt(UIGroup group, Vector2 position) { }
	}
}
