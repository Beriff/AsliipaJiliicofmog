using AsliipaJiliicofmog.Event;
using AsliipaJiliicofmog.Input;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;

namespace AsliipaJiliicofmog.Rendering.UI
{
	/// <summary>
	/// Represents the basic drawable UI element
	/// </summary>
	internal abstract class UIElement
	{
		protected static Texture2D Texture;
		public static void Initialize(SpriteBatch sb)
		{
			Texture = new Texture2D(sb.GraphicsDevice, 1, 1);
			Texture.SetData(new Color[] { Color.White });
		}
		public static readonly InputConsumer LocalInput = InputManager.GetConsumer("UI");

		protected Vector2 _Position;
		protected Vector2 _Size;
		protected Vector2 _Scale;

		public UIElement? Parent;

		public bool Active = true;
		public bool Visible = true;

		/// <summary>
		/// Get element's position in pixels.
		/// </summary>
		public virtual Vector2 AbsolutePosition { get => Parent == null ? Position : Parent.AbsolutePosition + Position; }

		/// <summary>
		/// Get element's size in pixels
		/// </summary>
		public virtual Vector2 AbsoluteSize { get => Parent == null ? _Size : Parent.AbsoluteSize * _Scale; }

		/// <summary>
		/// Get element's position relative to its parent. If there's none, result is identical to AbsolutePosition
		/// </summary>
		public virtual Vector2 Position { get => _Position; set => _Position = value; }

		/// <summary>
		/// Get element's size in pixels. Able to set new value if there's no parent. If there is, use
		/// <c>Scale</c> property.
		/// </summary>
		/// <exception cref="UIException"></exception>
		public virtual Vector2 Size
		{
			get
			{
				if (Parent == null)
					return _Size;
				return AbsoluteSize;
			}
			set
			{
				if (Parent != null)
					throw new UIException("Cannot change size (parent UIelement present). Did you mean Scale?");
				_Size = value;
			}
		}

		/// <summary>
		/// Get element's scale relative to its parent ({1;1} is equal to parent's size)
		/// </summary>
		public virtual Vector2 Scale { get => _Scale; set => _Scale = value; }

		public virtual Rectangle Bounds { get => new(AbsolutePosition.ToPoint(), AbsoluteSize.ToPoint()); }

		public virtual Rectangle BoundsAt(Vector2 p) => new(p.ToPoint(), AbsoluteSize.ToPoint());

		/// <summary>
		/// Check if mouse cursor hovers over the element
		/// </summary>
		protected bool Hovered() => Bounds.Contains(LocalInput.MousePos());

		public abstract void Render(SpriteBatch sb, UIGroup group);
		public abstract void Update();

		protected UIElement(UIElement? parent, Vector2 pos, Vector2 scale)
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


		public void MakeAppear(EventManager em, Easing.Ease? smoothing = null)
		{
			Easing.Ease ease = smoothing ?? Easing.SmoothStep;
			Visible = true;
			Active = true;

			if (Parent == null)
			{
				em.AddEvent(new(Size.X, 60, GetHashCode().ToString() + "1",
					EventQueueBehavior.Discard, (self) =>
					{
						Size = new(ease(self.Progress) * self.Data, Size.Y);
                    })
					);
				em.AddEvent(new(Size.Y, 60, GetHashCode().ToString() + "2",
					EventQueueBehavior.Discard, (self) =>
					{
						Size = new(Size.X, ease(self.Progress) * self.Data);
					})
					);
				Size = Vector2.Zero;

			}
			else
			{
				em.AddEvent(new(Scale.X, 60, GetHashCode().ToString() + "1",
					EventQueueBehavior.Discard, (self) =>
					{
						Scale = new(ease(self.Progress) * self.Data, Scale.Y);
					})
					);
				em.AddEvent(new(Scale.Y, 60, GetHashCode().ToString() + "2",
					EventQueueBehavior.Discard, (self) =>
					{
						Scale = new(Scale.X, ease(self.Progress) * self.Data);
					})
					);
				Scale = Vector2.Zero;
			}
		}

