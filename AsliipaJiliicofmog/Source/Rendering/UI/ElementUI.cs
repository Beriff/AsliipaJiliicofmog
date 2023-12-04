using AsliipaJiliicofmog.Event;
using AsliipaJiliicofmog.Input;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace AsliipaJiliicofmog.Rendering.UI
{
	public struct DimUI
	{
		/// <summary>
		/// Position relative to the parent
		/// </summary>
		public Vector2 Position;
		/// <summary>
		/// Additional positional offset (on top of this.Position)
		/// </summary>
		public Vector2 Offset;
		/// <summary>
		/// ADditional size offset (on top of this.Scale)
		/// </summary>
		public Vector2 Size;
		/// <summary>
		/// Size relative to the parent's size ( (1,1) = parent size)
		/// </summary>
		public Vector2 Scale;

		public DimUI(Vector2 pos, Vector2 offset, Vector2 size, Vector2 scale)
		{ Position = pos; Offset = offset; Size = size; Scale = scale; }

		/// <summary>
		/// Construct a DimUI with no local transforms (no offset or size)
		/// </summary>
		public static DimUI Global(Vector2 position, Vector2 scale) =>
			new(position, new(0), new(0), scale);

		/// <summary>
		/// Construct a DimUI with no global transforms (no scale or relative position)
		/// </summary>
		public static DimUI Local(Vector2 offset, Vector2 size) =>
			new(new(0), offset, size, new(0));
	}
	public class ElementUI
	{
		public static EventManager EventsUI;
		public static InputConsumer Input;
		public static Texture2D Texture;
		public static Viewport Viewport;
		public static GraphicsDevice GraphicsDevice;

		public static Vector2 ViewportSize() => Viewport.Bounds.Size.ToVector2();

		public static void Initialize(GraphicsDevice gd)
		{
			Texture = new(gd, 1, 1);

			Texture.SetData([Color.White]);

			EventsUI = new();
			Viewport = gd.Viewport;
			GraphicsDevice = gd;
			Input = InputManager.GetConsumer("UI");
		}

		public DimUI Dimensions;
		public string Name { get; set; }
		public bool Visible { get; set; } = true;
		public bool Active { get; set; } = true;
		public bool Hovered { get; set; }

		public event EventHandler MouseEntered;
		public event EventHandler MouseLeft;

		public virtual ElementUI? Parent { get; set; }
		public Vector2 Pivot { get; set; }
		public Vector2 PivotOffset
		{
			get => AbsoluteSize * Pivot;
		}
		public Rectangle Bounds
		{
			get =>
				new(AbsolutePosition.ToPoint(), AbsoluteSize.ToPoint());
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
				: Parent.AbsoluteSize * Dimensions.Position + Dimensions.Offset + Parent.AbsolutePosition
				) - PivotOffset;
		}

		public virtual void Render(SpriteBatch sb, GroupUI group) { }
		public virtual void Update() 
		{

            if (Bounds.Contains(Input.MousePos()))
			{
				if (!Hovered) MouseEntered?.Invoke(this, null!);
				Hovered = true;
			}
			else
			{
				if (Hovered) MouseLeft?.Invoke(this, null!);
				Hovered = false;
			}
				
		}

		public virtual ElementUI WithParent(ElementUI parent)
		{
			Parent = parent;
			return this;
		}

		public ElementUI(DimUI dims, string name = "ui-element")
		{
			Dimensions = dims;
			Pivot = new(0);
			Name = name;
		}
	}
}
