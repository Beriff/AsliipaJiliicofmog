using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Rendering.UI
{
	internal class VerticalScroll : UIElement
	{
		protected float _Progress = 0;
		protected int InnerOffset;
		public float Progress { get => _Progress; set => _Progress = MathHelper.Clamp(value, 0, 1); }

		protected bool ButtonHeld = false;

		public VerticalScroll(UIElement? parent, Vector2 pos, Vector2 size)
			: base(parent, pos, size) { }
		public VerticalScroll(Vector2 pos, Vector2 scale)
			: base(pos, scale) { }

		protected Rectangle ScrollButtonBounds(Vector2 p)
		{
			var size = new Point((int)AbsoluteSize.X);
			var pos = new Vector2(p.X, p.Y + Progress * (AbsoluteSize.Y - size.Y));
			return new(pos.ToPoint(), size);
		}

		public override void RenderAt(SpriteBatch sb, UIGroup group, Vector2 p)
		{
			sb.Draw(Texture, BoundsAt(p), group.Palette.MainDark);
            sb.Draw(Texture, ScrollButtonBounds(p), group.Palette.Main);
		}

		public override void UpdateAt(UIGroup group, Vector2 pos)
		{
			var bounds = ScrollButtonBounds(pos);
			if (LocalInput.GetM1State() == Input.PressType.Pressed
				&& bounds.Contains(LocalInput.MousePos()) )
			{
                ButtonHeld = true;
				InnerOffset = (int)LocalInput.MousePos().Y - bounds.Location.Y;
			} else if (LocalInput.GetM1State() == Input.PressType.Released)
			{
				ButtonHeld = false;
			}

			if(ButtonHeld)
			{
				var cursor_y = LocalInput.MousePos().Y;
				float p = (cursor_y - pos.Y - InnerOffset) / (AbsoluteSize.Y - AbsoluteSize.X);
				Progress = p;
			}
			

		}
	}
}