		public void MakeDisappear(EventManager em, Easing.Ease? smoothing = null) 
		{
			Easing.Ease ease = smoothing ?? Easing.SmoothStep;

			if (Parent == null)
			{
				em.AddEvent(new(Size.X, 60, GetHashCode().ToString() + "1",
					EventQueueBehavior.Discard, (self) =>
					{
						Size = new(ease(1 - self.Progress) * self.Data, Size.Y);
					})
					);
				em.AddEvent(new(Size.Y, 60, GetHashCode().ToString() + "2",
					EventQueueBehavior.Discard, (self) =>
					{
						Size = new(Size.X, ease(1 - self.Progress) * self.Data);
					})
					{ OnEnd = (ge) => { Visible = false; Active = false; } }
					);
				Size = Vector2.Zero;

			}
			else
			{
				em.AddEvent(new(Scale.X, 60, GetHashCode().ToString() + "1",
					EventQueueBehavior.Discard, (self) =>
					{
						Scale = new(ease(1 - self.Progress) * self.Data, Scale.Y);
					})
					);
				em.AddEvent(new(Scale.Y, 60, GetHashCode().ToString() + "2",
					EventQueueBehavior.Discard, (self) =>
					{
						Scale = new(Scale.X, ease(1 - self.Progress) * self.Data);
					})
					{ OnEnd = (ge) => { Visible = false; Active = false; } }
					);
				Scale = Vector2.Zero;
			}
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

		/// <summary>
		/// Executes code after switching to render target, but before rendering children
		/// </summary>
		protected void Render(SpriteBatch sb, UIGroup group, Action a)
		{
			if ((int)AbsoluteSize.X == 0 || (int)AbsoluteSize.Y == 0) { return; }

			RenderTarget ??= new(sb.GraphicsDevice, (int)AbsoluteSize.X, (int)AbsoluteSize.Y);
            Console.WriteLine(RenderTarget.RenderTargetUsage);

            sb.End();
			sb.GraphicsDevice.SetRenderTarget(RenderTarget);
			sb.GraphicsDevice.Clear(Color.Black);
			sb.Begin();
			a();
			foreach (var e in Elements)
			{
				e.Position -= AbsolutePosition;
                if (e.Visible) { e.Render(sb, group); }
				e.Position += AbsolutePosition;
			}
			sb.End();
			sb.GraphicsDevice.SetRenderTarget(null);
			sb.Begin();
			sb.Draw(RenderTarget, AbsolutePosition, new(Point.Zero, AbsoluteSize.ToPoint()), Color.White);
		}
		/// <summary>
		/// Renders the children elements
		/// </summary>
		/// <remarks>Rendering code executed before this function is discarded. Use 3 parameter overload for
		/// alternative behavior.</remarks>
		public override void Render(SpriteBatch sb, UIGroup group)
		{
			RenderTarget ??= new(sb.GraphicsDevice, (int)AbsoluteSize.X, (int)AbsoluteSize.Y);

			sb.End(); 
			sb.GraphicsDevice.SetRenderTarget(RenderTarget);
			sb.GraphicsDevice.Clear(Color.Black);
			sb.Begin();
			foreach (var e in Elements)
			{
				e.Position -= AbsolutePosition;
                if (e.Visible) { e.Render(sb, group); }
				e.Position += AbsolutePosition;
			}
			sb.End();
			sb.GraphicsDevice.SetRenderTarget(null);
			sb.Begin();
            sb.Draw(RenderTarget, AbsolutePosition, new(Point.Zero, AbsoluteSize.ToPoint()), Color.White);
		}
		/// <summary>
		/// Add element as a child, and set their parent automatically
		/// </summary>
		public virtual void AddElement(UIElement element)
		{
			Elements.Add(element);
			element.Parent = this;
		}

		protected UIContainer(UIElement? parent, Vector2 pos, Vector2 scale)
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
