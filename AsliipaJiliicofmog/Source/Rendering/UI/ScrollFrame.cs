
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog.Rendering.UI
{
	internal class ScrollFrame : Frame
	{

		public VerticalScroll Scroll;
		protected float ElementOffset;

		public ScrollFrame(UIElement? parent, Vector2 pos, Vector2 scale) 
			: base(parent, pos, scale) 
		{
			Scroll = new(this, new(-10, 0), new(10f / AbsoluteSize.X, 1));
		}
		public ScrollFrame(Vector2 pos, Vector2 size)
			: base(pos, size) 
		{
			Scroll = new(this, new(-10, 0), new(10f / AbsoluteSize.X, 1));
		}

		protected int MinY
		{
			get
			{
				int min = 0;
				foreach(var e in Elements) { min = (int)MathF.Min(min, e.Position.Y); }
				return min;
			}
		}

		protected int MaxY
		{
			get
			{
				int max = -100 * 100;
				foreach (var e in Elements) { max = (int)MathF.Max(max, e.Position.Y); }
				return max;
			}
		}

		public override void RenderAt(SpriteBatch sb, UIGroup group, Vector2 p)
		{
			ElementOffset = (int)((MaxY - MinY) * Scroll.Progress);

			RenderTarget ??= new(sb.GraphicsDevice, (int)AbsoluteSize.X, (int)AbsoluteSize.Y + (MaxY - MinY));

			if (RenderTarget.Bounds != new Rectangle(Point.Zero, AbsoluteSize.ToPoint()))
			{
				RenderTarget.Dispose();
				RenderTarget = new(sb.GraphicsDevice, (int)AbsoluteSize.X, (int)AbsoluteSize.Y);
			}

			sb.End();
			sb.GraphicsDevice.SetRenderTarget(RenderTarget);
			sb.Begin();
			sb.Draw(Texture,
				new Rectangle(p.ToPoint(), AbsoluteSize.ToPoint()),
				group.Palette.Main);
            foreach (var e in Elements)
			{
				if (e.Visible) { e.RenderAt(sb, group, e.Position - new Vector2(0, ElementOffset)); }
			}
			sb.End();
			sb.GraphicsDevice.SetRenderTarget(null);
			sb.Begin();
			sb.Draw(RenderTarget, p, new(p.ToPoint(), AbsoluteSize.ToPoint()), Color.White);
			Scroll.RenderAt(sb, group, p);
		}
		public override void UpdateAt(UIGroup group, Vector2 p)
		{
			p -= AbsolutePosition;
			Scroll.UpdateAt(group, Scroll.AbsolutePosition + p);
			foreach (var e in Elements)
			{
				if(e.Position.Y + e.Size.Y <= 0 || e.Position.Y >= Size.Y) { continue; } 
				if (e.Active) { e.UpdateAt(group, e.AbsolutePosition - new Vector2(0, ElementOffset)); }
			}
		}
	}
}
