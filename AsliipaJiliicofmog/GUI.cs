using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsliipaJiliicofmog
{
	namespace VisualElements
	{
		public abstract class VisualElement
		{
			public Point _Dimension;
			public Point _Position;

			public virtual Point Dimension { get => _Dimension; set => _Dimension = value; }
			public virtual Point Position { get => _Position; set => _Position = value; }

			public Action<VisualElement, GameTime> Update;
			public Action<VisualElement> OnEnable;
			public Action<VisualElement> OnDisable;
			public float Transparency;
			bool _Enabled;
			public bool Enabled
			{
				get { return _Enabled; }
				set { _Enabled = value; if (_Enabled) { OnEnable(this); } else { OnDisable(this); } }
			}
			public virtual void AddOnEnable(Action<VisualElement> val) { OnEnable = val; }
			public virtual void AddOnDisable(Action<VisualElement> val) { OnDisable = val; }
			//Set current element enabling/disabling animations to default ease-out-back
			public virtual VisualElement SetPopup()
			{
				OnEnable += (self) =>
				{
					Animator.Add(new Animation(15, Dimension.X, (t, coeff) => { Dimension = new((int)(Easing.OutBack(t) * coeff), Dimension.Y); }));
					Animator.Add(new Animation(15, Dimension.Y, (t, coeff) => { Dimension = new(Dimension.X, (int)(Easing.OutBack(t) * coeff)); }));
					Animator.Add(new Animation(15, Position.X, (t, coeff) => { Position = new((int)(coeff * 2 - Easing.OutBack(t) * coeff), Position.Y); }));
					Animator.Add(new Animation(15, Position.Y, (t, coeff) => { Position = new(Position.X, (int)(coeff * 2 - Easing.OutBack(t) * coeff)); }));
					Animator.Add(new Animation(15, 1, (t, coeff) => { Transparency = Easing.Linear(t); }));
				};
				return this;
			}
			// Simple linear transparency change
			public virtual VisualElement SetAppear()
			{
				OnEnable += (self) =>
				{
					Animator.Add(new Animation(5, 1, (t, coeff) => { self.Transparency = t; }));
				};
				OnDisable += (self) =>
				{
					Animator.Add(new Animation(5, 1, (t, coeff) => { self.Transparency = 1-t; }));
				};
				return this;
			}
			public VisualElement(Point position, Point size, bool direct = false)
			{
				if(direct)
				{
					_Position = position;
					_Dimension = size;
				} else
				{
					Position = position;
					Dimension = size;
				}
				
				OnEnable = (a) => { };
				OnDisable = (a) => { };
				Transparency = 1f;
			}
			public abstract void Render(SpriteBatch sb, Point? position = null);

			public virtual void AddToRender(GameClient gc)
			{
				gc.VElements.Add(this);
			}
			public virtual void AddOnUpdate(Action<VisualElement, GameTime> act)
			{
				Update = act;
			}
			public Hitbox GetUIBounds()
			{
				return Hitbox.FromSize(Position, Dimension);
			}
		}
		public class ProgressBar : VisualElement
		{
			public Color BarColor;
			public float Progress;
			public string Label;
			Vector2 StringDim;
			public void SetLabel(string newlabel)
			{
				Label = newlabel;
				StringDim = Asliipa.GameFont.MeasureString(newlabel);
			}
			public ProgressBar(float progress, Color color, string label, Point dims, Point pos) : base(pos, dims)
			{
				BarColor = color;
				Progress = progress;
				SetLabel(label);
				Update = (ve, gt) => { };
			}

			public override void Render(SpriteBatch sb, Point? position)
			{
				var pos = position ?? Position;
				int pxlength = (int)(Dimension.X * Progress);
				sb.Draw(GUI.Flatcolor, pos.ToVector2(), new Rectangle(Point.Zero, Dimension), Util.ChangeA(Color.Black, (int)Transparency*255));
				sb.Draw(GUI.Flatcolor, pos.ToVector2(), new Rectangle(Point.Zero, new Point(pxlength, Dimension.Y)), Util.ChangeA(BarColor, (int)(BarColor.A*Transparency)));
				sb.DrawString(Asliipa.GameFont, Label,
					new Vector2(Dimension.X / 2 - StringDim.X / 2, Dimension.Y / 2 - StringDim.Y / 2) + pos.ToVector2(), Util.ChangeA(Color.White, (int)(255 * Transparency)));
			}
		}

		public class Infobox : VisualElement
		{
			public string Header;
			public Color Color;
			public void SetHeader(string header)
			{
				Header = header;
				HeaderDim = Asliipa.GameFont.MeasureString(header);
			}
			public string Paragraph;

			public void SetParagraph(string para)
			{
				Paragraph = para;
				ParaDim = Asliipa.GameFont.MeasureString(para);
			}

			public Vector2 ParaDim;
			public Vector2 HeaderDim;
			readonly static Vector2 BoxSpacing = new Vector2(10);
			public Infobox(string header, string p, Color? color = null) : base(new(0), new(0))
			{
				SetHeader(header);
				SetParagraph(p);
				Color = color ?? Asliipa.MainGUIColor;
				AddOnUpdate((ve, gt) => { });
				Enabled = true;
			}
			public override void Render(SpriteBatch sb, Point? position)
			{
				var pos = position ?? Position;
				int box_x = (int)(Math.Max(HeaderDim.X, ParaDim.X));
				int box_y = (int)(HeaderDim.Y + ParaDim.Y + BoxSpacing.Y);
				sb.Draw(GUI.Flatcolor, pos.ToVector2(),
					new Rectangle(Point.Zero, new Point(box_x + BoxSpacing.ToPoint().X * 2, box_y)),
					Util.ChangeA(Color.Gray, (int)(255* Transparency)));
				sb.DrawString(Asliipa.GameFont, Header, 
					new Vector2(
						box_x / 2 - HeaderDim.X / 2 + BoxSpacing.X + pos.X, 
						pos.Y + 1),
					Util.ChangeA(Color.White, (int)(255 * Transparency)));
				sb.DrawString(Asliipa.GameFont, Paragraph, 
					new Vector2(
						box_x / 2 - ParaDim.X / 2 + BoxSpacing.X + pos.X, 
						pos.Y + HeaderDim.Y + BoxSpacing.Y),
					Util.ChangeA(Color.White, (int)(255 * Transparency)));
			}
		}

		public class Button : VisualElement
		{
			public Color ButtonColor;
			public string Label;
			public Vector2 StringDim;
			public Action OnClick;
			public Point Padding;
			Color RenderColor;

			public void SetLabel(string val)
			{
				Label = val;
				StringDim = Asliipa.GameFont.MeasureString(val);
			}
			public Hitbox ButtonBounds()
			{
				return new Hitbox(Position.ToVector2(), Position.ToVector2() + new Vector2(StringDim.X, StringDim.Y) + Padding.ToVector2());
			}
			public Button(Point pos, Point Dimension, Color color, string label, Action onclick) : base(pos, Dimension)
			{
				ButtonColor = color;
				SetLabel(label);
				AddOnClick(onclick);
				Padding = new(10);
				Enabled = true;
				Update = (ve, gt) =>
				{
					Vector2 mousePos = new(Mouse.GetState().X, Mouse.GetState().Y);
					if (ButtonBounds().Test(mousePos))
					{
						RenderColor = ButtonColor * 2.7f;
						if (InputHandler.LMBState() == KeyStates.JReleased && Enabled)
						{
							OnClick();
						}
							
					}
					else
						RenderColor = ButtonColor;
				};
			}

			public void AddOnClick(Action _event)
			{
				//TODO apparently gets disabled before onclick() is processed, fix tomorrow
				//gl
				OnClick = () => { if (Enabled) { _event(); } };
			}

			public override void AddToRender(GameClient gc)
			{
				gc.VElements.Add(this);
			}

			public override void Render(SpriteBatch sb, Point? position)
			{
				var pos = position ?? Position;
				int width = (int)(StringDim.X);
				sb.Draw(GUI.Flatcolor, pos.ToVector2(),
					new Rectangle(Point.Zero, new Point(width, Dimension.Y) + Padding),new Color(RenderColor, Transparency));
				sb.DrawString(Asliipa.GameFont, Label, new Vector2(width / 2 - StringDim.X / 2 + Padding.X / 2, Dimension.Y / 2 - StringDim.Y / 2 + Padding.Y / 2) + pos.ToVector2(), Util.ChangeA(Color.White, (int)(255 * Transparency)));
			}
		}

		public class Slider : VisualElement
		{
			public Color SliderColor;
			private float _Percent;
			public float Percent { get { return Math.Clamp(_Percent, 0, 1); } private set { _Percent = value; } }
			public Point SliderCursorPos;
			public Point SliderCursorSize;
			public bool SliderCursorLockOn;
			public int CursorShift = 0;
			public Action<Slider> OnChange;

			public Slider(Color slidercol, Point size, Point pos, Action<Slider> act) : base(pos,size)
			{
				SliderColor = slidercol;
				Percent = 0f;
				SliderCursorPos = new(0);
				SliderCursorSize = new(Dimension.X / 10, Dimension.Y);
				SliderCursorLockOn = false;
				AddOnUpdate((ve,gt)=> { });
				OnChange = act;
			}
			public override void AddOnUpdate(Action<VisualElement, GameTime> act)
			{
				Update = (ve, gt) =>
				{
					if (!Enabled)
						return;
					SliderCursorSize = new(Dimension.X / 10, Dimension.Y);
					//Calculate cursor shift:
					//1. get hitbox of slider cursor
					var slider = Hitbox.FromSize(SliderCursorPos.ToVector2() + Position.ToVector2() + new Vector2(CursorShift, 0), SliderCursorSize.ToVector2());
					int mx = Mouse.GetState().X;
					int my = Mouse.GetState().Y;

					bool hover = slider.Test(new Vector2(mx, my));
					if (hover)
						SliderColor = Color.DarkGray;
					else
						SliderColor = Color.Gray;

					//System.Diagnostics.Debug.WriteLine(InputHandler.prevMState.LeftButton);
					if (InputHandler.LMBState() == KeyStates.JPressed && hover)
					{
						SliderCursorLockOn = true;
					}
						
					else if (InputHandler.LMBState() == KeyStates.JReleased)
					{
						SliderCursorLockOn = false;
						SliderCursorPos += new Point(CursorShift, 0);
						CursorShift = 0;
					}

					var changed = false;

					if(SliderCursorLockOn)
					{
						//var newSliderPos = SliderCursorPos.X - InputHandler.MouseDragOffset.X;
						if(CursorShift + SliderCursorPos.X + SliderCursorSize.X <= Dimension.X && CursorShift + SliderCursorPos.X >= 0)
						{
							if (CursorShift != -InputHandler.MouseDragOffset.X) { changed = true; }
							CursorShift = -InputHandler.MouseDragOffset.X;
						} else
						{
							if(CursorShift + SliderCursorPos.X + SliderCursorSize.X > Dimension.X)
							{
								SliderCursorPos.X = Dimension.X - SliderCursorSize.X - CursorShift;
							} else
							{
								SliderCursorPos.X = -CursorShift;
							}
						}
					}

					//Calculate percentage
					Percent = (float)(SliderCursorPos.X + CursorShift) / (Dimension.X - SliderCursorSize.X);
					if (changed) { OnChange(this); }
					act(ve, gt);

					//TODO:
					//remove constant sliderPos += acceleration
					//probably make newSliderPos a class field
					//gl
				};
			}

			public override void Render(SpriteBatch sb, Point? position = null)
			{
				var pos = position ?? Position;
				sb.Draw(GUI.Flatcolor, pos.ToVector2(),
					new Rectangle(Point.Zero, Dimension), Util.ChangeA(Color.Black, (int)(255 * Transparency)));
				sb.Draw(GUI.Flatcolor, SliderCursorPos.ToVector2() + pos.ToVector2() + new Vector2(CursorShift, 0),
					new Rectangle(Point.Zero, SliderCursorSize), Util.ChangeA(SliderColor, (int)(SliderColor.A * Transparency)));
			}
		}

		public class Label : VisualElement
		{
			public string Text;
			public Color TextColor;

			public Point GetSize()
			{
				return Asliipa.GameFont.MeasureString(Text).ToPoint();
			}
			public override void Render(SpriteBatch sb, Point? position = null)
			{
				sb.DrawString(Asliipa.GameFont, Text, (position ?? Position).ToVector2(), Util.ChangeA(TextColor, (int)(TextColor.A * Transparency)));
			}

			public Label(string text, Color? color = null) : base(new(0), new(0))
			{
				TextColor = color ?? Color.White;
				Text = text;
				Update = (v, g) => { };
				Dimension = GetSize();
			}
		}

		public abstract class UIContainer : VisualElement
		{
			public List<VisualElement> Elements;
			public override void AddOnEnable(Action<VisualElement> val) { OnEnable = (element) => { foreach (var e in Elements) { e.Enabled = true; } val(element); }; }
			public override void AddOnDisable(Action<VisualElement> val) { OnDisable = (element) => { foreach (var e in Elements) { e.Enabled = false; } val(element); }; }
			public override Point Position { get => base.Position; set { var shift = value - _Position; foreach (var e in Elements) { e.Position += shift; } _Position = value; } }
			public UIContainer(Point pos, Point size) : base(pos, size, true)
			{
				Elements = new();
				AddOnEnable((e) => { });
				AddOnDisable((e) => { });
			}

			public abstract void AddElement(VisualElement element);

			public VisualElement this[int i] { get => Elements[i]; set => Elements[i] = value; }

			public override void AddOnUpdate(Action<VisualElement, GameTime> act)
			{
				Update = (ve, gt) =>
				{
					foreach (var element in Elements)
						element.Update(ve, gt);
					act(ve, gt);
				};
			}

		}

		public class Window : UIContainer
		{
			public Color WindowColor;
			public Window(Point pos, Point size, Color color) : base(pos, size)
			{
				WindowColor = color;
				Enabled = true;
				AddOnUpdate((v, g) => { });

				var button = new Button(new(Dimension.X - 31, -15), new(10), WindowColor, "x", () => { Enabled = !Enabled; }).SetAppear() as Button;
				AddElement(button);

			}

			public static Window LabeledWindow(Point pos, Point size, Color color, string text)
			{
				Window newwindow = new Window(pos, size, color);
				var header = new Label(text).SetAppear() as Label;
				header.Position = new((int)(size.X / 2 - header.GetSize().X / 2 - 15), 0);
				newwindow.AddElement(header);
				return newwindow;
			}

			public override void AddElement(VisualElement ve)
			{
				ve.Position += Position + new Point(15);
				Elements.Add(ve);
			}

			public override void Render(SpriteBatch sb, Point? position = null)
			{
				if(Enabled)
				{
					Point pos = position ?? Position;
					sb.Draw(GUI.Flatcolor, pos.ToVector2(),
						new Rectangle(Point.Zero, Dimension), Util.ChangeA(WindowColor, (int)(WindowColor.A * Transparency)));
					foreach (var ve in Elements)
						ve.Render(sb);
						
				}
				
			}

			public void SetRelPos(Vector2 pos, VisualElement ve)
			{
				ve.Position = new((int)pos.X * Dimension.X, (int)pos.Y * Dimension.Y);
			}
			public void SetRelSize(Vector2 size, VisualElement ve)
			{
				ve.Dimension = new((int)size.X * Dimension.X, (int)size.Y * Dimension.Y);
			}
		}

		public class ColumnList : UIContainer
		{
			public int Padding;

			public ColumnList(Point position, Point size) : base(position, size)
			{
				AddOnUpdate((ve,gt) => { });
				Elements = new();
				Padding = 20;
			}

			public override void AddElement(VisualElement element)
			{
				if(Elements.Count > 0)
				{
					var prevpos = Elements[^1];
					Elements.Add(element);
					element.Position = new(element.Position.X, prevpos.Position.Y + Padding + prevpos.Dimension.Y);
					//element.Position.X = Position.X;
				} else
				{
					Elements.Add(element);
					element.Position = new(element.Position.X, Position.Y);
				}
				
			}
			public override void AddOnUpdate(Action<VisualElement, GameTime> act)
			{
				Update = (ve, gt) => 
				{
					foreach (var mve in Elements)
						mve.Update(mve, gt);
					act(ve, gt);
				};
			}
			public override void Render(SpriteBatch sb, Point? position = null)
			{
				if (GUI.GUIDEBUG)
				{
					sb.Draw(GUI.Flatcolor, Position.ToVector2(), new Rectangle(new(0, 0), new(2, Elements.Count > 0 ? Elements[^1].Position.Y : 5)), Util.ChangeA(Color.Red, 15));
					sb.DrawString(Asliipa.GameFont, $"DBG ColumnList [{GetHashCode()}]", Position.ToVector2(), Color.Black);
				}
				foreach (var ve in Elements)
					ve.Render(sb, position);
			}
		}
		public class RowList : UIContainer
		{
			int Padding = 10;

			public RowList(Point pos, Point dim) : base(pos, dim)
			{
				AddOnUpdate((ve, gt) => { });
			}
			public override void AddOnUpdate(Action<VisualElement, GameTime> act)
			{
				Update = (ve, gt) =>
				{
					foreach (var element in Elements)
						element.Update(ve, gt);
					act(ve, gt);
				};
			}
			public override void AddElement(VisualElement element)
			{
				Elements.Add(element);
				element.Position = Position;
				var count = Elements.Count;
				var dim = Dimension.X / count;
				element.Position = new(element.Position.X + dim * (count - 1) + (count == 1 ? 0 : Padding), element.Position.Y);
				foreach (var elm in Elements)
				{
					elm.Dimension = new(dim, elm.Dimension.Y);
				}
					
			}
			public override void Render(SpriteBatch sb, Point? position = null)
			{
				if(GUI.GUIDEBUG)
				{
					sb.Draw(GUI.Flatcolor, Position.ToVector2(), new Rectangle(new(0, 0), new(Dimension.X, 2)), Util.ChangeA(Color.Red, 15));
					sb.DrawString(Asliipa.GameFont, $"DBG RowList [{GetHashCode()}]", Position.ToVector2(), Color.Black);
				}
				foreach (var ve in Elements)
					ve.Render(sb, position);
			}
		}

		public class ScrollList : UIContainer
		{
			public int Scroll;
			public int? ElementHeight;
			public int Padding = 6;
			//Scroll list only supports UI elements of the same height
			public ScrollList(Point pos, Point size) : base(pos, size)
			{
				AddOnUpdate((ve,gt) => { });
			}
			public override void AddElement(VisualElement element)
			{
				if (ElementHeight == null) { ElementHeight = element.Dimension.Y; }
				else { if (ElementHeight != element.Dimension.Y) { throw new Exception("ScrollList doesn't support elements with different height"); } }
				element.Position = new(element.Position.X, Position.Y + Elements.Count * (int)ElementHeight);
				Elements.Add(element);
			}
			public override void Render(SpriteBatch sb, Point? position = null)
			{
				if (GUI.GUIDEBUG)
				{
					sb.Draw(GUI.Flatcolor, Position.ToVector2(), new Rectangle(new(0, 0), new(2, Elements.Count > 0 ? Elements[^1].Position.Y : 5)), Util.ChangeA(Color.Red, 15));
					sb.DrawString(Asliipa.GameFont, $"DBG ColumnList [{GetHashCode()}]", Position.ToVector2(), Color.Black);
				}

				//max amount of elements that can be rendered
				int amount = Dimension.Y / (int)ElementHeight;
				Scroll = Math.Min(Scroll, Elements.Count - amount);

				for(int i = Scroll; i < amount + Scroll; i++)
				{
					if (i >= Elements.Count)
						break;

					var element = Elements[i];
					element.Position = new (element.Position.X, Position.Y + (i - Scroll) * (int)(ElementHeight + Padding));
					element.Render(sb, element.Position + new Point(7, 0));
				}

				//Render vertical slider
				var slider_bgy = amount * (int)ElementHeight;
				var slider_height = slider_bgy / (Elements.Count - amount);
				sb.Draw(GUI.Flatcolor, Position.ToVector2(), new Rectangle(new(0), new(5, slider_bgy)), Color.Black);
				sb.Draw(GUI.Flatcolor, new Vector2(Position.X, Position.Y + slider_height * Scroll), new Rectangle(new(0), new(5, slider_height)), Color.Gray);


			}
			public override void AddOnUpdate(Action<VisualElement, GameTime> act)
			{
				Update = (ve, gt) =>
				{
					int amount = Dimension.Y / (int)ElementHeight;
					if (GetUIBounds().Test(InputHandler.GetMousePos()))
					{
						if (InputHandler.Scroll > 0)
						{
							Scroll++;
						}
						else if (InputHandler.Scroll < 0) { Scroll--; }
						Scroll = Math.Clamp(Scroll, 0, Elements.Count);
					}
					for(int i = Scroll; i < amount + Scroll; i++)
					{
						if (i >= Elements.Count)
							break;
						Elements[i].Update(ve, gt);
					}
					act(ve, gt);
				};
			}
		}
		class Dropdown : VisualElement
		{
			public override Point Position { get => base.Position; set { base.Position = value; DroppedOptions = new(Position + new Point((int)StringDim.X, Dimension.Y), Dimension); } }
			public List<string> Options;
			string _SelectedOption;
			Color DropdownColor;
			public string SelectedOption { get => _SelectedOption; set { _SelectedOption = value; StringDim = Asliipa.GameFont.MeasureString(value); } }
			public Vector2 StringDim;
			public Color RenderColor;

			bool Dropped;
			ColumnList DroppedOptions;

			public Dropdown(Color color, string text, Point pos, Point size, params string[] options) : base(pos, size)
			{
				SelectedOption = text;
				RenderColor = DropdownColor = color;
				Options = new(options);
				AddOnUpdate((ve, gt) => { });
				Dropped = false;
				DroppedOptions = new(Position + new Point((int)StringDim.X, Position.Y), size);
				DroppedOptions.Padding = 0;
				RedoDropdown();
			}
			public Hitbox DropdownBounds()
			{
				return new Hitbox(Position.ToVector2(), Position.ToVector2() + new Vector2((int)StringDim.X, Dimension.Y));
			}
			public void RedoDropdown()
			{
				List<float> buttons_x = new();
				foreach (var option in Options)
				{
					DroppedOptions.AddElement(
						new Button(Position, new(Dimension.X, Dimension.Y), DropdownColor, option, () => { SelectedOption = option; })
						);
					buttons_x.Add(Asliipa.GameFont.MeasureString(option).X);
				}
				var max_x = buttons_x.Max();
				StringDim = new(max_x, StringDim.Y);
				foreach (var option in DroppedOptions.Elements)
					(option as Button).StringDim.X = max_x;
			}
			public override void AddOnUpdate(Action<VisualElement, GameTime> act)
			{
				Update = (ve, gt) =>
				{
					//update dropped options if the menu is dropped
					if (Dropped)
						DroppedOptions.Update(DroppedOptions, gt);
					//if pressed
					Vector2 mousePos = new(Mouse.GetState().X, Mouse.GetState().Y);
					if (InputHandler.LMBState() == KeyStates.JReleased)
					{
						if (DropdownBounds().Test(mousePos))
						{
							Dropped = !Dropped;
							if(Dropped)
							{
								//Create ColumnList and fill with buttons representing options
								RedoDropdown();
								DroppedOptions.Enabled = true;
							} else
							{
								DroppedOptions = new(Position + new Point((int)StringDim.X, Dimension.Y), Dimension);
								DroppedOptions.Padding = 11;
								DroppedOptions.Enabled = false;
							}
						} else
						{
							//recreate the column list
							DroppedOptions = new(Position + new Point((int)StringDim.X, Dimension.Y), Dimension);
							DroppedOptions.Padding = 11;
							Dropped = false;
							DroppedOptions.Enabled = false;
						}
							
					} else
					{
						if (DropdownBounds().Test(mousePos))
						{
							RenderColor = DropdownColor * 2.7f;
						} else
						{
							RenderColor = DropdownColor;
						}
					}
					act(ve, gt);
				};
			}
			public override void Render(SpriteBatch sb, Point? position = null)
			{
				var pos = position ?? Position;
				//render the textox
				sb.Draw(GUI.Flatcolor, pos.ToVector2(), new Rectangle(Point.Zero, new((int)StringDim.X, Dimension.Y)), new Color(RenderColor, Transparency));
				sb.DrawString(Asliipa.GameFont, SelectedOption, new(pos.X, pos.Y), Color.White);

				//render the options if Dropped is true
				DroppedOptions.Render(sb, position);
				//System.Diagnostics.Debug.WriteLine(Dimension);
			}
		}

		class Image : VisualElement
		{
			public Texture2D ImageTexture;
			public float ScalingFactor = 1f;
			public Image(Point pos, Texture2D image) : base(pos, new(image.Width, image.Height))
			{
				AddOnUpdate((ve, gt) => { });
				ImageTexture = image;
				
			}
			public Image(Point pos, Texture2D image, float size) : base(pos, new((int)size))
			{
				AddOnUpdate((ve, gt) => { });
				ImageTexture = image;
				ScalingFactor = size / image.Width;
			}
			public override void Render(SpriteBatch sb, Point? position = null)
			{
				sb.Draw(ImageTexture, (position ?? Position).ToVector2(), Color.White);
			}
		}
	}

	class GUI : ICloneable
	{
		public Action<GUI, SpriteBatch, GameTime> Render;
		public List<GUI> GUIList;
		public static bool GUIDEBUG = false;

		public static Texture2D Flatcolor;

		public static void Init(GraphicsDevice gd)
		{
			var tsize = 1024;
			Flatcolor = new Texture2D(gd, tsize, tsize);
			Color[] data = new Color[tsize * tsize];
			Util.EachXY(tsize, tsize, (x, y) => { data[y * tsize + x] = Color.White; });
			Flatcolor.SetData(data);
		}

		public GUI(Action<GUI, SpriteBatch, GameTime> render, params GUI[]? guilist)
		{
			Render = render;
			GUIList = new(guilist ?? new GUI[0]);
		}

		public object Clone()
		{
			List<GUI> newguilist = new();
			foreach(var gui in GUIList) { newguilist.Add(gui.Clone() as GUI); }
			GUI newgui = new(Render, newguilist.ToArray());
			return newgui;
		}

	}
}
