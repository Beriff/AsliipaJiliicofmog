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

		public virtual List<(string name, Action action)> GetActions(Inventory inv)
		{
			return new() { new("Drop", () => 
				{
					inv.RemoveItem(inv.FocusedItem); 
					new DroppedItem(inv.FocusedItem, inv.Client.Player.Position).AddToRender(inv.Client);
				}
			) };
		}
	}
	public class Inventory
	{
		public List<Item> InvSpace;
		public Window InvUIWindow;
		public string Name;
		public GameClient Client;

		public Item? FocusedItem;
		bool UpdateRequired = false;
		
		public Item this[int i]
		{
			get => InvSpace[i];
			set { InvSpace[i] = value; UpdateRequired = true; }
		}

		public Inventory(GameClient gc, Vector2 pos, string name = "inventory")
		{
			Client = gc;

			var viewport = gc.Sb.GraphicsDevice.Viewport;
			Point invui_offset = new Point(viewport.Width / 4, viewport.Height / 4);

			InvSpace = new();
			Name = name;
			InvUIWindow = Window.LabeledWindow(invui_offset, invui_offset.Mult(2), Asliipa.MainGUIColor, Name).SetPopup() as Window;
			InvUIWindow.AddElement(new ScrollList(new(0), invui_offset));
			InvUIWindow.AddElement(new ColumnList(new(invui_offset.X, 15), new(invui_offset.X, 15)));
			InvUIWindow.AddOnUpdate((ve, gt) => { if (UpdateRequired) { UpdateRequired = false; UpdateList(); }  if (InputHandler.GetKeyState(gc.GameControls.Inv) == KeyStates.JPressed) { ve.Enabled = !ve.Enabled; } });

			//Add empty elements to second column (focused item)
			var focusedcolumn = InvUIWindow[3] as ColumnList;
			focusedcolumn.AddElement(new Image(focusedcolumn.Position, Registry.TextureRegistry["zip"]));
			var header = new Label("focused item name");
			header.Position = focusedcolumn.Position;
			var desc = new Label("focused item desc");
			desc.Position = focusedcolumn.Position;
			focusedcolumn.AddElement(header);
			focusedcolumn.AddElement(desc);
			/*focusedcolumn.AddElement(new Button(focusedcolumn.Position, new(15), Asliipa.MainGUIColor, "Drop", 
				() => { }));*/

			InvUIWindow.AddToRender(gc);
		}

		public void UpdateList()
		{
			var columnlist = InvUIWindow[2] as ScrollList;
			InvUIWindow[2] = new ScrollList(columnlist.Position, columnlist.Dimension);
			columnlist = InvUIWindow[2] as ScrollList;
			for (int i = 0; i < InvSpace.Count; i++)
			{
				var button = new Button(new(columnlist.Position.X, 0), new(columnlist.Dimension.X, 15), Asliipa.MainGUIColor, InvSpace[i].Name, () => { });
				var item = InvSpace[i];
				button.OnClick = () => { SetFocusedItem(item); };
				columnlist.AddElement(button);
			}
		}
		public void SetFocusedItem(Item item)
		{
			FocusedItem = item;
			var focusedcolumn = InvUIWindow[3] as ColumnList;
			//Set focused item texture
			focusedcolumn[0] = new Image(focusedcolumn[0].Position, item.ItemTexture);
			//set the name of the item to the header
			var header = new Label(Util.WordWrap(item.Name, focusedcolumn.Dimension.X));
			header.Position = focusedcolumn[1].Position;
			focusedcolumn[1] = header;
			//set the description
			var desc = new Label(Util.WordWrap(item.Description, focusedcolumn.Dimension.X));
			desc.Position = focusedcolumn[2].Position;
			focusedcolumn[2] = desc;
			//remove the buttons provided by the previous focused item
			focusedcolumn.Elements.RemoveRange(3, focusedcolumn.Elements.Count - 3);
			//create new action buttons from item.GetActions()
			foreach(var pair in item.GetActions(this))
			{
				var button = new Button(desc.Position, new(15), Asliipa.MainGUIColor, pair.name, () => { });
				button.OnClick = pair.action;
				focusedcolumn.AddElement(button);
			}
		}
		public void AddItem(params Item[] items)
		{
			foreach(var item in items) { InvSpace.Add(item); }
			UpdateList();
		}
		public void RemoveItem(Item item)
		{
			InvSpace.Remove(item);
			UpdateRequired = true;
			//SetFocusedItem(InvSpace[0]);
		}
	}

	public class DroppedItem : Entity
	{
		public Item Item;
		public DroppedItem(Item item, Vector2 position) 
			: base(position, item.ItemTexture, item.Name, desc: item.Description)
		{
			Item = item;
		}
		public override void AddToRender(GameClient gc)
		{
			Animator.Add(new Animation(60, (int)Position.Y, (t, coeff) => 
			{
				Position.Y = coeff - Easing.Parabolic(Easing.OutBounce(t))*15;
			}
			));
			Animator.Add(new Animation(60, (int)Position.X, (t, coeff) => { Position.X = coeff + 35 * t; }));
			base.AddToRender(gc);
		}
		public override void Render(SpriteBatch sb, Vector2 offset, GameTime gt, GameClient gc)
		{
			sb.Draw(EntityTexture, Position + offset + AnchorOffset(), Color.Lerp(Color.White, Tint, .3f));
			if(ScreenCoordsHitbox(offset).Test(InputHandler.GetMousePos()))
			{
				Tint = Color.Black * Easing.Signaling((float)gt.TotalGameTime.TotalSeconds);
				if (InputHandler.LMBState() == KeyStates.JPressed)
				{
					gc.Player.Inventory.AddItem(Item);
					gc.RemoveEntity(this);
				}
			}
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
			Inventory = new(gc, Position, "Your Inventory");
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
