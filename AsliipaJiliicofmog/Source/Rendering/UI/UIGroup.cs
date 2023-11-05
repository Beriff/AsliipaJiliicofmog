using AsliipaJiliicofmog.Input;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;

namespace AsliipaJiliicofmog.Rendering.UI
{
	enum UIGroupQueueType
	{
		Override,
		Discard
	}
	public class UIException : Exception
	{
		public UIException() { }
		public UIException(string message) : base(message) { }
		public UIException(string message, Exception innerException) : base(message, innerException) { }
	}
	internal class UIPalette
	{
		public Color Main;
		public Color MainDark;

		public Color Highlight;
		public Color HighlightDark;

		public Color Readable;
		public Color ReadableDark;

		public Color Interactable;
		public Color InteractableDark;

		public Color Contour;
		public Color ContourDark;

		public UIPalette(Color main, Color mainDark, Color highlight, Color highlightDark, Color readable,
			Color readableDark, Color inter, Color interDark, Color contour)
		{
			Main = main;
			MainDark = mainDark;
			Highlight = highlight;
			HighlightDark = highlightDark;
			Readable = readable;
			ReadableDark = readableDark;
			Interactable = inter;
			InteractableDark = interDark;
			Contour = contour;
		}

		public static UIPalette Default => new(
			new(76, 76, 76), new(56, 56, 56),
			new(255, 106, 0), new(183, 73, 0),
			Color.White, Color.LightGray,
			new(158, 158, 158), new(114, 114, 114),
			new(122, 122, 122)
			);
			
	}
	internal class UIGroup
	{
		public readonly List<UIElement> Elements;

		public UIGroupQueueType QueueType;
		public string Name;
		public UIPalette Palette;
		public SpriteFont Font;

		public void SetVisible(bool v)
		{
			foreach (var e in Elements) { e.Visible = v; }
		}

		public void SetActive(bool v)
		{
			foreach (var e in Elements) { e.Active = v; }
		}

		public UIGroup(string name, SpriteFont font, UIGroupQueueType queueType = UIGroupQueueType.Discard)
		{
			Elements = new();
			Name = name;
			QueueType = queueType;
			Palette = UIPalette.Default;
			Font = font;
		}

		public UIElement Add(UIElement e)
		{
			Elements.Add(e);
			return e;
		}

		public void Render(SpriteBatch sb)
		{
			sb.Begin();
			foreach (var e in Elements)
			{
				if (e.Visible)
					e.Render(sb, this);
			}
			sb.End();
		}

		public void Update()
		{
			foreach (var e in Elements)
			{
				if (e.Active)
					e.Update();
			}
		}
	}
	
}
