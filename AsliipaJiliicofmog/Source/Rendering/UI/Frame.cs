using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Rendering.UI
{
	public class Frame : ContainerUI
	{
		public Color? BaseColor;

		public Frame(DimUI dims, string name = "ui-frame") : base(dims, name) { }

		public override void Render(SpriteBatch sb, GroupUI group)
		{
            SetContainerRenderer(sb);

            sb.Draw(Texture, new Rectangle(Point.Zero, AbsoluteSize.ToPoint()), 
				BaseColor ?? group.Palette.Background);
			RenderChildren(sb, group);

			RetractContainerRenderer(sb);
		}
	}
}
