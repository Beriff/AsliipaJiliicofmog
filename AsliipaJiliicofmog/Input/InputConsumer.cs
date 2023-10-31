using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog.Input
{
	enum PressType
	{
		Up, // Was up prev frame, still up
		Down, // Was down prev frame, still down
		Pressed, // Got pressed down this frame
		Released // Got released this frame
	}
	internal class InputConsumer
	{
		public bool Active;
		public string Name;
		public bool BlockInputStream;
		public bool IsBlocked;

		private KeyboardState KPrevious;
		private KeyboardState KCurrent;

		private MouseState MPrevious;
		private MouseState MCurrent;

		public InputConsumer(string name, bool blockstream = true)
		{
			Name = name;
			BlockInputStream = blockstream;
			Active = true;
			IsBlocked = false;

			KPrevious = new();
			KCurrent = new();
			MPrevious = new();
			MCurrent = new();
		}

		public void Update()
		{
			KPrevious = KCurrent;
			KCurrent = GetKeyboard();

			MPrevious = MCurrent;
			MCurrent = GetMouse();
		}
		public KeyboardState GetKeyboard() => IsBlocked ? new() : Keyboard.GetState();
		public MouseState GetMouse() => IsBlocked ? new() : Mouse.GetState();

		public PressType GetKeyState(Keys k)
		{
			if(KPrevious.IsKeyDown(k))
			{
				if (KCurrent.IsKeyDown(k)) { return PressType.Down; }
				return PressType.Released;
			}

			if (KCurrent.IsKeyDown(k)) { return PressType.Pressed; }
			return PressType.Up;
		}

		public PressType GetM1State()
		{
			if (MPrevious.LeftButton == ButtonState.Pressed)
			{
				if (MCurrent.LeftButton == ButtonState.Pressed) { return PressType.Down; }
				return PressType.Released;
			}

			if (MCurrent.LeftButton == ButtonState.Pressed) { return PressType.Pressed; }
			return PressType.Up;
		}

		public PressType GetM2State()
		{
			if (MPrevious.RightButton == ButtonState.Pressed)
			{
				if (MCurrent.RightButton == ButtonState.Pressed) { return PressType.Down; }
				return PressType.Released;
			}

			if (MCurrent.RightButton == ButtonState.Pressed) { return PressType.Pressed; }
			return PressType.Up;
		}

		public int GetScrollDelta()
		{
			return MCurrent.ScrollWheelValue - MPrevious.ScrollWheelValue;
		}
	}
}
