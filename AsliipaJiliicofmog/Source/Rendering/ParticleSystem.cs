using AsliipaJiliicofmog.Env;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Rendering
{
	public class Emitter
	{
		List<Particle> Particles = new();
		Vector2 Origin;
		readonly Texture2D Texture;
		readonly Random r = new();
		readonly Action<Emitter> OnUpdate;
		Vector2 Scale;

		public Emitter(Vector2 o, Texture2D t, Vector2 s, Action<Emitter> f)
		{
			Origin = o;
			Texture = t;
			Scale = s;
			OnUpdate = f;
		}

		public void Render(SpriteBatch sb, World w)
		{
			foreach (Particle x in Particles) x.Render(sb, w);
		}

		public void Update()
		{
			List<Particle> temp = new();
			foreach (Particle x in Particles)
			{
				x.Update();
				if (!x.IsDead()) temp.Add(x);
			}
			Particles = temp;
			Particles.Add(
				new(
					Origin,
					Texture,
					Scale,
					new(
						(float)(MathF.Pow(-1, r.Next(1, 3)) * r.NextDouble()),
						(float)(MathF.Pow(-1, r.Next(1, 3)) * r.NextDouble())
					)
				)
			);
			OnUpdate(this);
		}
	}

	public class Particle
	{
		readonly Texture2D Texture;
		float Lifespan;
		Vector2 Position;
		Vector2 Velocity;
		Vector2 Acceleration;
		Vector2 Scale;

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
			Lifespan -= 5;
		}

		public bool IsDead() => Lifespan < 0;

		public void Render(SpriteBatch sb, World w)
		{
			sb.Draw(Texture, w.GetWorldPosition(Position, sb), null, new Color(255, 255, 255, Lifespan / 255), 0f, new Vector2(0, 0), Scale, SpriteEffects.None, 0f);
		}
	}
}
