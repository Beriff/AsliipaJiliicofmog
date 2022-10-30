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

	public class Player : Creature
	{
		public override Action<GameClient> OnUpdate { get => base.OnUpdate; set
			{
				_OnUpdate = Behavior.PlayerController(this) + value;
			}
		}
		public Player(Texture2D texture, string name) : base(texture, name, pos: new(70))
		{
			Description = "Controllable creature";
			GenerateInfobox();
			Speed = 2;
		}
	}

	static class Behavior
	{
		public static Action<GameClient> PlayerController(Creature mob)
		{
			return (gc) =>
			{
				if (InputHandler.GetKeyState(Keys.W).Pressed())
				{
					mob.Move(gc, InputHandler.UP * mob.Speed);
				}
				else if (InputHandler.GetKeyState(Keys.S).Pressed())
				{
					mob.Move(gc, InputHandler.DOWN * mob.Speed);
				}

				if (InputHandler.GetKeyState(Keys.D).Pressed())
				{
					mob.Move(gc, InputHandler.RIGHT * mob.Speed);
				}
				else if (InputHandler.GetKeyState(Keys.A).Pressed())
				{
					mob.Move(gc, InputHandler.LEFT * mob.Speed);
				}
			};
		}
		public static Action<GameClient> RandomWalk(Creature mob)
		{
			return (gc) =>
			{

				Vector2 error = (mob.Position - mob.Node).Abs();
				var range = 5;

				//divide the error by speed, to allow bigger errors on higher speeds
				//same effect as just increasing the "1f" threshold below
				error /= mob.Speed;
				if (error.X < 1f && error.Y < 1f)
				{

					mob.Node = Util.RandomNear(mob.Position);
				} else
				{
					var offset = (mob.Node - mob.Position).NormThis() * mob.Speed;
					//if movement was unsuccessful, set new node
					//new node is set in the opposite (relative to the movement) direction +- 90 degrees
					if(!mob.Move(gc, offset))
					{
						var leftover = mob.Node - mob.Position;
						var opposite = -leftover;
						opposite = opposite.Rotate(Asliipa.Random.Next(-90, 90));
						mob.Node = mob.Position + opposite;
					}
				}

			};
		}
	}
}
