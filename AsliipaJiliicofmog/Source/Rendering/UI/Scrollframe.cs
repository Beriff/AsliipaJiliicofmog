using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsliipaJiliicofmog.Rendering.UI
{
	public class Scrollframe : ContainerUI
	{
		public VerticalScrollbar Scrollbar;
		public int Scroll;

		protected int HeightTop;
		protected int HeightBottom;
		protected ElementUI Holder;
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

		public Scrollframe(DimUI dims, string name) : base(dims, name) 
		{
			Scrollbar = new(new(new(0), new(0), new(10, 0), new(0, 1)), name + "-scrollbar");
			Children.Add(Scrollbar);
			Holder = new ElementUI(DimUI.Full, "scrollframe-holder");
			Children.Add(Holder);
		}

		public override void Add(ElementUI element)
		{
			element.Parent = Holder;
			RecalculateSpan();
		}
		public override void Remove(ElementUI element)
		{
			element.Parent = Holder;
			RecalculateSpan();
		}

		public override void Update()
		{
			if(Hovered)
			{
				Scrollbar.Progress += Input.GetScrollDelta() / 100.0f;
			}
			Holder.Dimensions.Offset.Y = -ElementSpan * Scrollbar.Progress;
			
			base.Update();
		}
	}
}
