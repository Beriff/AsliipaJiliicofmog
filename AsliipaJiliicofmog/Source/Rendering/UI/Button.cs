
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace AsliipaJiliicofmog.Source.Rendering.UI
{
	internal class Button : UIElement, IClickable
	{
		public Action OnClick { get; set; }

		protected bool RecalculateFlag = false;
		protected string _Label;
		public string Label { get => _Label; set { _Label = value; RecalculateFlag = true; } }

		protected Vector2 LabelSize;

		public Button(UIElement? parent, Action onclick, Vector2 pos, Vector2 scale)
			: base(parent, pos, scale) { OnClick = onclick; }
		public Button(Action onclick, Vector2 pos, Vector2 size) 
			: base(pos, size) { OnClick = onclick; }

		public override void Render(SpriteBatch sb, UIGroup group)
		{
			if (RecalculateFlag) { LabelSize = group.Font.MeasureString(Label); RecalculateFlag = false; }
			var text_pos = Bounds.Center.ToVector2() - (LabelSize / 2);
			sb.Draw(Texture, Bounds, Hovered() ? group.Palette.Interactable : group.Palette.InteractableDark);
			sb.DrawString(group.Font, Label, text_pos, group.Palette.Readable);
		}

		public override void Update()
		{
			if(Hovered() && LocalInput.GetM1State() == Input.PressType.Released)
				OnClick();
		}

	}
}
