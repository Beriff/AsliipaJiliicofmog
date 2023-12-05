using AsliipaJiliicofmog.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsliipaJiliicofmog.Rendering.UI
{
	public class HorizontalScrollbar : ElementUI
	{

		public float Progress;
		public bool ButtonHovered = false;
		public bool Held = false;

		protected int ClickOffset;
		public HorizontalScrollbar(DimUI dims, string name = "ui-scrollbar") : base(dims, name)
		{

		}

		protected Rectangle ScrollButtonBounds()
		{
			int width = (int)(AbsoluteSize.X - AbsoluteSize.Y);
			return new(
				new(
					(int)(AbsolutePosition.X + width * Progress) - ClickOffset,
					(int)AbsolutePosition.Y
					),
				new((int)AbsoluteSize.Y, (int)AbsoluteSize.Y)
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
				ClickOffset = (int)(mpos.X - b_bounds.Location.X);
			}

			else if (Input.GetM1State() == PressType.Released)
			{
				Held = false;
			}


			if (Held)
			{
				int width = (int)(AbsoluteSize.X - AbsoluteSize.Y);
				float clamped_mpos = MathHelper.Clamp(
					Input.MousePos().X,
					AbsolutePosition.X + ClickOffset,
					AbsolutePosition.X + width + ClickOffset
					);
				Progress = (clamped_mpos - AbsolutePosition.X) / width;
			}

			base.Update();
		}

	}
}
