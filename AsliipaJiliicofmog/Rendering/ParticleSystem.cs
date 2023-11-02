using System;
using AsliipaJiliicofmog.Env;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Rendering;

internal class Emitter
{
}

internal class Particle
{
  Texture2D texture;
  float lifetime;
  float lifespan;
  float particleLife;
  float speed;
  Emitter emitter;
  float emissionRate;
  Vector2 position;
  Vector2 velocity;
  Vector2 acceleration;
  Vector2 scale;

  public Particle(Vector2 l, Texture2D t, Vector2 s)
  {
    position = l;
    acceleration = new Vector2(0.05f, 0);
    velocity = new Vector2();
    lifespan = 255;
    texture = t;
    scale = s;
  }

  public void Update()
  {
    velocity += acceleration;
    position += velocity;
    lifespan -= 2;
  }

  private bool IsDead() => lifespan < 0;


  public void Display(SpriteBatch sb, World w)
  {
    // sb.Draw(texture, w.GetWorldPosition(position, sb), new Color(255, 255, 255, lifespan));
    sb.Draw(texture, w.GetWorldPosition(position, sb), null, new Color(255, 255, 255, lifespan), 0f, new Vector2(0,0), scale, SpriteEffects.None, 0f);
  }
}
