using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog
{
	public class Scene
	{
		Tile[,] Space;
		public const int SceneSize = 50;
		public const int SceneSizePixels = SceneSize * Tile.TextureSize;

		public List<Particle> Particles;
		public List<Entity> Entities;
		public Tile GetAt(int x, int y) => Space[x, y];
		public void Render(SpriteBatch sb, Vector2 offset, GameTime gt, GameClient gc)
		{
			if(!Asliipa.UIFocused)
				offset += InputHandler.MouseDragOffset.ToVector2();
			int width = sb.GraphicsDevice.Viewport.Width;
			int height = sb.GraphicsDevice.Viewport.Height;

			int fitx = (int)Math.Ceiling((decimal)width / Tile.TextureSize);
			int fity = (int)Math.Ceiling((decimal)height / Tile.TextureSize);

			int halffitx = fitx / 2 + 1;
			int halffity = fity / 2 + 1;

			var centeroffset = new Vector2(width / 2, height / 2);

			Point targettile = new Point((int)Math.Ceiling(offset.X / Tile.TextureSize), (int)Math.Ceiling(offset.Y / Tile.TextureSize));
			Util.EachXY(targettile.X + halffitx, targettile.Y + halffity, (x, y) =>
			{
				
				if (y >= 0 && y < SceneSize && x >= 0 && x < SceneSize)
				{
					//do not ask how this works
					Vector2 drawoffset = new Vector2((x - (targettile.X - halffitx)) * Tile.TextureSize, (y - (targettile.Y - halffity)) * Tile.TextureSize);
					sb.Draw(Space[x, y].Texture, drawoffset, Color.White);
					
				}
				
			}, targettile.X - halffitx, targettile.Y - halffity);

			int f(float x)
			{
				return (int)Math.Ceiling(x / Tile.TextureSize) * Tile.TextureSize;
			}
			var roffset = new Vector2(f(offset.X), f(offset.Y));

			//DRAW PARTICLES
			foreach(var particle in Particles)
			{
				particle.Render(sb, -roffset + centeroffset);
			}
			gc.EntityProcessor.Process(sb, -roffset + centeroffset, gc, gt);
			//DRAW ENTITIES -- moved to GameClient.EntityProcessor
			/*foreach(var entity in Entities)
			{
				if (!entity.RenderEnabled)
					continue;
				entity.Render(sb, -roffset + centeroffset, gt, gc);
			}*/
		}

		public Scene(Tile tile)
		{
			Space = new Tile[SceneSize, SceneSize];
			Particles = new();
			Entities = new();
			Util.EachXY(SceneSize, SceneSize, (x, y) =>
			{
				Space[x, y] = (Tile)tile.Clone();
			});
		}

		public void Update(GameClient gc)
		{
			List<Particle> removelist = new();
			foreach (var particle in Particles)
			{
				if (particle.Life > particle.Lifetime)
					removelist.Add(particle);
				particle.Update();
				
			}
			foreach (var particle in removelist)
				Particles.Remove(particle);

			foreach (var entity in Entities)
			{
				entity.OnUpdate(gc);
			}
				
		}
	}
}
