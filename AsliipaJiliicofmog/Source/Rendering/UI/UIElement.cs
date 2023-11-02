using AsliipaJiliicofmog.Input;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;

namespace AsliipaJiliicofmog.Source.Rendering.UI
{
	public class UIException : Exception
	{
		public UIException() { }
		public UIException(string message) : base(message) { }
		public UIException(string message,  Exception innerException) : base(message, innerException) { }
	}
	internal static class UIManager
	{
		public static readonly List<UIElement> Elements;
		public static readonly InputConsumer Input;

		static UIManager()
		{
			Elements = new();
			Input = InputManager.GetConsumer("UI");
		}
	}
	internal abstract class UIElement
	{
		protected static readonly Texture2D Texture;

		private Vector2 _Position;
		private Vector2 _Size;
		private Vector2 _Scale;

		public UIElement? Parent;

		public Vector2 AbsolutePosition { get => _Position; }
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

		public abstract void Render();
		public abstract void Update();
	}
}
