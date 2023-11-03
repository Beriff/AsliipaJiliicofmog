using AsliipaJiliicofmog.Input;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;

namespace AsliipaJiliicofmog.Source.Rendering.UI
{
	internal abstract class UIElement
	{
		protected static Texture2D Texture;
		public static void Initialize(SpriteBatch sb) 
		{ 
			Texture = new Texture2D(sb.GraphicsDevice, 1, 1);
			Texture.SetData(new Color[] { Color.White });
		}

		protected Vector2 _Position;
		protected Vector2 _Size;
		protected Vector2 _Scale;

		public UIElement? Parent;

		public bool Active = true;
		public bool Visible = true;

		public Vector2 AbsolutePosition { get => Parent == null ? _Position : Parent.AbsolutePosition + _Position; }
		public Vector2 AbsoluteSize { get => Parent == null ? _Size : Parent.AbsoluteSize * _Scale ; }

		public virtual Vector2 Position { get => _Position; set => _Position = value; }
		public virtual Vector2 Size 
		{ 
			get => _Size; 
			set 
			{
				if(Parent != null)
					throw new UIException("Cannot change size (parent UIelement present). Did you mean Scale? ");
				_Size = value;
			} 
		}
		public virtual Vector2 Scale { get => _Scale; set => _Scale = value; }

		public abstract void Render(SpriteBatch sb, UIPalette uip);
		public abstract void Update();

		protected UIElement(UIElement parent, Vector2 pos, Vector2 scale)
		{
			Parent = parent;
			Position = pos;
			Scale = scale;
		}

		protected UIElement(Vector2 pos, Vector2 size)
		{
			Parent = null;
			Position = pos;
			Size = size;
		}
	}
	internal abstract class UIContainer : UIElement
	{
		public List<UIElement> Elements;
		protected RenderTarget2D RenderTarget;

		public override void Update()
		{
			foreach (var e in Elements)
			{
				if(e.Active) { e.Update(); }
			}
		}

		public override void Render(SpriteBatch sb, UIPalette uip)
		{
			sb.End(); 
			sb.GraphicsDevice.SetRenderTarget(RenderTarget);
			sb.Begin();
			foreach (var e in Elements)
			{
				if (e.Visible) { e.Render(sb, uip); }
			}
			sb.End();
			sb.GraphicsDevice.SetRenderTarget(null);
			sb.Begin();
		}

		public void AddElement(UIElement element)
		{
			Elements.Add(element);
			element.Parent = this;
		}

		protected UIContainer(UIElement parent, Vector2 pos, Vector2 scale)
			: base(parent, pos, scale)
		{
			Elements = new();
		}

		protected UIContainer(Vector2 pos, Vector2 size)
			: base(pos, size)
		{
			Elements = new();
		}
	}
}
