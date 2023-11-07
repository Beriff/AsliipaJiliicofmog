
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

		public override void Render(SpriteBatch sb, UIGroup group)
		{
			ElementOffset = (int)((MaxY - MinY) * Scroll.Progress);

            RenderTarget ??= new(sb.GraphicsDevice, (int)AbsoluteSize.X, (int)AbsoluteSize.Y + (MaxY - MinY));

			sb.End();
			sb.GraphicsDevice.SetRenderTarget(RenderTarget);
			sb.Begin();
			sb.Draw(Texture,
				new Rectangle(AbsolutePosition.ToPoint(), AbsoluteSize.ToPoint()),
				group.Palette.Main);
            foreach (var e in Elements)
			{
                e.Position -= new Vector2(0, ElementOffset);
				if (e.Visible) { e.Render(sb, group); }
				e.Position += new Vector2(0, ElementOffset);
			}
			sb.End();
			sb.GraphicsDevice.SetRenderTarget(null);
			sb.Begin();
			sb.Draw(RenderTarget, AbsolutePosition, new(AbsolutePosition.ToPoint(), AbsoluteSize.ToPoint()), Color.White);
			Scroll.Render(sb, group);
		}
		public override void Update()
		{
			Scroll.Update();
			foreach (var e in Elements)
			{
				e.Position -= new Vector2(0, ElementOffset);
				if(e.Position.Y + e.Size.Y <= 0 || e.Position.Y >= Size.Y) { continue; } 
				if (e.Active) { e.Update(); }
				e.Position += new Vector2(0, ElementOffset);
			}
		}
	}
}
