using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog
{
	enum KeyStates
	{
		JPressed,
		Hold,
		JReleased,
		Released
	}
	static class InputHandler
	{
		//KeyStates enum extension
		public static bool Pressed(this KeyStates keystate)
		{
			return keystate == KeyStates.Hold || keystate == KeyStates.JPressed;
		}

		//Other class methods
		public static KeyboardState prevKState = new KeyboardState();
		public static MouseState prevMState = new MouseState();

		static KeyboardState newKState = new();
		static MouseState newMState = new();
		static int prevScrollValue = 0;
		public static int Scroll = 0;

		public static Point MouseDragOffset;
		static Point DragMousePos;

		public static GameClient ActiveGameClient;

		//public static Dictionary<Keys, KeyStates> InputKeyStates = new();
		static private bool pressed(ButtonState bs) { return bs == ButtonState.Pressed; }
		static private bool released(ButtonState bs) { return bs == ButtonState.Released; }
		public static KeyStates GetKeyState(Keys key)
		{
			if (prevKState.IsKeyDown(key) && Keyboard.GetState().IsKeyDown(key))
				return KeyStates.Hold;
			else if (prevKState.IsKeyDown(key) && !Keyboard.GetState().IsKeyDown(key))
				return KeyStates.JReleased;
			else if (!prevKState.IsKeyDown(key) && Keyboard.GetState().IsKeyDown(key))
				return KeyStates.JPressed;
			else
				return KeyStates.Released;
		}
		public static KeyStates LMBState()
		{
			if (pressed(prevMState.LeftButton) && pressed(Mouse.GetState().LeftButton))
				return KeyStates.Hold;
			else if (pressed(prevMState.LeftButton) && released(Mouse.GetState().LeftButton))
				return KeyStates.JReleased;
			else if (released(prevMState.LeftButton) && pressed(Mouse.GetState().LeftButton))
				return KeyStates.JPressed;
			else
				return KeyStates.Released;
		}

		public static KeyStates RMBState()
		{
			if (pressed(prevMState.RightButton) && pressed(Mouse.GetState().RightButton))
				return KeyStates.Hold;
			else if (pressed(prevMState.RightButton) && released(Mouse.GetState().RightButton))
				return KeyStates.JReleased;
			else if (released(prevMState.RightButton) && pressed(Mouse.GetState().RightButton))
				return KeyStates.JPressed;
			else
				return KeyStates.Released;
		}

		public static void Update(GameClient client, KeyboardState newKstate, MouseState newMstate)
		{
			//calculate scroll
			Scroll = prevScrollValue - newMstate.ScrollWheelValue;

			//	calculate drag
			if (released(prevMState.LeftButton) && pressed(newMstate.LeftButton))
			{
				DragMousePos = new Point(newMstate.X, newMstate.Y);
			}
			else if (pressed(prevMState.LeftButton) && pressed(newMstate.LeftButton))
			{
				MouseDragOffset = DragMousePos - new Point(newMstate.X, newMstate.Y);
			}
			else if (pressed(prevMState.LeftButton) && released(newMstate.LeftButton))
			{
				ActiveGameClient.Camera += MouseDragOffset.ToVector2();
				MouseDragOffset = Point.Zero;

				foreach (var act in client.OnLClickEvents)
				{
					act();
				}
			}
					


			//	set states
			prevMState = newMstate;
			prevKState = newKstate;
			prevScrollValue = prevMState.ScrollWheelValue;
		}
		public static Vector2 GetMousePos()
		{
			return new(Mouse.GetState().X, Mouse.GetState().Y);
		}
		public static Vector2 UP = new(0, -1);
		public static Vector2 DOWN = new(0, 1);
		public static Vector2 LEFT = new(-1, 0);
		public static Vector2 RIGHT = new(1, 0);
	}
}
