using AsliipaJiliicofmog.Input;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;

namespace AsliipaJiliicofmog.Source.Rendering.UI
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
		public Color Accent;

		public UIPalette(Color main, Color mainDark, Color highlight, Color highlightDark, Color accent)
		{
			Main = main;
			MainDark = mainDark;
			Highlight = highlight;
			HighlightDark = highlightDark;
			Accent = accent;
		}

		public static UIPalette Default => new(Color.Gray, Color.DarkGray, Color.Orange, Color.DarkOrange, Color.LightGoldenrodYellow);
	}
	internal class UIGroup
	{
		public readonly List<UIElement> Elements;

		public UIGroupQueueType QueueType;
		public string Name;
		public UIPalette Palette;

		public void SetVisible(bool v)
		{
			foreach (var e in Elements) { e.Visible = v; }
		}

		public void SetActive(bool v)
		{
			foreach (var e in Elements) { e.Active = v; }
		}

		public UIGroup(string name, UIGroupQueueType queueType = UIGroupQueueType.Discard)
		{
			Elements = new();
			Name = name;
			QueueType = queueType;
			Palette = UIPalette.Default;
		}

		public UIElement Add(UIElement e)
		{
			Elements.Add(e);
			return e;
		}

		public void Render(SpriteBatch sb, UIPalette uip)
		{
			sb.Begin();
			foreach (var e in Elements)
			{
				if (e.Visible)
					e.Render(sb, uip);
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
