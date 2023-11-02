using System;
using AsliipaJiliicofmog.Env;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Rendering
{

	internal class Emitter
	{
	}

	internal class Particle
	{
		public Texture2D Texture;

		public float Lifetime;
		public float Lifespan;
		public float ParticleLife;

		public float Speed;
		public Vector2 Position;
		public Vector2 Velocity;
		public Vector2 Acceleration;

		public Emitter Emitter;
		public float EmissionRate;

		public Vector2 Scale;

		public Particle(Vector2 position, Texture2D texture, Vector2 scale)
		{
			Position = position;
			Acceleration = new(0.05f, 0);
			Velocity = new();
			Lifespan = 255;
			Texture = texture;
			Scale = scale;
		}

		public void Update()
		{
			Velocity += Acceleration;
			Position += Velocity;
			Lifespan -= 2;
		}

		private bool IsDead() => Lifespan < 0;

		public void Render(SpriteBatch sb, World w)
		{
			sb.Draw(Texture, 
				w.GetWorldPosition(Position, sb), 
				null, new Color(255, 255, 255, Lifespan), 
				0f, new Vector2(0, 0), Scale, SpriteEffects.None, 0f);
		}
	}

}


