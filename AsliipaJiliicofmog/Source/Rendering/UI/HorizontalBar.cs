using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Rendering.UI
{
	public class HorizontalBar : UIElement
	{
		public int BarWidth = 6;
		public override void RenderAt(SpriteBatch sb, UIGroup group, Vector2 position)
		{
			var pos = position + new Vector2(0, (AbsoluteSize.Y - BarWidth ) / 2f);
			var size = new Vector2(AbsoluteSize.X, BarWidth);
            sb.Draw(Texture, 
				new Rectangle(pos.ToPoint(), size.ToPoint()), 
				group.Palette.MainDark);
		}
		public override void UpdateAt(UIGroup group, Vector2 pos) { }

		public HorizontalBar(UIElement? parent, Vector2 pos, Vector2 scale)
			: base(parent, pos, scale) { }

		public HorizontalBar(Vector2 pos, Vector2 size)
			: base(pos, size) { }
	}
}
