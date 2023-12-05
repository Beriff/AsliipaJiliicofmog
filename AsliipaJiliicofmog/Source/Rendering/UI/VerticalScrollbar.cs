
using AsliipaJiliicofmog.Input;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Rendering.UI
{
	public class VerticalScrollbar : ElementUI
	{
		public float Progress;
		public bool ButtonHovered = false;
		public bool Held = false;

		protected int ClickOffset;
		public VerticalScrollbar(DimUI dims, string name = "ui-scrollbar") : base(dims, name) 
		{ 

		}

		protected Rectangle ScrollButtonBounds()
		{
			int height = (int)(AbsoluteSize.Y - AbsoluteSize.X);
			return new(
				new(
					(int)AbsolutePosition.X, 
					(int)(AbsolutePosition.Y + height * Progress) + ClickOffset
					),
				new((int)AbsoluteSize.X, (int)AbsoluteSize.X)
				);
        }

		public override void Render(SpriteBatch sb, GroupUI group)
		{
            sb.Draw(Texture, Bounds, group.Palette.BackgroundDark);
            sb.Draw(Texture, ScrollButtonBounds(),
				ButtonHovered ? group.Palette.Foreground : group.Palette.ForegroundDark
				);
		}

		public override void Update()
		{
			var b_bounds = ScrollButtonBounds();
			var mpos = Input.MousePos();

			if (b_bounds.Contains(mpos))
				ButtonHovered = true;
			else
				ButtonHovered = false;

			if (ButtonHovered && Input.GetM1State() == PressType.Pressed)
			{
				Held = true;
				ClickOffset = (int)(b_bounds.Location.Y - mpos.Y);
			}
				
			else if (Input.GetM1State() == PressType.Released)
			{
				Held = false;
			}
				

			if(Held)
			{
				int height = (int)(AbsoluteSize.Y - AbsoluteSize.X);
				float clamped_mpos = MathHelper.Clamp(
					Input.MousePos().Y,
					AbsolutePosition.Y - ClickOffset,
					AbsolutePosition.Y + height - ClickOffset
					);
				Progress = (clamped_mpos - AbsolutePosition.Y) / height;
			}

			base.Update();
		}
	}
}
