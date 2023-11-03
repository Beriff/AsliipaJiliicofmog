using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Source.Rendering.UI
{
	internal class HorizontalBar : UIElement
	{
		public override void Render(SpriteBatch sb, UIPalette uip)
		{
            Console.WriteLine(AbsoluteSize);
            sb.Draw(Texture, 
				new Rectangle(AbsolutePosition.ToPoint(), AbsoluteSize.ToPoint()), 
				uip.MainDark);
		}
		public override void Update() { }

		public HorizontalBar(UIElement parent, Vector2 pos, Vector2 scale)
			: base(parent, pos, scale) { }

		public HorizontalBar(Vector2 pos, Vector2 size)
			: base(pos, size) { }
	}
}
