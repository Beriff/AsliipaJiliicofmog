﻿using System;
using AsliipaJiliicofmog.Data;
using AsliipaJiliicofmog.Env;
using AsliipaJiliicofmog.Input;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Interactive
{
	internal class Entity
	{
		virtual public Vector2 Position { get; set; }
		virtual public string Name { get; set; }
		virtual public Texture2D Texture { get; set; }

		public readonly InputConsumer LocalInput;
		public bool RenderName = true;

		public virtual void Render(SpriteBatch sb, Vector2 screenpos)
		{
			sb.Draw(Texture, screenpos, Color.White);
		}
		public virtual void RenderInWorld(SpriteBatch sb, World w)
		{
			if(RenderName)
			{
				Vector2 vp_pos = ViewportPosition(sb, w);
				Rectangle screen_hitbox = new(vp_pos.ToPoint(), Texture.Bounds.Size);
				if (screen_hitbox.Contains(LocalInput.MousePos()))
				{
					var text_size = Registry.DefaultFont.MeasureString(Name);
					var renderpos = vp_pos + new Vector2(Texture.Bounds.Width / 2f, Texture.Bounds.Height);
					renderpos -= new Vector2(text_size.X / 2, 0);
					sb.DrawString(Registry.DefaultFont, Name, renderpos, Color.White);
				}
			}
			sb.Draw(Texture, Position - w.Camera.Position + sb.GraphicsDevice.Viewport.Bounds.Size.ToVector2() / 2, Color.White);
		}

		public virtual void Update(World w) 
		{

		}

		/// <summary>
		/// Convert world position to screenspace coordinates
		/// </summary>
		public Vector2 ViewportPosition(SpriteBatch sb, World w)
		{
			var vp_mid = new Vector2(sb.GraphicsDevice.Viewport.Width, sb.GraphicsDevice.Viewport.Height) / 2;
			return Position + vp_mid - w.Camera.Position;
		}

		public Entity(string name, Texture2D texture)
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