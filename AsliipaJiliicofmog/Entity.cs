using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog
{
	public abstract class Entity
	{
		public Vector2 Position;
		public Hitbox EntityHitbox;

		public Texture2D EntityTexture;
		public Vector2 Anchor;

		public Color Color;
		public Color Tint;

		public string Name;
		public string Description;

		public bool RenderEnabled = true;
		public bool Collidable;

		public VisualElements.Infobox EntityInfobox;

		protected Action _OnClick;
		public virtual Action OnClick { get => _OnClick; set
			{
				_OnClick = () =>
				{
					value();
				};
			} }
		protected Action<GameClient> _OnUpdate;
		public virtual Action<GameClient> OnUpdate { get => _OnUpdate; set {

				_OnUpdate = (gc) =>
				{

					value(gc);
				};

			} }

		public virtual void SetHitbox()
		{
			EntityHitbox = Hitbox.FromSize(new Point((int)Position.X, (int)(Position.Y + EntityTexture.Height * .6f)), new Point(EntityTexture.Width, (int)(EntityTexture.Height * .3f)));
		}

		public Entity
			(Vector2 pos, Texture2D texture, string name, 
			Color? col = null, Color? tint = null, string? desc = null,
			Vector2? anchor = null, Hitbox? hitbox = null, bool coll = false)
		{
			Position = pos;
			EntityTexture = texture;
			Name = name;
			Color = col ?? Color.White;
			Tint = tint ?? Color;
			Description = desc ?? "You cannot identify this object.";
			Anchor = anchor ?? new(0);
			if (hitbox != null)
				EntityHitbox = hitbox;
			else
				SetHitbox();
			OnUpdate = (scene) => { };
			Collidable = coll;
			GenerateInfobox();
		}
		protected Vector2 AnchorOffset()
		{
			return Anchor * new Vector2(EntityTexture.Width, EntityTexture.Height);
		}
		protected Hitbox ScreenCoordsHitbox(Vector2 offset)
		{
			return Hitbox.FromSize((Position + offset + AnchorOffset()).ToPoint(), new Point(Tile.TextureSize));
		}
		public virtual void Render(SpriteBatch sb, Vector2 offset, GameTime gt, GameClient gc)
		{
			//if debug is enabled, draw hitboxes
			if(GUI.GUIDEBUG)
				sb.Draw(GUI.Flatcolor, offset + Position, EntityHitbox.ToRect(), Color.Lerp(Color.Red, Color.Black, .3f));
			sb.Draw(EntityTexture, Position + offset + AnchorOffset(), Color.Lerp(Color.White, Tint, .3f));
			//draw infobox
			Hitbox schitbox = ScreenCoordsHitbox(offset);
			var mousepos = InputHandler.GetMousePos();
			
			if (schitbox.Test(mousepos.ToPoint()))
			{
				Tint = Color.Black * Easing.Signaling((float)gt.TotalGameTime.TotalSeconds);
				EntityInfobox.Position = (mousepos).ToPoint();
				EntityInfobox.Render(sb, null);
				
				//if clicked on entity, toggle sidebar
				if(InputHandler.LMBState() == KeyStates.JPressed)
				{
					gc.UpdateSidebar(EntityTexture, Name, Description);
					gc.ToggleSidebar();
				}
			}
			else
			{
				Tint = Color;
			}
			
		}
		public virtual void AddToRender(GameClient gc)
		{

			gc.AddEntity(this);
		}
		/// <summary>
		/// attempts to move the entity
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="offset"></param>
		/// <returns>true if movement was successful, false if movement was blocked</returns>
		public virtual bool Move(GameClient gc, Vector2 offset)
		{
			var target_hitbox = EntityHitbox + offset.ToPoint();
			foreach(var entity in gc.EntityList())
			{
				if (entity.EntityHitbox.Test(target_hitbox) && entity != this)
				{
					//Util.DPrint()
					return false;
				}
					
			}
			Position += offset;
			EntityHitbox = target_hitbox;
			return true;
		}

		public string EntityType()
		{
			return GetType().Name;
		}

		public virtual void GenerateInfobox()
		{
			EntityInfobox = new(Name, $"[{EntityType()}]\n{Description}");
		}

	}
	class ParticleEmitter : Entity
	{
		public Particle Instance;
		public Hitbox RandomOffset;
		public float Chance;
		public float Radius;
		public override Action<GameClient> OnUpdate 
		{ 
			get => base.OnUpdate;
			set 
			{ _OnUpdate = (gc) => 
				{
					if(Asliipa.Random.NextDouble() < Chance)
					{
						var particle = Instance.Copy();
						particle.StartPosition = new(Asliipa.Random.Next(RandomOffset.Start.X, RandomOffset.End.X), Asliipa.Random.Next(RandomOffset.Start.Y, RandomOffset.End.Y));
						particle.StartPosition += Position;
						gc.FocusedScene.Particles.Add(particle);
					}
				}; 
			} 
		}
		public ParticleEmitter(Vector2 pos, Particle instance, float chance = 1f, Hitbox? offset = null) : base(pos, null, $"p.emitter", hitbox: new(Rectangle.Empty))
		{
			Instance = instance;
			Instance.StartPosition = Position;
			RenderEnabled = false;
			Chance = chance;
			RandomOffset = offset ?? new(Point.Zero, new(Tile.TextureSize));
		}

		public static ParticleEmitter Campfire(Vector2 position)
		{
			return new(position, Particle.FireParticle, .5f);
		}
		public static ParticleEmitter Circle(Vector2 position, float radius, float amount, Particle? _particle = null, float chance = .3f)
		{
			var particle = _particle ?? Particle.FireParticle;
			var circle_emitter = new ParticleEmitter(position, particle, chance);
			circle_emitter.Radius = radius;
			circle_emitter._OnUpdate = (gc) =>
			{
				for (int i = 0; i < amount; i++)
				{
					float x = i * circle_emitter.Radius / amount;
					float y = (float)Math.Sqrt(circle_emitter.Radius * circle_emitter.Radius - x * x);
					var quarter1 = circle_emitter.Instance.Copy();
					var quarter2 = circle_emitter.Instance.Copy();
					var quarter3 = circle_emitter.Instance.Copy();
					var quarter4 = circle_emitter.Instance.Copy();
					quarter1.StartPosition = new Vector2(x, y) + circle_emitter.Position;
					quarter2.StartPosition = new Vector2(x, -y) + circle_emitter.Position;
					quarter3.StartPosition = new Vector2(-x, y) + circle_emitter.Position;
					quarter4.StartPosition = new Vector2(-x, -y) + circle_emitter.Position;

					if (Asliipa.Random.NextDouble() < circle_emitter.Chance)
						gc.FocusedScene.Particles.Add(quarter1);
					if (Asliipa.Random.NextDouble() < circle_emitter.Chance)
						gc.FocusedScene.Particles.Add(quarter2);
					if (Asliipa.Random.NextDouble() < circle_emitter.Chance)
						gc.FocusedScene.Particles.Add(quarter3);
					if (Asliipa.Random.NextDouble() < circle_emitter.Chance)
						gc.FocusedScene.Particles.Add(quarter4);
				}
			};
			return circle_emitter;
		} 
	}
	class Prop : Entity
	{
		public Prop(Vector2 position, Texture2D texture, string name, string desc, Vector2? anchor = null) : base(position, texture, name, desc: desc, coll: true, anchor: anchor)
		{

		}
	}
}
