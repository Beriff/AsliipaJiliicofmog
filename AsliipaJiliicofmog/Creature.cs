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
		//StateMachine Machine;
		//Used for AI
		public Vector2 Node = new(0);
		public bool CastShadow = true;

		public int Health;
		public int MaxHealth;

		public WeightedList<Item> Drops;

		public bool Dead { get => Health <= 0; }
		public bool Damageable = true;

		public float Speed = 1;
		public Texture2D Shadow;

		public override Action<GameClient> OnClick { get => _OnClick; set => _OnClick = (gc) => {Damage(gc.Player, gc, 1); value(gc); }; }
		public override Action<GameClient> OnUpdate { get => base.OnUpdate; set => _OnUpdate = (gc) =>
		{
			if (Dead)
			{
				if (Drops.Count != 0)
				{
					var drop = Drops.Get().Clone() as Item;

					new DroppedItem(drop, Position).AddToRender(gc);
				}
				gc.RemoveEntity(this);
			};
			value(gc);
		}; }

		public Vector2 ShadowOffset()
		{
			//return Vector2.Zero;
			return new Vector2(-1f, .5f) * (new Vector2(EntityTexture.Width, EntityTexture.Height) + AnchorOffset() * new Vector2(1, 1.5f));
		}
		public Creature(Texture2D texture, string name, Vector2? anchor = null, string? desc = null, Vector2? pos = null, int? health = null) : base(pos ?? new(0), texture, name, anchor: anchor, desc: desc)
		{
			Collidable = true;
			OnClick = (gc) => { };
			OnUpdate = (gc) => { };
			GenerateShadow(texture.GraphicsDevice);
			Health = MaxHealth = health ?? 5;
			Drops = new();
		}
		public override void DrawShadow(SpriteBatch sb, Vector2 offset)
		{
			//Draw entity shadow
			if (CastShadow)
				sb.Draw(Shadow, Position + offset + ShadowOffset(), Color.Black);
		}
		public override void Render(SpriteBatch sb, Vector2 offset, GameTime gt, GameClient gc)
		{

			Hitbox schitbox = ScreenCoordsHitbox(offset);
			if (GUI.GUIDEBUG)
				sb.Draw(GUI.Flatcolor, schitbox.ToRect(), Color.Blue);
			var mousepos = InputHandler.GetMousePos();

			if (schitbox.Test(mousepos))
			{
				VisualElements.Label nametag = new(Name);
				nametag.Position = new Vector2(schitbox.Middle().X - nametag.GetSize().X / 2, schitbox.Start.Y - nametag.GetSize().Y).ToPoint();
				nametag.Render(sb);
				if (InputHandler.LMBState() == KeyStates.JReleased)
					OnClick(gc);
			}

			base.Render(sb, offset, gt, gc);
		}
		public override object Clone()
		{
			var creature = new Creature(EntityTexture.Default, Name, Anchor, Description, Position);
			creature.OnUpdate = (gc) => { };
			//creature.OnClick = (gc) => { };
			creature.Node = Node;
			creature.EntityHitbox = EntityHitbox.Clone();
			creature.Health = MaxHealth;
			creature.MaxHealth = MaxHealth;
			creature.Drops = WeightedList<Item>.CopyFrom(Drops);
			return creature;
		}
		public virtual void GenerateShadow(GraphicsDevice gd)
		{
			Shadow = Shaders.ChangeTexture(EntityTexture.Default, Shaders.ShearMapX(1.03f, gd));
			Shadow = Shaders.ChangeTexture(Shadow, Shaders.ShrinkMapY(3, gd));
			Shadow = Shaders.ChangeTexture(Shadow, Shaders.ColorOpaque(new Color(0,0,0,.5f), gd));
		}

		public void Damage(Entity source, GameClient gc, int amount)
		{
			if (!Damageable || source == this)
				return;
			var targethp = Health - amount;
			Health = Math.Clamp(targethp, 0, MaxHealth);

			var dmg_particle = TextParticle.DamageParticle(amount);
			dmg_particle.StartPosition = Position;
			gc.FocusedScene.Particles.Add(dmg_particle);
		}
	}

	public class Prop : Creature
	{
		public List<ToolType> RequiredTool;
		public int Toughness;

		public bool CustomDrop;
		public GameClient Gc;
		public Prop(GameClient gc, Texture2D texture, string name, string desc, int toughness, int health, Vector2? anchor, ToolType[] tooltypes) : base(texture, name, desc: desc, anchor: anchor, health: health)
		{
			Toughness = toughness;
			RequiredTool = new(tooltypes);
			Gc = gc;
			Drops = new();

			OnClick = (gc) =>
			{

				var tool = Gc.Player.Equipped as Tool;
				if (CanHarvest(tool))
					Harvest(tool);
			};
			Damageable = false;
		}
		public Prop WithDrop(Item drop, int weight)
		{
			Drops.Add(drop, weight);
			CustomDrop = true;
			return this;
		}
		public bool CanHarvest(Tool tool)
		{
			if (RequiredTool.Count == 0)
				return true;
			if (tool == null)
				return RequiredTool.Count == 0;
			else
			{
				foreach (var type in tool.ToolTypes)
				{
					if (!RequiredTool.Contains(type))
						return false;
				}
				return true;
			}
		}
		public void Harvest(Tool? htool = null)
		{

			if (Health > 0)
			{
				Health -= 1;
				if(htool != null)
					htool.Damage();
				Animator.Add(new(30, 0, (t, c) => { Tint = Color.Lerp(Color.Red, Color.White, t); }));
			} else
			{
				Item drop = Drops.Get();
				if (!CustomDrop)
				{
					Health = MaxHealth;
					Placeable default_drop = (Placeable)drop;
					default_drop.PlaceableEntity = this;
					new DroppedItem(default_drop, Position).AddToRender(Gc);
					Gc.RemoveEntity(this);
				} else 
				{
					new DroppedItem(Drops.Get(), Position).AddToRender(Gc);
					Gc.RemoveEntity(this);
				}
			}
		}
		public override object Clone()
		{
			var prop = new Prop(Gc, EntityTexture.Default, Name, Description, Toughness, MaxHealth, Anchor, RequiredTool.ToArray());
			prop._OnUpdate = OnUpdate;
			prop.EntityHitbox = EntityHitbox.Clone();
			prop.CustomDrop = CustomDrop;
			prop.Health = MaxHealth;
			if (CustomDrop)
				prop.Drops = WeightedList<Item>.CopyFrom(Drops);
			else
				prop.Drops.Add(prop.GetItem(), 1);
			prop.CastShadow = CastShadow;
			return prop;
		}

	}
}
