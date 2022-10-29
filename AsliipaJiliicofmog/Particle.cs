using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog
{
	public class Particle
	{
		public Color ParticleColor;
		public Vector2 StartPosition;
		public Vector2 Position;
		public Vector2 Acceleration;
		public Vector2 StartVelocity;
		public int Lifetime;
		public int Life;
		public float Transparency;
		public Action<Particle> OnUpdate;

		public float LifeProgress()
		{
			return (float)Life / Lifetime;
		}

		public static Action<Particle> FadeOut = (self) => { self.Transparency = 1 - self.LifeProgress(); };
		public static Action<Particle> WiggleY(float Amplitude = 10) { return (self) => { self.Position.Y += (int)(Amplitude * Math.Sin(Amplitude * 2 * Math.PI * self.LifeProgress())); }; }
		public static Action<Particle> WiggleX(float Amplitude = 10) { return (self) => { self.Position.X += (int)(Amplitude * Math.Sin(Amplitude * 2 * Math.PI * self.LifeProgress())); }; }
		public static Action<Particle> ColorShift(Color a, Color b)
		{
			return (self) => {
				self.ParticleColor = Color.Lerp(a, b, self.LifeProgress());
				};
		}

		public Particle(Color col, Vector2 pos, Vector2 accel, Vector2 vel, int lifetime, Action<Particle>? update = null)
		{
			ParticleColor = col;
			StartPosition = Position = pos;
			Acceleration = accel;
			StartVelocity = vel;
			Lifetime = lifetime;
			Life = 0;
			OnUpdate = update ?? ((self) => { });
			Transparency = 1;
		}
		public Particle Copy()
		{
			return new(ParticleColor, StartPosition, Acceleration, StartVelocity, Lifetime + Asliipa.Random.Next(-55,55), OnUpdate);
		}
		public void Update()
		{
			
			Life += 1;
			Position = StartPosition + StartVelocity * Life + (Acceleration * Life * Life) * .5f;
			OnUpdate(this);
		}
		public void Render(SpriteBatch sb, Vector2 offset)
		{
			sb.Draw(GUI.Flatcolor, Position + offset, new Rectangle(new(0), new(5)), ParticleColor * Transparency);
		}

		public static Particle FireParticle = new Particle(Color.Red, new(0), new(0,0), new(0, -1), 100,
				WiggleX(2.5f) + FadeOut + ColorShift(Color.Red, Color.Yellow));
	}
}
