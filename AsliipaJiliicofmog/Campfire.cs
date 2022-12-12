using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog
{
	class Campfire : StationContainer
	{
		public ParticleEmitter Fire;
		
		void InitRecipes()
		{
			static Texture2D cooked(Texture2D og)
			{
				return Shaders.ChangeTexture(og, Shaders.Blend(.4f, Color.Black, og.GraphicsDevice));
			}


			Recipes.Add(
				new Recipe()
				.WithReq(Registry.ItemRegistry["pork"])
				.WithYield(Usable.NewFood(cooked(Registry.TextureRegistry["pork"]), "cooked pork", "its cooked pork", 5))
				);
		}
		public Campfire(GameClient gc) : base(gc, Registry.TextureRegistry["campfire_base"], "campfire", "A place to rest and cook")
		{
			Fire = new ParticleEmitter(Vector2.Zero, Particle.FireParticle, .6f);
			InitRecipes();
			OnPlace = (gc) => { Fire.AddToRender(gc); };
			OnRemove = (gc) => gc.RemoveEntity(Fire);
			OnRClick = (gc) =>
			{
				var equipped = gc.Player.Equipped;
				if (equipped != null)
				{
					var aslist = new List<Item>() { equipped };
					var player = gc.Player;
					foreach (var recipe in Recipes)
					{
						if (recipe.Satisfies(aslist))
						{

							player.Equipped = null;
							player.Inventory.RemoveItem(equipped);
							Slot[0] = equipped;
							Animator.Add(Animation.Schedule(60,
								() =>
								{
									Slot[0] = null;
									new DroppedItem(recipe.Yields[0], Position).AddToRender(gc);
								},
								null));
							break;
						}
					}
				}
			};
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
		public override void Render(SpriteBatch sb, Vector2 offset, GameTime gt, GameClient gc)
		{
			base.Render(sb, offset, gt, gc);
			if (Slot[0] != null)
			{
				var texture = Shaders.ChangeTexture(Slot[0].ItemTexture, Shaders.Shrink(2, sb.GraphicsDevice));
				texture = Shaders.ChangeTexture(texture, Shaders.ClampOpacity(sb.GraphicsDevice));
				var pos = new Vector2(EntityTexture.TextureDims.X / 2, 10) - texture.Size() / 2;
				sb.Draw(texture, offset + pos + Position, Color.White);
			}
		}
	}
}
