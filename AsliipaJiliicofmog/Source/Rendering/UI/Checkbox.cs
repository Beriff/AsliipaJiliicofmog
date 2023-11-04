using AsliipaJiliicofmog.Math;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace AsliipaJiliicofmog.Source.Rendering.UI
{
	internal class Checkbox : UIElement, IClickable
	{
		public bool Checked;
		public Action OnClick { get; set; } = () => { };

		public Checkbox(UIElement? parent, Vector2 pos, Vector2 scale)
			: base(parent, pos, scale) { Checked  = false; }
		public Checkbox(Vector2 pos, Vector2 size)
			: base(pos, size) {  Checked = false; }

		public override void Render(SpriteBatch sb, UIPalette uip)
		{
			//Draw the checkbox
			var dims = new Rectangle(AbsolutePosition.ToPoint(), AbsoluteSize.ToPoint());
			sb.Draw(Texture, dims, uip.Main);
			//Draw the check
			if(Checked)
			{
				var cdims = new Rectangle(
					(AbsolutePosition + AbsoluteSize / 4).ToPoint(),
					(AbsoluteSize / 2).ToPoint());
				sb.Draw(Texture, cdims, uip.Accent);
			}
		}

		public override void Update()
		{
			if(Hovered() && LocalInput.GetM1State() == Input.PressType.Released)
			{
				Checked = !Checked;
				OnClick();
			}
		}
	}
}
