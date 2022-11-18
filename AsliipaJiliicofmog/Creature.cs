﻿using Microsoft.Xna.Framework;
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

		public float Speed = 1;
		public Texture2D Shadow;
		public Vector2 ShadowOffset()
		{
			//return Vector2.Zero;
			return new Vector2(-1f, .5f) * (new Vector2(EntityTexture.Width, EntityTexture.Height) + AnchorOffset() * new Vector2(1, 1.5f));
		}

		public Creature(Texture2D texture, string name, Vector2? anchor = null, string? desc = null, Vector2? pos = null) : base(pos ?? new(0), texture, name, anchor: anchor, desc: desc)
		{
			Collidable = true;
			OnClick = () => { };
			GenerateShadow(texture.GraphicsDevice);
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
					OnClick();
			}

			base.Render(sb, offset, gt, gc);
		}
		public override object Clone()
		{
			var creature = new Creature(EntityTexture.Default, Name, Anchor, Description, Position);
			creature._OnUpdate = OnUpdate;
			creature._OnClick = OnClick;
			creature.Node = Node;
			creature.EntityHitbox = EntityHitbox.Clone();
			return creature;
		}
		public virtual void GenerateShadow(GraphicsDevice gd)
		{
			Shadow = Util.ChangeTexture(EntityTexture.Default, Util.ShearMapX(1.03f, gd));
			Shadow = Util.ChangeTexture(Shadow, Util.ShrinkMapY(3, gd));
			Shadow = Util.ChangeTexture(Shadow, Util.ColorOpaque(new Color(0,0,0,.5f), gd));
		}
	}

	public class Prop : Creature
	{
		public List<ToolType> RequiredTool;
		public int Toughness;
		public WeightedList<Item> Drops;

		public bool CustomDrop;

		public int Health;
		public int MaxHealth;
		public GameClient Gc;
		public Prop(GameClient gc, Texture2D texture, string name, string desc, int toughness, int health, Vector2? anchor, ToolType[] tooltypes) : base(texture, name, desc: desc, anchor: anchor)
		{
			Toughness = toughness;
			RequiredTool = new(tooltypes);
			MaxHealth = Health = 5;
			Gc = gc;
			Drops = new();

			OnClick = () =>
			{

				var tool = Gc.Player.Equipped as Tool;
				if (CanHarvest(tool))
					Harvest(tool);
			};
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
					default_drop.PlaceableEntity = Clone() as Creature;
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
			if (CustomDrop)
				prop.Drops = WeightedList<Item>.CopyFrom(Drops);
			else
				prop.Drops.Add(prop.GetItem(), 1);
			prop.CastShadow = CastShadow;
			return prop;
		}

	}
}
