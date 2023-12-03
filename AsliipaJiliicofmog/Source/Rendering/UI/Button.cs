
using AsliipaJiliicofmog.Data;
using AsliipaJiliicofmog.Event;
using AsliipaJiliicofmog.Input;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Rendering.UI
{
	public class Button : ElementUI
	{
		public Button(Action onclick, string text, DimUI dims, string name = "ui-button") : base(dims, name) 
		{ 
			OnClick = onclick;
			Text = text;
			MouseEntered += (_,_) => 
			{
				EventsUI.RemoveEvent(name + "-onhoverend");
				EventsUI.AddEvent(new(0, 60, name+"-onhover", EventQueueBehavior.Discard,
					(ge) => { HoverProgress = Easing.SmoothStep(ge.Progress); }));
			};
			MouseLeft += (_, _) =>
			{
				EventsUI.RemoveEvent(name + "-onhover");
				EventsUI.AddEvent(new(0, 60, name + "-onhoverend", EventQueueBehavior.Discard,
					(ge) => { HoverProgress = Easing.SmoothStep(1 - ge.Progress); }));
			};
		}

		protected string _Text;
		protected Vector2 TextDims;
		protected float HoverProgress;

		public string Text
		{
			get => _Text;
			set {
				_Text = value;
				TextDims = Registry.DefaultFont.MeasureString(value);
			}
		}
		public Action OnClick { get; set; }

		public override void Update()
		{
			if (Hovered && Input.GetM1State() == PressType.Released)
				OnClick();
			base.Update();
		}
		public override void Render(SpriteBatch sb, GroupUI group)
		{
			sb.Draw(Texture, BoundsRect, 
				Color.Lerp(group.Palette.ForegroundDark, group.Palette.Foreground, HoverProgress));
			var textpos = BoundsRect.Center.ToVector2() - TextDims / 2;
			sb.DrawString(Registry.DefaultFont, Text, textpos, group.Palette.Text);
		}
	}
}
