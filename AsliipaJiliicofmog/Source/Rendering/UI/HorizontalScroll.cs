using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Source.Rendering.UI
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

		protected Rectangle ScrollButtonBounds()
		{
			var size = new Point((int)AbsoluteSize.Y);
			var pos = new Vector2(AbsolutePosition.X + Progress * (AbsoluteSize.X - size.X), AbsolutePosition.Y);
			return new(pos.ToPoint(), size);
		}

		public override void Render(SpriteBatch sb, UIGroup group)
		{
			sb.Draw(Texture, Bounds, group.Palette.MainDark);
			sb.Draw(Texture, ScrollButtonBounds(), group.Palette.Main);
		}

		public override void Update()
		{
			var bounds = ScrollButtonBounds();
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
				float p = (cursor_x - AbsolutePosition.X - InnerOffset) / (AbsoluteSize.X - AbsoluteSize.Y);
				Progress = p;
			}


		}
	}
}
