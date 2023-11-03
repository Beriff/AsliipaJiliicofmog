using System;
using AsliipaJiliicofmog.Data;
using AsliipaJiliicofmog.Env;
using AsliipaJiliicofmog.Input;
using AsliipaJiliicofmog.Math;
using AsliipaJiliicofmog.Rendering;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Interactive
{
	internal class Entity
	{
		virtual public Vector2 Position { get; set; }
		virtual public string Name { get; set; }
		virtual public IGameTexture Texture { get; set; }

		public readonly InputConsumer LocalInput;
		public bool RenderName = true;

		public virtual void Render(SpriteBatch sb, GameTime gt, Vector2 screenpos)
		{
			Texture.Render(sb, gt, screenpos, Color.White);
		}
		public virtual void RenderInWorld(SpriteBatch sb, GameTime gt, World w)
		{
			if(RenderName)
			{
				Vector2 vp_pos = ViewportPosition(sb, w);
				Rectangle screen_hitbox = new(vp_pos.ToPoint(), Texture.Size.Mul(w.Camera.Scale));
				if (screen_hitbox.Contains(LocalInput.MousePos()))
				{
					var text_size = Registry.DefaultFont.MeasureString(Name);
					var renderpos = UnscaledViewportPosition(sb, w) + new Vector2(Texture.Size.X / 2f, Texture.Size.Y);
					renderpos -= new Vector2(text_size.X / 2, 0);
					sb.DrawString(Registry.DefaultFont, Name, renderpos, Color.White);
				}
			}
			Texture.Render(sb, gt, Position - w.Camera.Position + sb.GraphicsDevice.Viewport.Bounds.Size.ToVector2() / 2, Color.White);
		}

		public virtual void Update(World w) 
		{

		}

		/// <summary>
		/// Convert world position to screenspace coordinates accounting the zoom
		/// </summary>
		public Vector2 ViewportPosition(SpriteBatch sb, World w)
		{
			var vp_mid = new Vector2(sb.GraphicsDevice.Viewport.Width, sb.GraphicsDevice.Viewport.Height) / 2;
			return (Position - w.Camera.Position) * w.Camera.Scale + vp_mid;
		}

		public Vector2 UnscaledViewportPosition(SpriteBatch sb, World w)
		{
			var vp_mid = new Vector2(sb.GraphicsDevice.Viewport.Width, sb.GraphicsDevice.Viewport.Height) / 2;
			return Position - w.Camera.Position + vp_mid;
		}

		public Entity(string name, IGameTexture texture)
		{
			Name = name;
			Texture = texture;
			Position = Vector2.Zero;
			LocalInput = InputManager.GetConsumer("Gameplay");
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
					(Position + Texture.Size.ToVector2() * HitboxAnchor).ToPoint(), 
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
			Rectangle rect = new((target + Texture.Size.ToVector2() * HitboxAnchor).ToPoint(),
				HitboxSize.ToPoint());
			foreach(var entity in w.Entities)
			{
				if (
					entity.GetType() == typeof(PhysicalEntity) 
					&& entity != this
					&& rect.Intersects(((PhysicalEntity)entity).Hitbox)
					) { return;	}
			}
			Position = target;
		}
		public PhysicalEntity(string name, IGameTexture texture, Vector2? hitsize = null)
			: base(name, texture)
		{
			HitboxSize = hitsize ?? texture.Size.ToVector2();
		}
	}
}
