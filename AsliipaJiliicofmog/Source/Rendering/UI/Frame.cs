using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Source.Rendering.UI
{
	/// <summary>
	/// A simple UI container that has background color
	/// </summary>
	internal class Frame : UIContainer
	{
		public Frame(UIElement? parent, Vector2 pos, Vector2 scale)
			: base(parent, pos, scale) { }
		public Frame (Vector2 pos, Vector2 size)
			: base(pos, size) { }

		public override void Render(SpriteBatch sb, UIPalette uip)
		{
			base.Render(sb, uip, () =>
			{
				sb.Draw(Texture,
				new Rectangle(AbsolutePosition.ToPoint(), AbsoluteSize.ToPoint()),
				uip.Main);
			});
		}
	}
}
