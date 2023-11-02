using System;
using System.Collections.Generic;
using AsliipaJiliicofmog.Env;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Rendering
{

  internal class Emitter
  {
    List<Particle> particles = new();
    Vector2 origin;
    Texture2D texture;

    Random r = new Random();

    public Emitter(Vector2 o, Texture2D t)
    {
      origin = o;
      texture = t;
    }

    public void Render(SpriteBatch sb, World w)
    {
      foreach (Particle x in particles) x.Display(sb, w);
      particles.Add(
        new Particle(
          origin,
          texture,
          new Vector2(0.01f, 0.01f),
          new Vector2((float)((System.Math.Pow((-1), r.Next(1, 3))) * r.NextDouble()), (float)((System.Math.Pow((-1), r.Next(1, 3))) * r.NextDouble()))
        )
      );
    }

    public void Update()
    {
      List<Particle> temp = new();
      foreach (Particle x in particles)
      {
        x.Update();
        if (!x.IsDead()) temp.Add(x);
      }
      particles = temp;
    }
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
      // sb.Draw(texture, w.GetWorldPosition(position, sb), new Color(255, 255, 255, lifespan));
      sb.Draw(Texture, w.GetWorldPosition(Position, sb), null, new Color(255, 255, 255, Lifespan), 0f, new Vector2(0, 0), Scale, SpriteEffects.None, 0f);
    }
  }
}