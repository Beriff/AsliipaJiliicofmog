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

		public static Frame Window(string name, DimUI dims)
		{
			var f = new Frame(dims, name);
			var closebtn = new Button(
				() => { f.Active = false; f.Visible = false; },
				"X", new DimUI(new(1, 0), new(0), new(15), new(0)),
				name + "-clsbtn"
				) { Pivot = new(1, 0) };
			var title = new Label(
				name, new(new(0), new(0), new(0, 15), new(1, 0)), 
				ax: AlignmentX.Left);

			f.Add(closebtn);
			f.Add(title);
			return f;
		}
	}
}
