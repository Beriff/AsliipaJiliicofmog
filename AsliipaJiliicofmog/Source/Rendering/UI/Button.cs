
using AsliipaJiliicofmog.Event;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Rendering.UI
{
	public class Button : UIElement, IClickable
	{
		public Action OnClick { get; set; }
		//Indicates that the text changed, and the placement needs to be recalculated
		protected bool RecalculateFlag = false;
		protected string _Label;
		protected Color CurrentBGColor;
		public string Label { get => _Label; set { _Label = value; RecalculateFlag = true; } }

		protected Vector2 LabelSize;

		public Button(UIElement? parent, Action onclick, Vector2 pos, Vector2 scale)
			: base(parent, pos, scale)
		{
			OnClick = onclick;
			OnHoverStart = (group, position) =>
			{
				UIEvents.RemoveEvent(Name + "hover_deactivate");

				UIEvents.AddEvent(new(0, 60, Name + "hover_activate", EventQueueBehavior.Discard, (ge) =>
				{
					CurrentBGColor = Color.Lerp(CurrentBGColor, group.Palette.Interactable, ge.Progress);
				}));
			};
			OnHoverEnd = (group, position) =>
			{

				UIEvents.RemoveEvent(Name + "hover_activate");

				UIEvents.AddEvent(new(0, 60, Name + "hover_deactivate", EventQueueBehavior.Discard, (ge) =>
				{
					CurrentBGColor = Color.Lerp(CurrentBGColor, group.Palette.InteractableDark, ge.Progress);
				}));
			};
		}
		public Button(Action onclick, Vector2 pos, Vector2 size)
			: base(pos, size)
		{
			OnClick = onclick;
			OnHoverStart = (group, position) =>
			{
				UIEvents.RemoveEvent(Name + "hover_deactivate");

				UIEvents.AddEvent(new(0, 60, Name + "hover_activate", EventQueueBehavior.Discard, (ge) =>
				{
					CurrentBGColor = Color.Lerp(CurrentBGColor, group.Palette.Interactable, ge.Progress);
				}));
			};
			OnHoverEnd = (group, position) =>
			{
				UIEvents.RemoveEvent(Name + "hover_activate");

				UIEvents.AddEvent(new(0, 60, Name + "hover_deactivate", EventQueueBehavior.Discard, (ge) =>
				{
					CurrentBGColor = Color.Lerp(CurrentBGColor, group.Palette.InteractableDark, ge.Progress);
				}));
			};
		}

		public override void RenderAt(SpriteBatch sb, UIGroup group, Vector2 pos)
		{
			if (CurrentBGColor == Color.Transparent) { CurrentBGColor = group.Palette.InteractableDark; }
			if (RecalculateFlag) { LabelSize = group.Font.MeasureString(Label); RecalculateFlag = false; }
			var text_pos = BoundsAt(pos).Center.ToVector2() - (LabelSize / 2);
			sb.Draw(Texture, BoundsAt(pos), CurrentBGColor);
			sb.DrawString(group.Font, Label, text_pos, group.Palette.Readable);
		}

		public override void UpdateAt(UIGroup group, Vector2 pos)
		{
			if (IsHovered && LocalInput.GetM1State() == Input.PressType.Released)
				OnClick();
			base.UpdateAt(group, pos);
		}

	}
}
