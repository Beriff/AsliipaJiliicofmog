using AsliipaJiliicofmog.Rendering;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Rendering.UI
{
	internal class Image : UIElement
	{
		public IGameTexture Picture;
		public Image(IGameTexture picture, Vector2 pos, Vector2 size)
			: base(pos, size)
		{
			Picture = picture;
		}
		public Image(IGameTexture picture, UIElement? parent, Vector2 pos, Vector2 scale)
			: base(parent, pos, scale)
		{
			Picture = picture;
		}

		public override void RenderAt(SpriteBatch sb, UIGroup group, Vector2 p)
		{
			Picture.Render(sb, p, Color.White);
		}
		public override void UpdateAt(UIGroup group, Vector2 p) { }
	}
}
