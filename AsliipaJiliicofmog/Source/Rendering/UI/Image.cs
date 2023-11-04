using AsliipaJiliicofmog.Rendering;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Source.Rendering.UI
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

		public override void Render(SpriteBatch sb, UIGroup group)
		{
			Picture.Render(sb, AbsolutePosition, Color.White);
		}
		public override void Update() { }
	}
}
