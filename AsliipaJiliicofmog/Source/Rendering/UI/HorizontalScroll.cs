using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Rendering.UI
{
	internal class HorizontalScroll : UIElement
	{
		protected float _Progress = 0;
		protected int InnerOffset;
		public float Progress { get => _Progress; set => _Progress = MathHelper.Clamp(value, 0, 1); }

		protected bool ButtonHeld = false;

		public HorizontalScroll(UIElement? parent, Vector2 pos, Vector2 size)
			: base(parent, pos, size) { }
		public HorizontalScroll(Vector2 pos, Vector2 scale)
			: base(pos, scale) { }

		protected Rectangle ScrollButtonBounds(Vector2 position)
		{
			var size = new Point((int)AbsoluteSize.Y);
			var pos = new Vector2(position.X + Progress * (AbsoluteSize.X - size.X), position.Y);
			return new(pos.ToPoint(), size);
		}

		public override void RenderAt(SpriteBatch sb, UIGroup group, Vector2 p)
		{
			sb.Draw(Texture, BoundsAt(p), group.Palette.MainDark);
			sb.Draw(Texture, ScrollButtonBounds(p), group.Palette.Main);
		}

		public override void UpdateAt(Vector2 pos)
		{
			var bounds = ScrollButtonBounds(pos);
			if (LocalInput.GetM1State() == Input.PressType.Pressed
				&& bounds.Contains(LocalInput.MousePos()))
			{
				ButtonHeld = true;
				InnerOffset = (int)LocalInput.MousePos().X - bounds.Location.X;
			}
			else if (LocalInput.GetM1State() == Input.PressType.Released)
			{
				ButtonHeld = false;
			}

			if (ButtonHeld)
			{
				var cursor_x = LocalInput.MousePos().X;
				float p = (cursor_x - pos.X - InnerOffset) / (AbsoluteSize.X - AbsoluteSize.Y);
				Progress = p;
			}


		}
	}
}
