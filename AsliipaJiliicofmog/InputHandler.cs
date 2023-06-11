using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog
{
	enum PressState
	{
		Released,
		Pressed,
		JustPressed,
		JustReleased
	}
	class InputHandler
	{
		KeyboardState KState;
		KeyboardState PrevKState;
		MouseState MState;
		MouseState PrevMState;

		public InputHandler()
		{
			KState = PrevKState = Keyboard.GetState();
			MState = PrevMState = Mouse.GetState();
		}

		public void Update()
		{
			PrevKState = KState;
			KState = Keyboard.GetState();
			PrevMState = MState;
			MState = Mouse.GetState();
		}

		public PressState GetState(Keys k)
		{
			if (KState.IsKeyDown(k))
			{
				if (PrevKState.IsKeyDown(k))
					return PressState.Pressed;
				else
					return PressState.JustPressed;
			} else
			{
				if (PrevKState.IsKeyDown(k))
					return PressState.JustReleased;
				else
					return PressState.Released;
			}
		}
		public PressState M2State()
		{
			if (MState.RightButton == ButtonState.Pressed)
			{
				if (PrevMState.RightButton == ButtonState.Pressed)
					return PressState.Pressed;
				else
					return PressState.JustPressed;
			}
			else
			{
				if (PrevMState.RightButton == ButtonState.Pressed)
					return PressState.JustReleased;
				else
					return PressState.Released;
			}
		}
		public PressState M1State()
		{
			if (MState.LeftButton == ButtonState.Pressed)
			{
				if (PrevMState.LeftButton == ButtonState.Pressed)
					return PressState.Pressed;
				else
					return PressState.JustPressed;
			}
			else
			{
				if (PrevMState.LeftButton == ButtonState.Pressed)
					return PressState.JustReleased;
				else
					return PressState.Released;
			}
		}
		public Point MouseShift()
		{
			return MState.Position - PrevMState.Position;
		}
	}
}
