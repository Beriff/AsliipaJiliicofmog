using AsliipaJiliicofmog.Data;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsliipaJiliicofmog.Rendering.UI
{
	public enum AlignmentX
	{
		Left,
		Center,
		Right
	}
	public enum AlignmentY 
	{ 
		Top, 
		Center,
		Bottom,
	}
	public class Label : ElementUI
	{
		public (AlignmentX X, AlignmentY Y) Alignment;
		public string Text;
		public Label(string text, DimUI dims, string name = "ui-label",
			AlignmentX ax = AlignmentX.Center, AlignmentY ay = AlignmentY.Center) 
			: base(dims, name)
		{
			Text = text;
			Alignment = (ax, ay);
		}

		public override void Render(SpriteBatch sb, GroupUI group)
		{
			Vector2 textsize = Registry.DefaultFont.MeasureString(Text);
			float posx = Alignment.X switch
			{
				AlignmentX.Left => AbsolutePosition.X,
				AlignmentX.Center => Bounds.Center.X - textsize.X / 2,
				AlignmentX.Right => AbsolutePosition.X + AbsoluteSize.X - textsize.X
			};
			float posy = Alignment.Y switch
			{
				AlignmentY.Top => AbsolutePosition.Y,
				AlignmentY.Center => Bounds.Center.Y - textsize.Y / 2,
				AlignmentY.Bottom => AbsolutePosition.Y + AbsoluteSize.Y - textsize.Y
			};
			sb.DrawString(Registry.DefaultFont, Text, new(posx, posy), group.Palette.Text);
		}
	}
}
