﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsliipaJiliicofmog
{
	struct RelativePosition
	{
		public UIElement Source;
		// Scale relative to source's scale ( (1,1) is 100% of source size )
		public Vector2 RelScale;
		// Position is relative to source's anchor ( (0,0) is source's anchor position)
		public Vector2 RelPosition;

		public Vector2 GetPosition() => Source.GetAnchorPos() + RelPosition * Source.Scale;
		public Vector2 GetScale() => Source.Scale * RelScale;

		public RelativePosition(UIElement source, Vector2 scale, Vector2 pos)
		{
			Source = source;
			RelScale = scale;
			RelPosition = pos;
		}
		public RelativePosition(UIElement source, (float a, float b) scale, (float a, float b) pos)
			: this(source, new Vector2(scale.a, scale.b), new Vector2(pos.a, pos.b)) { }
	}
	struct UIColorPalette
	{
		public Color Main;
		public Color MainDark;
		public Color Highlight;
		public Color HighlightDark;
		public Color Accent;
		public Color Contrast;

		public UIColorPalette(Color main, Color mainDark, Color highlight, Color highlightDark, Color accent, Color contrast)
		{
			Main = main;
			MainDark = mainDark;
			Highlight = highlight;
			HighlightDark = highlightDark;
			Accent = accent;
			Contrast = contrast;
		}
		public static UIColorPalette Default()
		{
			return new UIColorPalette(
				new(107, 107, 107),
				new(84, 84, 84),
				new(82, 75, 183),
				new(50, 55, 158),
				new(255, 161, 107),
				new(255, 255, 255)
				);
		}
	}
	class UIControl
	{
		public UIColorPalette Palette;
		public SpriteFont Font;

		public List<UIElement> UIElements;
		public Texture2D Blank;
		private SpriteBatch Sb;
		public InputHandler Input;
		public UIControl(UIColorPalette palette, SpriteBatch sb, SpriteFont font)
		{
			Palette = palette;
			Sb = sb;
			Blank = new Texture2D(sb.GraphicsDevice, 1, 1);
			Blank.SetData<Color>(new Color[] { Color.White });
			UIElements = new();
			Font = font;
			Input = new();
		}
		public void Render(SpriteBatch sb, GameTime gt)
		{
			foreach(UIElement e in UIElements)
			{
				if(e.Visible) { e.Render(sb, gt); }
			}
		}
		public void Update(GameTime gt)
		{
			Input.Update();
			foreach (UIElement e in UIElements)
			{
				if (e.Active) { e.Update(gt); }
			}
		}
		public void AddElement(UIElement e) { UIElements.Add(e); }
		public void RemoveElement(UIElement e) { UIElements.Remove(e); }
	}
	abstract class UIElement
	{
		protected Vector2 _Position;
		protected Vector2 _Scale;
		protected RelativePosition? _RelPosition;

		public UIControl Controller;
		public bool Visible { get; set; }
		public bool Active { get; set; }

		public virtual Vector2 Position 
		{ 
			get => RelPosition?.GetPosition() ?? _Position;
			set { if (RelPosition == null) { _Position = value; } }
		}
		public virtual Vector2 Scale 
		{
			get => RelPosition?.GetScale() ?? _Scale;
			set { if (RelPosition == null) { _Scale = value; } }
		}
		public bool MouseHover() => GetBounds().Contains(Mouse.GetState().Position);
		public virtual RelativePosition? RelPosition { get; set; }

		//Anchor is the reference point of the whole element
		// (0,0) is top left, (1,1) is bottom right, (.5,.5) is the center of the element
		public virtual Vector2 Anchor { get; set; }
		public Vector2 GetAnchorPos()
		{
			return Position + Scale * Anchor;
		}
		public abstract void Render(SpriteBatch sb, GameTime gt);
		public abstract void RenderAt(Vector2 position, SpriteBatch sb, GameTime gt);
		public abstract void Update(GameTime gt);

		public UIElement(RelativePosition relpos, UIControl controller)
		{
			Visible = true;
			Active = true;
			RelPosition = relpos;
			Controller = controller;
			Controller.AddElement(this);
		}
		public UIElement(Vector2 scale, Vector2 position, UIControl controller)
		{
			Visible = true;
			Active = true;
			RelPosition = null;
			Controller = controller;
			Scale = scale;
			Position = position;
			Controller.AddElement(this);
		}

		public Rectangle GetBounds()
		{
			return new(Position.ToPoint(), Scale.ToPoint());
		}

	}
	abstract class UIContainer : UIElement
	{
		public UIContainer(RelativePosition relpos, UIControl controller) : base(relpos, controller) { Contents = new(); }
		public UIContainer(Vector2 scale, Vector2 position, UIControl controller) : base(scale, position, controller) { Contents = new();  }

		public List<UIElement> Contents;
		public Vector2 ChildLocalPosition(UIElement child)
		{
			return child.Position - Position;
		}
		public override void Render(SpriteBatch sb, GameTime gt)
		{
			foreach(var e in Contents) { if (e.Visible) { e.RenderAt(e.Position - Position, sb, gt); } }
		}
		public override void RenderAt(Vector2 position, SpriteBatch sb, GameTime gt)
		{
			foreach (var e in Contents) { if (e.Visible) { e.RenderAt(position, sb, gt); } }
		}
		public override void Update(GameTime gt)
		{
			foreach (var e in Contents) { if (e.Active) { e.Update(gt); } }
		}
		
		public void AddElement(UIElement e) { Contents.Add(e); e.Controller.RemoveElement(e); }
	}
	interface IClickable
	{
		public Action OnClick { get; set; }
	}
	interface IFocusable
	{
		public bool Focused { get; set; }
		public void OnInput(Keys k);
	}

	/// <summary>
	/// Renders elements inside itself, does not render anything outside itself
	/// </summary>
	class Frame : UIContainer
	{
		public Frame(RelativePosition relpos, UIControl controller) : base(relpos, controller) { }
		public Frame(Vector2 scale, Vector2 position, UIControl controller) : base(scale, position, controller) { }
		public Color? MainColor;
		public override void Render(SpriteBatch sb, GameTime gt)
		{
			//this cant be optimal
			sb.End();
			RenderTarget2D frame = new(sb.GraphicsDevice, (int)Scale.X, (int)Scale.Y);
			sb.GraphicsDevice.SetRenderTarget(frame);
			sb.Begin();
			sb.Draw(Controller.Blank, NumExtend.Vec2Rect(Vector2.Zero, Scale), MainColor ?? Controller.Palette.Main);
			base.Render(sb, gt);
			sb.End();
			sb.GraphicsDevice.SetRenderTarget(null);
			sb.Begin();
			sb.Draw(frame, Position, Color.White);
			sb.End();
			frame.Dispose();
			sb.Begin();
		}
		public override void RenderAt(Vector2 position, SpriteBatch sb, GameTime gt)
		{
			RenderTarget2D frame = new(sb.GraphicsDevice, (int)Scale.X, (int)Scale.Y);
			sb.Draw(Controller.Blank, NumExtend.Vec2Rect(position, Scale), MainColor ?? Controller.Palette.Main);
			sb.GraphicsDevice.SetRenderTarget(frame);

			base.RenderAt(position, sb, gt);
			sb.GraphicsDevice.SetRenderTarget(null);
			sb.Draw(frame, Position, Color.White);
			frame.Dispose();
		}
	}
	/// <summary>
	/// Renders text with provided font. Wraps text to fit horizontal size if this.Wrap == true
	/// </summary>
	class Textbox : UIElement
	{
		public Textbox(RelativePosition relpos, UIControl controller, string text) : base(relpos, controller) { Text = text; }
		public Textbox(Vector2 scale, Vector2 position, UIControl controller, string text) : base(scale, position, controller) { Text = text; }

		protected string _Text;
		public bool Wrap = true;
		public static string WrapString(SpriteFont font, string s, int hsize_px)
		{
			string buffer = "";
			int hsize_space = (int)font.MeasureString(" ").X;
			List<string> words = new();
			foreach(var word in s.Split(' ', '\n'))
			{
				if(font.MeasureString(buffer + word).X + hsize_space > hsize_px)
				{
					words.Add(buffer + '\n');
					buffer = word + ' ';
					continue;
				} else
				{
					buffer += word + ' ';
				}
			}
			words.Add(buffer);
			return string.Join("", words);
		}
		public string Text {
			get => _Text;
			set { _Text = Wrap ? WrapString(Controller.Font, value, (int)Scale.X) : value; }
		}

		public override void Render(SpriteBatch sb, GameTime gt)
		{
			sb.DrawString(Controller.Font, Text, Position, Controller.Palette.Contrast);
		}
		public override void RenderAt(Vector2 position, SpriteBatch sb, GameTime gt)
		{
			sb.DrawString(Controller.Font, Text, position, Controller.Palette.Contrast);
		}
		public override void Update(GameTime gt) { }
	}
	class ProgressBar : UIElement
	{
		public int Progress;
		public int MaxProgress;
		public ProgressBar(RelativePosition relpos, UIControl controller, int maxprogress) : base(relpos, controller) 
		{
			MaxProgress = Progress = maxprogress;
		}
		public ProgressBar(Vector2 scale, Vector2 position, UIControl controller, int maxprogress) : base(scale, position, controller) 
		{
			MaxProgress = Progress = maxprogress;
		}
		public override void Update(GameTime gt) { }
		public override void Render(SpriteBatch sb, GameTime gt)
		{
			sb.Draw(Controller.Blank, NumExtend.Vec2Rect(Position, Scale), Controller.Palette.MainDark);
			sb.Draw(Controller.Blank, NumExtend.Vec2Rect(Position, Scale * new Vector2(Progress/(float)MaxProgress, 1)), Controller.Palette.Highlight);
		}
		public override void RenderAt(Vector2 position, SpriteBatch sb, GameTime gt)
		{
			sb.Draw(Controller.Blank, NumExtend.Vec2Rect(position, Scale), Controller.Palette.MainDark);
			sb.Draw(Controller.Blank, NumExtend.Vec2Rect(position, Scale * new Vector2(Progress / (float)MaxProgress, 1)), Controller.Palette.Highlight);
		}

	}
	class Button : UIElement, IClickable
	{
		public string Text;
		public Action OnClick { get; set; }
		public Button(RelativePosition relpos, UIControl controller, string text, Action onclick) : base(relpos, controller)
		{
			Text = text;
			OnClick = onclick;
		}
		public Button(Vector2 scale, Vector2 position, UIControl controller, string text, Action onclick) : base(scale, position, controller)
		{
			Text = text;
			OnClick = onclick;
		}

		public override void Render(SpriteBatch sb, GameTime gt)
		{
			//change button color on hover
			Color rectcolor = MouseHover() ? 
				Controller.Palette.Highlight : Controller.Palette.HighlightDark;

			sb.Draw(Controller.Blank, NumExtend.Vec2Rect(Position, Scale), rectcolor);

			sb.DrawString(Controller.Font, Text, 
				GetBounds().Center.ToVector2() - Controller.Font.MeasureString(Text) / 2,
				Controller.Palette.Contrast);
		}

		public override void RenderAt(Vector2 position, SpriteBatch sb, GameTime gt)
		{
			var bounds = new Rectangle(position.ToPoint(), Scale.ToPoint());
			Color rectcolor = bounds.Contains(Mouse.GetState().Position) ?
				Controller.Palette.Highlight : Controller.Palette.HighlightDark;

			sb.Draw(Controller.Blank, NumExtend.Vec2Rect(position, Scale), rectcolor);

			sb.DrawString(Controller.Font, Text,
				bounds.Center.ToVector2() - Controller.Font.MeasureString(Text) / 2,
				Controller.Palette.Contrast);
		}

		/*
		 * Since OnClick check is handled separately from rendering the button,
		 * if the button were to be rendered with RenderAt() method,
		 * the click bounds would not be calculated correctly.
		 * RenderAt() can be called from container classes,
		 * so putting clickable objects inside UI containers should generally be
		 * avoided (except the containers that do not move).
		 */
		public override void Update(GameTime gt)
		{
			if (Controller.Input.M1State() == PressState.JustReleased)
			{
				if (GetBounds().Contains(Mouse.GetState().Position))
					OnClick();
			}
		}
	}
	class Slider : UIElement
	{
		public float SliderProgress;
		public int? Steps; //if not null, slider position can only be a/n of horizontal size (a [1;n])

		protected bool CursorLockOn;
		//returns absolute slider position (its x coord)
		protected int AbsSliderPos()
		{
			return (int)(Position.X + SliderProgress * Scale.X - Scale.Y/2);
		}

		public override void Render(SpriteBatch sb, GameTime gt)
		{
			var bounds = new Rectangle(new(AbsSliderPos(), (int)Position.Y), new((int)Scale.Y));
			var slidercolor = Controller.Palette.Highlight;

			if (bounds.Contains(Mouse.GetState().Position))
			{
				slidercolor = Controller.Palette.HighlightDark;
			}

			sb.Draw(Controller.Blank, NumExtend.Vec2Rect(Position, Scale), Controller.Palette.MainDark);
			if(Steps != null)
			{
				for(int i = 0; i <= Steps; i++)
				{
					sb.Draw(Controller.Blank, 
						NumExtend.Vec2Rect(new Vector2( i/(float)Steps*Scale.X - Scale.Y/8, 3*Scale.Y/8) + Position, new(Scale.Y/4) ),
						Controller.Palette.Main);
				}
			}
			sb.Draw(Controller.Blank, NumExtend.Vec2Rect(new(AbsSliderPos(), Position.Y), new(Scale.Y)), slidercolor);
		}
		public override void RenderAt(Vector2 position, SpriteBatch sb, GameTime gt)
		{
			var abssliderpos = (int)(position.X + SliderProgress * Scale.X - Scale.Y / 2);
			sb.Draw(Controller.Blank, NumExtend.Vec2Rect(position, Scale), Controller.Palette.MainDark);
			sb.Draw(Controller.Blank, NumExtend.Vec2Rect(new(abssliderpos, Position.Y), new(Scale.Y)), Controller.Palette.Highlight);
		}

		public Slider(RelativePosition relpos, UIControl controller, int? steps = null) : base(relpos, controller)
		{
			Steps = steps;
			CursorLockOn = false;
			SliderProgress = 0f;
		}
		public Slider(Vector2 scale, Vector2 position, UIControl controller, int? steps = null) : base(scale, position, controller) 
		{
			Steps = steps;
			CursorLockOn = false;
			SliderProgress = 0f;
		}
		public override void Update(GameTime gt)
		{
			var bounds = new Rectangle(new(AbsSliderPos(), (int)Position.Y), new((int)Scale.Y));

			if (Controller.Input.M1State() == PressState.JustPressed && bounds.Contains(Mouse.GetState().Position))
			{
				CursorLockOn = true;
			}
			else if(CursorLockOn && Controller.Input.M1State() == PressState.JustReleased)
			{
				CursorLockOn = false;
				if(Steps != null)
				{
					var posx = Math.Clamp(Mouse.GetState().Position.X, Position.X, Position.X + Scale.X);
					float step = ((float)(Scale.X / Steps));
					posx = MathF.Round( (posx - Position.X) / step) * step;
					SliderProgress = posx / (Scale.X);
				}
			}
			else if(CursorLockOn)
			{
				var posx = Math.Clamp(Mouse.GetState().Position.X, Position.X, Position.X + Scale.X);
				var progress = (posx - Position.X) / (Scale.X);
				SliderProgress = progress;
			}
		}

	}
	class VerticalSlider : UIElement
	{
		public float SliderProgress;
		public int? Steps;

		protected bool CursorLockOn;
		protected int AbsSliderPos()
		{
			return (int)(Position.Y + SliderProgress * Scale.Y - Scale.X / 2);
		}

		public override void Render(SpriteBatch sb, GameTime gt)
		{
			var bounds = new Rectangle(new(AbsSliderPos(), (int)Position.Y), new((int)Scale.X));
			var slidercolor = Controller.Palette.Highlight;

			if (bounds.Contains(Mouse.GetState().Position))
			{
				slidercolor = Controller.Palette.HighlightDark;
			}

			sb.Draw(Controller.Blank, NumExtend.Vec2Rect(Position, Scale), Controller.Palette.MainDark);
			if (Steps != null)
			{
				for (int i = 0; i <= Steps; i++)
				{
					sb.Draw(Controller.Blank,
						NumExtend.Vec2Rect(new Vector2(3 * Scale.X / 8, i / (float)Steps * Scale.Y - Scale.X / 8) + Position, new(Scale.X / 4)),
						Controller.Palette.Main);
				}
			}
			sb.Draw(Controller.Blank, NumExtend.Vec2Rect(new(Position.X, AbsSliderPos()), new(Scale.X)), slidercolor);
		}
		public override void RenderAt(Vector2 position, SpriteBatch sb, GameTime gt)
		{
			var abssliderpos = (int)(position.Y + SliderProgress * Scale.Y - Scale.X / 2);
			sb.Draw(Controller.Blank, NumExtend.Vec2Rect(position, Scale), Controller.Palette.MainDark);
			sb.Draw(Controller.Blank, NumExtend.Vec2Rect(new(Position.X, abssliderpos), new(Scale.X)), Controller.Palette.Highlight);
		}

		public VerticalSlider(RelativePosition relpos, UIControl controller, int? steps = null) : base(relpos, controller)
		{
			Steps = steps;
			CursorLockOn = false;
			SliderProgress = 0f;
		}
		public VerticalSlider(Vector2 scale, Vector2 position, UIControl controller, int? steps = null) : base(scale, position, controller)
		{
			Steps = steps;
			CursorLockOn = false;
			SliderProgress = 0f;
		}
		public override void Update(GameTime gt)
		{
			var bounds = new Rectangle(new((int)Position.X, AbsSliderPos()), new((int)Scale.X));

			if (Controller.Input.M1State() == PressState.JustPressed && bounds.Contains(Mouse.GetState().Position))
			{
				CursorLockOn = true;
			}
			else if (CursorLockOn && Controller.Input.M1State() == PressState.JustReleased)
			{
				CursorLockOn = false;
				if (Steps != null)
				{
					var posx = Math.Clamp(Mouse.GetState().Position.Y, Position.Y, Position.Y + Scale.Y);
					float step = ((float)(Scale.Y / Steps));
					posx = MathF.Round((posx - Position.Y) / step) * step;
					SliderProgress = posx / (Scale.Y);
				}
			}
			else if (CursorLockOn)
			{
				var posx = Math.Clamp(Mouse.GetState().Position.Y, Position.Y, Position.Y + Scale.Y);
				var progress = (posx - Position.Y) / (Scale.Y);
				SliderProgress = progress;
			}
		}
	}
	class Scrollbox : UIContainer
	{
		public int Padding = 10;
		public int Scroll = 0;
		public Scrollbox(RelativePosition relpos, UIControl controller) : base(relpos, controller) { }
		public Scrollbox(Vector2 scale, Vector2 position, UIControl controller) : base(scale, position, controller) { }
		protected int Height()
		{
			int h = Padding * Contents.Count;
			foreach(var e in Contents) { h += (int)e.Scale.Y; }
			return h;
		}
		protected int GetHighestElement()
		{
			int h = (Contents.Count == 0) ? 0 : (int)(ChildLocalPosition(Contents[0]).Y);
			foreach(var e in Contents)
			{
				h = (int)Math.Min(h, (int)(ChildLocalPosition(e).Y));
			}
			return h;
		}
		protected int GetLowestElement()
		{
			int h = (Contents.Count == 0) ? 0 : (int)(ChildLocalPosition(Contents[0]).Y + Contents[0].Scale.Y);
			foreach (var e in Contents)
			{
				h = (int)Math.Max(h, ChildLocalPosition(e).Y + e.Scale.Y);
			}
			return h;
		}

		public override void Render(SpriteBatch sb, GameTime gt)
		{
			sb.End();
			RenderTarget2D frame = new(sb.GraphicsDevice, (int)Scale.X, (int)Scale.Y);
			sb.GraphicsDevice.SetRenderTarget(frame);
			sb.Begin();
			sb.Draw(Controller.Blank, NumExtend.Vec2Rect(Vector2.Zero, Scale), Controller.Palette.Main);

			for(int i = 0; i < Contents.Count; i++)
			{
				var position = new Vector2(Contents[i].Position.X - Position.X, Contents[i].Position.Y - Position.Y + Scroll);
				Contents[i].RenderAt(position, sb, gt);
			}
			//draw scrollbar
			var range = GetLowestElement() - Scale.Y - GetHighestElement();
			var scrollbarsize = Scale.Y / range * Scale.Y * .25f;
			var scrollbarshift = (-Scroll - GetHighestElement()) / (range) * (Scale.Y - scrollbarsize);
			Console.WriteLine($"S{-Scroll} SBS{scrollbarsize} [{scrollbarshift}]");
			sb.Draw(Controller.Blank, NumExtend.Vec2Rect(new(0, scrollbarshift), new(Scale.X * .1f, scrollbarsize)), Controller.Palette.MainDark);

			sb.End();
			sb.GraphicsDevice.SetRenderTarget(null);
			sb.Begin();
			sb.Draw(frame, Position, Color.White);
			sb.End();
			frame.Dispose();
			sb.Begin();
		}

		public override void Update(GameTime gt)
		{
			if(MouseHover())
			{
				var scroll = 3 * Controller.Input.GetScroll();
				var coordscroll = -(Scroll + scroll);
				if (coordscroll > GetHighestElement() && coordscroll + Scale.Y < GetLowestElement())
					Scroll += scroll;
			}
			base.Update(gt);
		}
	}
}
