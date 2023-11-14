using AsliipaJiliicofmog.Data;
using AsliipaJiliicofmog.Env;
using AsliipaJiliicofmog.Rendering;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AsliipaJiliicofmog.Interactive
{
	public class Player : Creature
	{
		public Player() 
			: base("Carlos", 
				  new StateTexture(
					  ("rest", new AnimatedTexture(Registry.Textures["heart"], 16, 5)),
					  ("left", new GameTexture(Registry.Textures["arrow_left"])),
					  ("right", new GameTexture(Registry.Textures["arrow_right"])),
					  ("up", new GameTexture(Registry.Textures["arrow_up"])),
					  ("down", new GameTexture(Registry.Textures["arrow_down"]))
					  ), 
				  Bodypart.Humanoid())
		{
			SetBottomHitbox();
		}

		public override void Update(World w)
		{
			//force the world camera to look at the player
			w.Camera.Position = Position;
			bool pressed = false;

            if (LocalInput.GetKeyboard().IsKeyDown(Keys.W))
			{
				Move(-Vector2.UnitY, w);
				(Texture as StateTexture).SwitchState("up");
				pressed = true;
			}
			else if (LocalInput.GetKeyboard().IsKeyDown(Keys.S))
			{
				Move(Vector2.UnitY, w);
				(Texture as StateTexture).SwitchState("down");
				pressed = true;
			}
				
			if (LocalInput.GetKeyboard().IsKeyDown(Keys.A))
			{
				Move(-Vector2.UnitX, w);
				(Texture as StateTexture).SwitchState("left");
				pressed = true;
			}
			else if (LocalInput.GetKeyboard().IsKeyDown(Keys.D))
			{
				Move(Vector2.UnitX, w);
				(Texture as StateTexture).SwitchState("right");
				pressed = true;
			}

			if(!pressed) { (Texture as StateTexture).SwitchState("rest"); }
		}
	}
}
