using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Source.Rendering.UI
{
	internal class HorizontalBar : UIElement
	{
		public int BarWidth = 6;
		public override void Render(SpriteBatch sb, UIPalette uip)
		{
			var pos = AbsolutePosition + new Vector2(0, (AbsoluteSize.Y - BarWidth ) / 2f);
			var size = new Vector2(AbsoluteSize.X, BarWidth);
            sb.Draw(Texture, 
				new Rectangle(pos.ToPoint(), size.ToPoint()), 
				uip.MainDark);
		}
		public override void Update() { }

		public HorizontalBar(UIElement? parent, Vector2 pos, Vector2 scale)
			: base(parent, pos, scale) { }

		public HorizontalBar(Vector2 pos, Vector2 size)
			: base(pos, size) { }
	}
}
