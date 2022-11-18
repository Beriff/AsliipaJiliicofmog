using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog
{
	class Campfire : StationContainer
	{
		public ParticleEmitter Fire;
		public Campfire(GameClient gc) : base(gc, Registry.TextureRegistry["campfire_base"], "campfire", "A place to rest and cook")
		{
			Fire = new ParticleEmitter(Vector2.Zero, Particle.FireParticle, .3f);

			OnPlace = (gc) => Fire.AddToRender(gc);
			OnRemove = (gc) => gc.RemoveEntity(Fire);
		}
		public override bool Move(GameClient gc, Vector2 offset)
		{
			var thitbox = EntityHitbox + offset;
			foreach (var entity in gc.EntityList())
			{
				if (entity.EntityHitbox.Test(thitbox) && entity != this)
				{
					if (entity.Collidable)
						return false;
				}
				else if (thitbox.Start.X < 0 || thitbox.End.X > Scene.SceneSizePixels || thitbox.Start.Y < 0 || thitbox.End.Y > Scene.SceneSizePixels)
				{
					return false;
				}

			}
			offset = new(offset.X, offset.Y);
			Position += offset;
			EntityHitbox += offset;
			Fire.Position += offset;
			return true;
		}
	}
}
