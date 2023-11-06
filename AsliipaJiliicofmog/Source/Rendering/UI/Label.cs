using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Rendering.UI
{
	enum TextAlignX
	{
		Right,
		Middle,
		Left
	}
	enum TextAlignY
	{
		Top,
		Middle,
		Bottom
	}
	internal class Label : UIElement
	{
		public (TextAlignX X, TextAlignY Y) Alignment = (TextAlignX.Middle, TextAlignY.Middle);

		protected Vector2 TextSize;

		public string _Text;
		public string Text { get => _Text; }
		public Color Color = Color.White;

		public Label WithText(string text, UIGroup group)
		{
			TextSize = group.Font.MeasureString(text);
			_Text = text;
			return this;
		}

		public Label(UIElement? parent, Vector2 pos, Vector2 scale)
			: base(parent, pos, scale) { }

		public Label(Vector2 pos, Vector2 size)
			: base(pos, size) { }

		public override void Render(SpriteBatch sb, UIGroup group)
		{
			var textpos = Vector2.Zero;
			switch(Alignment.X)
			{
				case TextAlignX.Left:
					textpos.X = AbsolutePosition.X; break;
				case TextAlignX.Middle:
					textpos.X = AbsolutePosition.X + AbsoluteSize.X / 2 - TextSize.X / 2; break;
				case TextAlignX.Right:
					textpos.X = AbsolutePosition.X + AbsoluteSize.X - TextSize.X; break;
			}
			switch (Alignment.Y)
			{
				case TextAlignY.Top:
					textpos.Y = AbsolutePosition.Y; break;
				case TextAlignY.Middle:
					textpos.Y= AbsolutePosition.Y + AbsoluteSize.Y / 2 - TextSize.Y / 2; break;
				case TextAlignY.Bottom:
					textpos.Y = AbsolutePosition.Y + AbsoluteSize.Y - TextSize.Y; break;
			}
            sb.DrawString(group.Font, Text, textpos, Color);
		}

		public override void Update() { }
	}
}
