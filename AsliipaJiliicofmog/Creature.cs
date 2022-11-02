using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog
{
	public class Creature : Entity
	{
		StateMachine Machine;
		//Used for AI
		public Vector2 Node = new(0);

		public float Speed = 1;

		public Creature(Texture2D texture, string name, Vector2? anchor = null, string? desc = null, Vector2? pos = null) : base(pos ?? new(0), texture, name, anchor: anchor, desc: desc)
		{
			Collidable = true;
		}
		public override void Render(SpriteBatch sb, Vector2 offset, GameTime gt, GameClient gc)
		{
			//Draw entity shadow
			Rectangle shadow_bounds = new((int)(Position.X + offset.X), (int)(Position.Y + EntityTexture.Height * .6f + offset.Y), EntityTexture.Width, (int)(EntityTexture.Height * .3f));
			sb.Draw(GUI.Flatcolor, shadow_bounds, new Color(Color.Black, 137));

			Hitbox schitbox = ScreenCoordsHitbox(offset);
			var mousepos = InputHandler.GetMousePos();

			if (schitbox.Test(mousepos))
			{
				VisualElements.Label nametag = new(Name);
				nametag.Position = new Vector2(schitbox.Start.X + nametag.GetSize().X / 2 - 5, schitbox.Start.Y - nametag.GetSize().Y).ToPoint();
				nametag.Render(sb);
			}

			base.Render(sb, offset, gt, gc);
		}
		public override object Clone()
		{
			var creature = new Creature(EntityTexture, Name, Anchor, Description, Position);
			creature._OnUpdate = OnUpdate;
			creature._OnClick = OnClick;
			creature.Node = Node;
			creature.EntityHitbox = EntityHitbox.Clone();
			return creature;
		}
	}
}
