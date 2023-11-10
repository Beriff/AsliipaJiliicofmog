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
		protected Vector2 _TextureOffsetPivot;

		virtual public Vector2 Position { get; set; }
		virtual public string Name { get; set; }
		virtual public IGameTexture Texture { get; set; }
		virtual public Vector2 TextureOffsetPivot 
		{
			get => -_TextureOffsetPivot * Texture.Size.ToVector2(); 
			set => _TextureOffsetPivot = value; 
		}

		public readonly InputConsumer LocalInput;
		public bool RenderName = true;

		public virtual void Render(SpriteBatch sb, Vector2 screenpos)
		{
			Texture.Render(sb, screenpos + TextureOffsetPivot, Color.White);
		}
		public virtual void RenderInWorld(SpriteBatch sb, GameTime gt, World w)
		{
			if(RenderName)
			{
				Vector2 vp_pos = ViewportPosition(sb, w);
				Rectangle screen_hitbox = new((vp_pos + TextureOffsetPivot * w.Camera.Scale).ToPoint(), Texture.Size.Mul(w.Camera.Scale));
				if (screen_hitbox.Contains(LocalInput.MousePos()))
				{
					var text_size = Registry.DefaultFont.MeasureString(Name);
					var renderpos = UnscaledViewportPosition(sb, w) + new Vector2(Texture.Size.X / 2f, Texture.Size.Y);
					renderpos -= new Vector2(text_size.X / 2, 0);
					renderpos += TextureOffsetPivot;
					sb.DrawString(Registry.DefaultFont, Name, renderpos, Color.White);
				}
			}
			Texture.Render(sb, 
				Position 
					- w.Camera.Position 
					+ sb.GraphicsDevice.Viewport.Bounds.Size.ToVector2() / 2
					+ TextureOffsetPivot,
				Color.White);
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
		public Vector2 HitboxScale;
		public Vector2 TexturePivot = Vector2.Zero;
		public Rectangle Hitbox 
		{ 
			get => 
				new(
					(Position + Texture.Size.ToVector2() * HitboxAnchor).ToPoint(), 
					(HitboxSize * HitboxScale).ToPoint()
					); 
		}

		public bool Collides(PhysicalEntity other)
		{
			return Hitbox.Intersects(other.Hitbox);
		}

		public void Shift(Vector2 shift, World w)
		{
			Rectangle rect = new (Hitbox.Location + shift.ToPoint(), Hitbox.Size);
			foreach(var entity in w.Entities)
			{
				if (
					entity.GetType() == typeof(PhysicalEntity) 
					&& entity != this
					&& rect.Intersects(((PhysicalEntity)entity).Hitbox)
					) { return;	}
			}
			Position += shift;
		}

		public PhysicalEntity SetBottomHitbox()
		{
			HitboxAnchor = new(0, .6f);
			HitboxScale = new(1, .3f);
			return this;
		}

		public PhysicalEntity(string name, IGameTexture texture, Vector2? hitsize = null)
			: base(name, texture)
		{
			HitboxSize = hitsize ?? texture.Size.ToVector2();
		}
	}
}
