
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace AsliipaJiliicofmog.Rendering.UI
{
	internal class Button : UIElement, IClickable
	{
		public Action OnClick { get; set; }

		protected bool RecalculateFlag = false;
		protected string _Label;
		protected bool IsHovered = false;
		public string Label { get => _Label; set { _Label = value; RecalculateFlag = true; } }

		protected Vector2 LabelSize;

		public Button(UIElement? parent, Action onclick, Vector2 pos, Vector2 scale)
			: base(parent, pos, scale) { OnClick = onclick; }
		public Button(Action onclick, Vector2 pos, Vector2 size) 
			: base(pos, size) { OnClick = onclick; }

		public override void RenderAt(SpriteBatch sb, UIGroup group, Vector2 pos)
		{
            if (RecalculateFlag) { LabelSize = group.Font.MeasureString(Label); RecalculateFlag = false; }
			var text_pos = BoundsAt(pos).Center.ToVector2() - (LabelSize / 2);
            sb.Draw(Texture, BoundsAt(pos), IsHovered ? group.Palette.Interactable : group.Palette.InteractableDark);
			sb.DrawString(group.Font, Label, text_pos, group.Palette.Readable);
		}

		public override void UpdateAt(Vector2 pos)
		{
			IsHovered = Hovered(pos);
            if (Hovered(pos) && LocalInput.GetM1State() == Input.PressType.Released)
				OnClick();
		}

	}
}
