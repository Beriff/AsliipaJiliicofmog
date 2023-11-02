using System;
using System.Collections.Generic;
using AsliipaJiliicofmog.Env;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Rendering
{
	internal class Emitter
	{
		List<Particle> Particles = [];
		Vector2 Origin;
		readonly Texture2D Texture;
		readonly Random r = new();

		public Emitter(Vector2 o, Texture2D t)
		{
			Origin = o;
			Texture = t;
		}

		public void Render(SpriteBatch sb, World w)
		{
			foreach (Particle x in Particles) x.Display(sb, w);
		}

		public void Update()
		{
			List<Particle> temp = [];
			foreach (Particle x in Particles)
			{
				x.Update();
				if (!x.IsDead()) temp.Add(x);
			}
			Particles = temp;
			Particles.Add(
				new Particle(
					Origin,
					Texture,
					new Vector2(0.01f, 0.01f),
					new Vector2(
						(float)(System.Math.Pow(-1, r.Next(1, 3)) * r.NextDouble()),
						(float)(System.Math.Pow(-1, r.Next(1, 3)) * r.NextDouble())
					)
				)
			);
		}
	}

	internal class Particle
	{
		public Texture2D Texture;
		public float Lifespan;
		public Vector2 Position;
		public Vector2 Velocity;
		public Vector2 Acceleration;
		public Vector2 Scale;

		public Particle(Vector2 l, Texture2D t, Vector2 s, Vector2 v)
		{
			Position = l;
			Acceleration = new Vector2(0, 0.01f);
			Velocity = v;
			Lifespan = 255;
			Texture = t;
			Scale = s;
		}

		public void Update()
		{
			Velocity += Acceleration;
			Position += Velocity;
			Lifespan -= 1;
		}

		public bool IsDead() => Lifespan < 0;

		public void Display(SpriteBatch sb, World w)
		{
			sb.Draw(Texture, w.GetWorldPosition(Position, sb), null, new Color(255, 255, 255, Lifespan), 0f, new Vector2(0, 0), Scale, SpriteEffects.None, 0f);
		}
	}
}
