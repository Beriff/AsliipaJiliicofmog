using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog
{
	enum BpToken
	{
		Internal,
		External,
		Vital,
		Muscle,
		Hold,
		Walk,
		Eat,
		Sight,
		Breath
	}
	class Bodypart
	{
		public List<BpToken> Properties;
		public string Name;
		public bool Bloody;

		public Bodypart Parent;
		public List<Bodypart> Children;

		public int MaxHealth;
		public int Health;

		public void AddChild(Bodypart other)
		{
			other.Parent = this;
			Children.Add(other);
		}
		public Bodypart SetParent(Bodypart other)
		{
			Parent = other;
			other.Children.Add(this);
			return this;
		}

		public Bodypart(string name, int health)
		{
			Children = new();
			Bloody = true;
			Name = name;
			Health = MaxHealth = health;
			Properties = new();
		}
		public Bodypart WithProperties(params BpToken[] tokens)
		{
			Properties.AddRange(tokens);
			return this;
		}

		//Get humanoid anatomy
		public Bodypart[] GetHumanoid()
		{
			var torso = new Bodypart("torso", 100).WithProperties(BpToken.Vital, BpToken.External);
			var head = new Bodypart("head", 50).WithProperties(BpToken.Vital, BpToken.External).SetParent(torso);
			var lefteye = new Bodypart("left eye", 15).WithProperties(BpToken.Sight, BpToken.External).SetParent(head);
			var righteye = new Bodypart("right eye", 15).WithProperties(BpToken.Sight, BpToken.External).SetParent(head);
			var lefthand = new Bodypart("left hand", 50).WithProperties(BpToken.Hold, BpToken.External).SetParent(torso);
			var righthand = new Bodypart("right hand", 50).WithProperties(BpToken.Hold, BpToken.External).SetParent(torso);
			var leftleg = new Bodypart("left leg", 40).WithProperties(BpToken.Walk, BpToken.External).SetParent(torso);
			var rightleg = new Bodypart("right leg", 40).WithProperties(BpToken.Walk, BpToken.External).SetParent(torso);
			var heart = new Bodypart("heart", 20).WithProperties(BpToken.Vital, BpToken.Internal).SetParent(torso);
			var jaw = new Bodypart("jaw", 70).WithProperties(BpToken.Eat, BpToken.External).SetParent(head);
			var stomach = new Bodypart("stomach", 50).WithProperties(BpToken.Vital, BpToken.Internal).SetParent(torso);
			var lungs = new Bodypart("lungs", 40).WithProperties(BpToken.Breath, BpToken.Internal).SetParent(torso);

			return new Bodypart[] { torso, head, lefteye, righteye, lefthand, righthand, leftleg, rightleg, heart, jaw, stomach, lungs };
		}
	}
	public class Creature : Entity
	{
		Bodypart[] Anatomy;
		StateMachine Machine;

		public int Speed = 1;

		public Creature(Vector2 pos, Texture2D texture, string name, Vector2? anchor = null, string? desc = null) : base(pos, texture, name, anchor: anchor, desc: desc)
		{
			
		}
		public override void Render(SpriteBatch sb, Vector2 offset, GameTime gt, GameClient gc)
		{
			//Draw entity shadow
			Rectangle shadow_bounds = new((int)(Position.X + offset.X), (int)(Position.Y + EntityTexture.Height * .6f + offset.Y), EntityTexture.Width, (int)(EntityTexture.Height * .3f));
			sb.Draw(GUI.Flatcolor, shadow_bounds, new Color(Color.Black, 137));

			Hitbox schitbox = ScreenCoordsHitbox(offset);
			var mousepos = InputHandler.GetMousePos();

			if (schitbox.Test(mousepos.ToPoint()))
			{
				VisualElements.Label nametag = new(Name);
				nametag.Position = new Point(schitbox.Start.X + nametag.GetSize().X / 2 - 5, schitbox.Start.Y - nametag.GetSize().Y);
				nametag.Render(sb);
			}

			base.Render(sb, offset, gt, gc);
		}
	}

	public class Player : Creature
	{
		public override Action<GameClient> OnUpdate { get => base.OnUpdate; set
			{
				_OnUpdate = Behavior.PlayerController(this) + value;
			}
		}
		public Player(Texture2D texture, string name) : base(new(70), texture, name)
		{
			Description = "Controllable creature";
			GenerateInfobox();
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
	}
}
