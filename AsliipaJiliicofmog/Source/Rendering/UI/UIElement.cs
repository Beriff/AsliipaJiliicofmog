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
		public static readonly EventManager UIEvents = new();

		protected Vector2 _Position;
		protected Vector2 _Size;
		protected Vector2 _Scale;
		protected Vector2 _Pivot;

		protected bool IsHovered = false;

		public UIElement? Parent;

		public bool Active = true;
		public bool Visible = true;

		public string Name;
		public Action<UIGroup, Vector2> OnHoverStart = (_,_) => { };
		public Action<UIGroup, Vector2> OnHoverEnd = (_,_) => { };

		/// <summary>
		/// Get element's position in pixels, including pivot shift
		/// </summary>
		public virtual Vector2 AbsolutePosition { get => Parent == null ? Position : Parent.AbsolutePosition + Position + Pivot; }

		/// <summary>
		/// Get element's size in pixels
		/// </summary>
		public virtual Vector2 AbsoluteSize { get => Parent == null ? _Size : Parent.AbsoluteSize * _Scale; }

		/// <summary>
		/// Get element's position relative to its parent. If there's none, result is identical to AbsolutePosition
		/// (includes pivot shift)
		/// </summary>
		public virtual Vector2 Position { get => _Position + Pivot; set => _Position = value; }

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

		public virtual Vector2 Pivot { get => _Pivot * -Size; set => _Pivot = value; }

		public virtual Rectangle Bounds { get => new(AbsolutePosition.ToPoint(), AbsoluteSize.ToPoint()); }
		public virtual Rectangle BoundsAt(Vector2 pos) => new(pos.ToPoint(), AbsoluteSize.ToPoint());

		/// <summary>
		/// Check if mouse cursor hovers over the element
		/// </summary>
		protected bool Hovered() => Bounds.Contains(LocalInput.MousePos());
		protected bool Hovered(Vector2 pos) => BoundsAt(pos).Contains(LocalInput.MousePos());

		public abstract void RenderAt(SpriteBatch sb, UIGroup group, Vector2 position);
		public void Render(SpriteBatch sb, UIGroup group) => RenderAt(sb, group, AbsolutePosition);

		/// <summary>
		/// Call from child class if you need OnHoverStart and OnHoverEnd functionality
		/// </summary>
		public virtual void UpdateAt(UIGroup group, Vector2 position)
		{
			var new_frame_hover = Hovered(position);

			if (!IsHovered && new_frame_hover)
				OnHoverStart(group, position);
			else if(IsHovered && !new_frame_hover)
				OnHoverEnd(group, position);

			IsHovered = new_frame_hover;
		}
		public void Update(UIGroup group) => UpdateAt(group, AbsolutePosition);

		protected UIElement(UIElement? parent, Vector2 pos, Vector2 scale)
		{
			Parent = parent;
			Position = pos;
			Scale = scale;
			Pivot = Vector2.Zero;
		}

		protected UIElement(Vector2 pos, Vector2 size)
		{
			Parent = null;
			Position = pos;
			Size = size;
			Pivot = Vector2.Zero;
		}


		public void MakeAppear(Easing.Ease? smoothing = null)
		{
			Easing.Ease ease = smoothing ?? Easing.SmoothStep;
			Visible = true;
			Active = true;

			if (Parent == null)
			{
				UIEvents.AddEvent(new(Size.X, 60, GetHashCode().ToString() + "_ax",
					EventQueueBehavior.Discard, (self) =>
					{
						Size = new(ease(self.Progress) * self.Data, Size.Y);
                    })
					);
				UIEvents.AddEvent(new(Size.Y, 60, GetHashCode().ToString() + "_ay",
					EventQueueBehavior.Discard, (self) =>
					{
						Size = new(Size.X, ease(self.Progress) * self.Data);
					})
					);
				Size = Vector2.Zero;

			}
			else
			{
				UIEvents.AddEvent(new(Scale.X, 60, GetHashCode().ToString() + "_ax",
					EventQueueBehavior.Discard, (self) =>
					{
						Scale = new(ease(self.Progress) * self.Data, Scale.Y);
					})
					);
				UIEvents.AddEvent(new(Scale.Y, 60, GetHashCode().ToString() + "_ay",
					EventQueueBehavior.Discard, (self) =>
					{
						Scale = new(Scale.X, ease(self.Progress) * self.Data);
					})
					);
				Scale = Vector2.Zero;
			}
		}

		public void MakeDisappear(Easing.Ease? smoothing = null) 
		{
			Easing.Ease ease = smoothing ?? Easing.SmoothStep;

			if (Parent == null)
			{
				UIEvents.AddEvent(new(Size.X, 60, GetHashCode().ToString() + "_dx",
					EventQueueBehavior.Discard, (self) =>
					{
						Size = new(ease(1 - self.Progress) * self.Data, Size.Y);
					})
					{ OnEnd = (ge) => { Size = new(ge.Data, Size.Y); } }
					);
				UIEvents.AddEvent(new(Size.Y, 60, GetHashCode().ToString() + "_dy",
					EventQueueBehavior.Discard, (self) =>
					{
						Size = new(Size.X, ease(1 - self.Progress) * self.Data);
					})
					{ OnEnd = (ge) => { Visible = false; Active = false; Size = new(Size.X, ge.Data); } }
					);

			}
			else
			{
				UIEvents.AddEvent(new(Scale.X, 60, GetHashCode().ToString() + "_dx",
					EventQueueBehavior.Discard, (self) =>
					{
						Scale = new(ease(1 - self.Progress) * self.Data, Scale.Y);
					})
					{ OnEnd = (ge) => { Size = new(ge.Data, Size.Y); } }
					);
				UIEvents.AddEvent(new(Scale.Y, 60, GetHashCode().ToString() + "_dy",
					EventQueueBehavior.Discard, (self) =>
					{
						Scale = new(Scale.X, ease(1 - self.Progress) * self.Data);
					})
					{ OnEnd = (ge) => { Visible = false; Active = false; Size = new(Size.X, ge.Data); } }
					);
			}
		}

		public Vector2 SizeToScale(Vector2 size)
		{
			return size / AbsoluteSize;
		}

	}
	internal abstract class UIContainer : UIElement
	{
		public List<UIElement> Elements;
		protected RenderTarget2D RenderTarget;

		public UIElement FindElement(string name)
		{
			foreach(var e in Elements)
			{
				if (e.Name == name)
					return e;
			}
			throw new UIException("Element not found");
		}

		public override void UpdateAt(UIGroup group, Vector2 pos)
		{
			foreach (var e in Elements)
			{
				if (e.Active) { e.UpdateAt(group, pos + e.Position); }
			}
		}

		/// <summary>
		/// Executes code after switching to render target, but before rendering children
		/// </summary>
		protected void RenderAt(SpriteBatch sb, UIGroup group, Action a, Vector2 p)
		{
			if ((int)AbsoluteSize.X == 0 || (int)AbsoluteSize.Y == 0) { return; }

			

            sb.End();

			RenderTarget ??= new(sb.GraphicsDevice, (int)AbsoluteSize.X, (int)AbsoluteSize.Y);

			if (RenderTarget.Bounds != new Rectangle(Point.Zero, AbsoluteSize.ToPoint())) 
			{
				RenderTarget.Dispose();
				RenderTarget = new(sb.GraphicsDevice, (int)AbsoluteSize.X, (int)AbsoluteSize.Y);
            }

            sb.GraphicsDevice.SetRenderTarget(RenderTarget);
			sb.GraphicsDevice.Clear(Color.Black);
			sb.Begin(samplerState: SamplerState.PointWrap);
			a();
			foreach (var e in Elements)
			{
                if (e.Visible) { e.RenderAt(sb, group, e.Position); }
            }
			sb.End();
			sb.GraphicsDevice.SetRenderTarget(null);
			sb.Begin();
			sb.Draw(RenderTarget, p, new(Point.Zero, AbsoluteSize.ToPoint()), Color.White);
		}
		/// <summary>
		/// Renders the children elements
		/// </summary>
		/// <remarks>Rendering code executed before this function is discarded. Use 3 parameter overload for
		/// alternative behavior.</remarks>
		public override void RenderAt(SpriteBatch sb, UIGroup group, Vector2 p)
		{
			RenderTarget ??= new(sb.GraphicsDevice, (int)AbsoluteSize.X, (int)AbsoluteSize.Y);

			if (RenderTarget.Bounds != new Rectangle(Point.Zero, AbsoluteSize.ToPoint()))
			{
				RenderTarget.Dispose();
				RenderTarget = new(sb.GraphicsDevice, (int)AbsoluteSize.X, (int)AbsoluteSize.Y);
			}

			sb.End(); 
			sb.GraphicsDevice.SetRenderTarget(RenderTarget);
			sb.GraphicsDevice.Clear(Color.Black);
			sb.Begin();
			foreach (var e in Elements)
			{
                if (e.Visible) { e.RenderAt(sb, group, e.Position); }
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
