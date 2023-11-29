using AsliipaJiliicofmog.Event;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Diagnostics.CodeAnalysis;

using static System.Formats.Asn1.AsnWriter;

namespace AsliipaJiliicofmog.Rendering.UI
{
	public struct DimUI
	{
		public Vector2 Position;
		public Vector2 Offset;
		public Vector2 Size;
		public Vector2 Scale;

		public DimUI(Vector2 pos, Vector2 offset, Vector2 size, Vector2 scale)
		{ Position = pos; Offset = offset; Size = size; Scale = scale; }
	}
	public class ElementUI
	{
		public static EventManager EventsUI;
		public static Texture2D Texture;
		public static Viewport Viewport;
		public static GraphicsDevice GraphicsDevice;

		public static Vector2 ViewportSize() => Viewport.Bounds.Size.ToVector2();

		public static void Initialize(GraphicsDevice gd)
		{
			Texture = new(gd, 1, 1);
			EventsUI = new();
			Viewport = gd.Viewport;
			GraphicsDevice = gd;
		}

		public DimUI Dimensions { get; set; }

		protected ElementUI? _Parent;
		public virtual ElementUI? Parent 
		{ 
			get => _Parent;
			set { 
				_Parent = value; 
				if (value is ContainerUI container) 
				{
					container.Children.Add(value);
				} 
			}
		}

		public Vector2 Pivot { get; set; }
		public Vector2 PivotOffset
		{
			get =>
				Bounds * Pivot;
		}

		public Vector2 Bounds
		{
			get =>
			Parent == null ?
				Dimensions.Size + ViewportSize() * Dimensions.Scale
				: Parent.AbsoluteSize * Dimensions.Scale + Dimensions.Size;
		}

		public Rectangle BoundsRect
		{
			get =>
				new(Point.Zero, Bounds.ToPoint());
		}

		public Vector2 AbsoluteSize 
		{
			get =>
				Parent == null ?
				Dimensions.Size + ViewportSize() * Dimensions.Scale
				: Parent.AbsoluteSize * Dimensions.Scale + Dimensions.Size;
				
		}

		public Vector2 AbsolutePosition
		{
			get => (
				Parent == null ?
				Dimensions.Offset + Dimensions.Position * ViewportSize()
				: Parent.AbsoluteSize * Dimensions.Position + Dimensions.Offset
				) + PivotOffset;
		}

		public virtual void Render(SpriteBatch sb) { }
		public virtual void Update() { }

		public virtual ElementUI WithParent(ElementUI parent)
		{
			Parent = parent;
			return this;
		}

		public ElementUI(DimUI dims)
		{
			Dimensions = dims;
			Pivot = new(0);
		}
	}
}
