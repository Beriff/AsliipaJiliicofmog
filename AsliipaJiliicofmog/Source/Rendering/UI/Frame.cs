using System;
using AsliipaJiliicofmog.Event;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Rendering.UI
{
	/// <summary>
	/// A simple UI container that has background color
	/// </summary>
	internal class Frame : UIContainer
	{
		public Frame(UIElement? parent, Vector2 pos, Vector2 scale)
			: base(parent, pos, scale) { }
		public Frame (Vector2 pos, Vector2 size)
			: base(pos, size) { }

		public override void RenderAt(SpriteBatch sb, UIGroup group, Vector2 p)
		{
            base.RenderAt(sb, group, () =>
			{
                sb.Draw(Texture,
				new Rectangle(Point.Zero, AbsoluteSize.ToPoint()),
				group.Palette.Main);
			}, p);
		}

		/// <summary>
		/// Creates a frame with a button to close it
		/// </summary>
		public static Frame Window(string title, SpriteFont font, Vector2 pos, Vector2 size)
		{
            Frame f = new(pos, size);
			var button = new Button(
					null, () => { f.MakeDisappear(); },
					new(0, 0),
					new(20 / size.X, 20 / size.Y)
					)
			{
				Label = "x",
				Name = $"{f.GetHashCode()}_btn",
			};
			var label = new Label(
				null, new(20, 0),
				new((size.X - 20) / size.X, 20 / size.Y)
				)
			{
				Alignment = (TextAlignX.Middle, TextAlignY.Middle)
			};
			label.WithText(title, font);

			f.AddElement(button);
			f.AddElement(label);

			return f;
		}
	}
}
