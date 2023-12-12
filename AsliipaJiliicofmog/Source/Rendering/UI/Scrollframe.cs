using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Rendering.UI
{
	public class Scrollframe : ContainerUI
	{
		public VerticalScrollbar Scrollbar;
		public int Scroll;
		
		protected int HeightTop;
		protected int HeightBottom;
		protected int ElementSpan {  get => HeightBottom - HeightTop; }

		protected void RecalculateSpan()
		{
			float min = 10000;
			float max = -1;

			foreach (var e in Children)
			{
				min = MathF.Min(min, e.AbsolutePosition.Y);
				max = MathF.Max(max, e.AbsolutePosition.Y);
			}

			HeightTop = (int)min;
			HeightBottom = (int)max;
		}

		public Scrollframe(DimUI dims, string name = "ui-scrollframe") : base(dims, name) 
		{
			Scrollbar = new(new(new(0), new(0), new(10, 0), new(0, 1)), name + "-scrollbar");
			//don't add it to children, so it doesn't scroll with the rest of contents
			Scrollbar.Parent = this; 

		}

		public override void Add(ElementUI element)
		{
			base.Add(element);
			RecalculateSpan();
		}
		public override void Remove(ElementUI element)
		{
			base.Remove(element);
			RecalculateSpan();
		}

		public override void Update()
		{
			if(Hovered)
			{
                Scrollbar.Progress -= Input.GetScrollDelta() / 1200.0f;
			}
			Scroll = (int)(Scrollbar.Progress * ElementSpan);
			Scrollbar.Update();
			
			base.Update();
		}

		public override void Render(SpriteBatch sb, GroupUI group)
		{
			SetContainerRenderer(sb);

			sb.Draw(Texture, new Rectangle(Point.Zero, AbsoluteSize.ToPoint()),
				group.Palette.Background);
			RenderChildren(sb, group);

			RetractContainerRenderer(sb);
			Scrollbar.Render(sb, group);
		}

		protected override void RenderChildren(SpriteBatch sb, GroupUI group)
		{
			foreach (var child in Children)
			{
				if (!child.Visible) continue;
				child.Dimensions.Offset -= AbsolutePosition;
				child.Dimensions.Offset.Y -= Scroll;
				child.Render(sb, group);
				child.Dimensions.Offset.Y += Scroll;
				child.Dimensions.Offset += AbsolutePosition;
			}
		}
	}
}
