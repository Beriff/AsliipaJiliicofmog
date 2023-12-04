using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Rendering.UI
{
	public class ListLayout : ContainerUI
	{
		protected int _Spacing;
		public int Spacing 
		{
			get => _Spacing;
			set
			{
				var diff = value - _Spacing;
				foreach (var e in Children) e.Dimensions.Offset.Y += diff;
				_Spacing = value;
			} 
		}
		public ListLayout(DimUI dims, int spacing = 5, string name = "ui-listlayout") : base(dims, name) 
		{
			Spacing = spacing;
		}
		public override void Add(ElementUI element)
		{
			int verticaloffset = 0;
			foreach(var e in Children) verticaloffset += (int)e.AbsoluteSize.Y + Spacing;
			Children.Add(element);
			element.Dimensions.Offset.Y = verticaloffset;
			element.Parent = this;
		}

		public override void Remove(ElementUI element)
		{
			int index = Children.IndexOf(element);
			int occupied_size = (int)element.AbsoluteSize.Y + Spacing;
			if (index != -1)
			{
				Children.RemoveAt(index);
				for(int i = index;  i < Children.Count; i++)
				{
					Children[i].Dimensions.Offset.Y -= occupied_size;
				}
			}
		}

		public override void Render(SpriteBatch sb, GroupUI group)
		{
			foreach(var e in Children)
			{
				if(!e.Visible) continue;
				e.Render(sb, group);
			}
		}

		public override void Update()
		{
			foreach (var e in Children)
			{
				if (!e.Active) continue;
				e.Update();
			}
		}
	}
}
