using AsliipaJiliicofmog.Data;
using AsliipaJiliicofmog.Env;
using AsliipaJiliicofmog.Rendering;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AsliipaJiliicofmog.Interactive
{
	internal class Player : Creature
	{
		public Player() 
			: base("Carlos", 
				  new StateTexture(("rest",new AnimatedTexture(Registry.Textures["heart"], 16, 5))), 
				  Bodypart.Humanoid())
		{
			SetBottomHitbox();
		}

		public override void Update(World w)
		{
			//force the world camera to look at the player
			w.Camera.Position = Position;

            if (LocalInput.GetKeyboard().IsKeyDown(Keys.W))
				Move(-Vector2.UnitY, w);
			if (LocalInput.GetKeyboard().IsKeyDown(Keys.S))
				Move(Vector2.UnitY, w);
			if (LocalInput.GetKeyboard().IsKeyDown(Keys.A))
				Move(-Vector2.UnitX, w);
			if (LocalInput.GetKeyboard().IsKeyDown(Keys.D))
				Move(Vector2.UnitX, w);
		}
	}
}
