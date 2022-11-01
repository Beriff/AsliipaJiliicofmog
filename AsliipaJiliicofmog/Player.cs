using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using AsliipaJiliicofmog.VisualElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AsliipaJiliicofmog
{
	public class Item
	{
		public Texture2D ItemTexture;
		public string Name;
		public string Description;

		public Item(Texture2D texture, string name, string desc)
		{
			ItemTexture = texture;
			Name = name;
			Description = desc;
		}
	}
	public class Inventory
	{
		public List<Item> InvSpace;
		public Window InvUIWindow;
		public string Name;

		public Item? FocusedItem;
		
		public Inventory(GameClient gc, string name = "inventory")
		{
			var viewport = gc.Sb.GraphicsDevice.Viewport;
			Point invui_offset = new Point(viewport.Width / 4, viewport.Height / 4);

			InvSpace = new();
			Name = name;
			InvUIWindow = Window.LabeledWindow(invui_offset, invui_offset.Mult(2), Asliipa.MainGUIColor, Name).SetPopup() as Window;
			InvUIWindow.AddElement(new ScrollList(new(0, 15), invui_offset));
			InvUIWindow.AddElement(new ColumnList(new(invui_offset.X, 15), new(invui_offset.X, 15)));
			InvUIWindow.AddOnUpdate((ve, gt) => { if (InputHandler.GetKeyState(gc.GameControls.Inv) == KeyStates.JPressed) { ve.Enabled = !ve.Enabled; } });

			InvUIWindow.AddToRender(gc);
		}

		public void Update()
		{
			var columnlist = InvUIWindow[2] as ScrollList;
			InvUIWindow[2] = new ScrollList(columnlist.Position, columnlist.Dimension);
			columnlist = InvUIWindow[2] as ScrollList;
			for (int i = 0; i < InvSpace.Count; i++)
			{
				columnlist.AddElement(
					new Button(new(columnlist.Position.X, 0), new(columnlist.Dimension.X, 15), Asliipa.MainGUIColor, InvSpace[i].Name, () => { })
					);
			}
		}
		public void AddItem(params Item[] items)
		{
			foreach(var item in items) { InvSpace.Add(item); }
			Update();
		}
	}
	public class Player : Creature
	{
		public Inventory Inventory;
		public override Action<GameClient> OnUpdate
		{
			get => base.OnUpdate; set
			{
				_OnUpdate = Behavior.PlayerController(this) + value;
			}
		}
		public Player(Texture2D texture, string name, GameClient gc) : base(texture, name, pos: new(70))
		{
			Description = "Controllable creature";
			GenerateInfobox();
			Speed = 2;
			Inventory = new(gc, "Your Inventory");
		}
	}

	static class Behavior
	{
		public static Action<GameClient> PlayerController(Creature mob)
		{
			return (gc) =>
			{
				if (InputHandler.GetKeyState(Keys.W).Pressed())
				{
					mob.Move(gc, InputHandler.UP * mob.Speed);
				}
				else if (InputHandler.GetKeyState(Keys.S).Pressed())
				{
					mob.Move(gc, InputHandler.DOWN * mob.Speed);
				}

				if (InputHandler.GetKeyState(Keys.D).Pressed())
				{
					mob.Move(gc, InputHandler.RIGHT * mob.Speed);
				}
				else if (InputHandler.GetKeyState(Keys.A).Pressed())
				{
					mob.Move(gc, InputHandler.LEFT * mob.Speed);
				}
			};
		}
		public static Action<GameClient> RandomWalk(Creature mob)
		{
			return (gc) =>
			{

				Vector2 error = (mob.Position - mob.Node).Abs();
				var range = 5;

				//divide the error by speed, to allow bigger errors on higher speeds
				//same effect as just increasing the "1f" threshold below
				error /= mob.Speed;
				if (error.X < 1f && error.Y < 1f)
				{

					mob.Node = Util.RandomNear(mob.Position);
				}
				else
				{
					var offset = (mob.Node - mob.Position).NormThis() * mob.Speed;
					//if movement was unsuccessful, set new node
					//new node is set in the opposite (relative to the movement) direction +- 90 degrees
					if (!mob.Move(gc, offset))
					{
						var leftover = mob.Node - mob.Position;
						var opposite = -leftover;
						opposite = opposite.Rotate(Asliipa.Random.Next(-90, 90));
						mob.Node = mob.Position + opposite;
					}
				}

			};
		}
	}
}
