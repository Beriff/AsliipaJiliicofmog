using System;
using System.Collections.Generic;
using AsliipaJiliicofmog.Env;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsliipaJiliicofmog.Rendering;

internal class Emitter
{
  List<Particle> particles = new();
  Vector2 origin;
  Texture2D texture;

  Random r = new Random();

  public Emitter(Vector2 o, Texture2D t) {
    origin = o;
    texture = t;
  }

  public void Render(SpriteBatch sb, World w) {
    foreach(Particle x in particles) x.Display(sb, w);
    particles.Add(new Particle(origin, texture, new Vector2(0.01f, 0.01f), new Vector2((float)(((-1) ^ r.Next(0,3)) * r.NextDouble()),0)));
  }

  public void Update() {
    List<Particle> temp = new();
    foreach(Particle x in particles) {
      x.Update();
      if (!x.IsDead()) temp.Add(x);
    }
    particles = temp;
  }
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

  public Particle(Vector2 l, Texture2D t, Vector2 s, Vector2 v)
  {
    position = l;
    acceleration = new Vector2(0, 0.01f);
    velocity = v;
    lifespan = 255;
    texture = t;
    scale = s;
  }

  public void Update()
  {
    velocity += acceleration;
    position += velocity;
    lifespan -= 1;
  }

  public bool IsDead() => lifespan < 0;

  public void Display(SpriteBatch sb, World w)
  {
    // sb.Draw(texture, w.GetWorldPosition(position, sb), new Color(255, 255, 255, lifespan));
    sb.Draw(texture, w.GetWorldPosition(position, sb), null, new Color(255, 255, 255, lifespan), 0f, new Vector2(0, 0), scale, SpriteEffects.None, 0f);
  }
}
