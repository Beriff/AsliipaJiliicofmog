using AsliipaJiliicofmog.Env;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Interactive
{
	internal class Entity
	{
		virtual public Vector2 Position { get; set; }
		virtual public string Name { get; set; }
		virtual public Texture2D Texture { get; set; }

		public virtual void Render(SpriteBatch sb, Vector2 screenpos)
		{
			sb.Draw(Texture, screenpos, Color.White);
		}
		public virtual void RenderInWorld(SpriteBatch sb, World w)
		{
			sb.Draw(Texture, Position - w.Camera.Position + sb.GraphicsDevice.Viewport.Bounds.Size.ToVector2() / 2, Color.White);
		}
		public virtual void Update() { }
		public Entity(string name, Texture2D texture)
		{
			Name = name;
			Texture = texture;
			Position = Vector2.Zero;
		}
	}

	internal class PhysicalEntity : Entity
	{
		public Vector2 HitboxSize;
		public Vector2 HitboxAnchor;
		public Rectangle Hitbox 
		{ 
			get => 
				new(
					(Position + Texture.Bounds.Size.ToVector2() * HitboxAnchor).ToPoint(), 
					HitboxSize.ToPoint()
					); 
		}

		public bool Collides(PhysicalEntity other)
		{
			return Hitbox.Intersects(other.Hitbox);
		}

		public void Shift(Vector2 shift, World w)
		{
			var target = Position + shift;
			Rectangle rect = new((target + Texture.Bounds.Size.ToVector2() * HitboxAnchor).ToPoint(),
				HitboxSize.ToPoint());
			foreach(var entity in w.Entities)
			{
				if(typeof(Entity).IsSubclassOf(typeof(PhysicalEntity))
					&& rect.Intersects(((PhysicalEntity)entity).Hitbox))
				{
					return;
				}
			}
			Position = target;
		}
		public PhysicalEntity(string name, Texture2D texture, Vector2? hitsize = null)
			: base(name, texture)
		{
			HitboxSize = hitsize ?? texture.Bounds.Size.ToVector2();
		}
	}
}
