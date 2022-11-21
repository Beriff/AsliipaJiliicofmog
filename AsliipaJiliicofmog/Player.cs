using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using AsliipaJiliicofmog.VisualElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AsliipaJiliicofmog
{
	public class Item : ICloneable
	{
		public Texture2D ItemTexture;
		protected string _Name;
		protected string _Description;
		public virtual string Name { get => _Name; set => _Name = value; }
		public virtual string Description { get => _Description; set => _Description = value; }

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

		public virtual object Clone()
		{
			return new Item(ItemTexture, Name, Description);
		}
		public override string ToString()
		{
			return Name;
		}
		public bool IsCloneOf(Item other)
		{
			return Name == other.Name && Description == other.Description && ItemTexture == other.ItemTexture;
		}
	}
	public class Equippable : Item
	{
		public Vector2 Anchor;
		public Action<GameClient> OnEquip = (gc) => { };
		public Action<GameClient> OnUnequip = (gc) => { };
		public Action<GameClient> EquippedUpdate = (gc) => { };
		public Equippable(Texture2D texture, string name, string desc, Vector2? anchor = null) : base(texture, name, desc)
		{
			Anchor = anchor ?? new(0, 0);
		}

		public override List<(string name, Action action)> GetActions(Inventory inv)
		{
			return new()
			{
				new("Drop", () =>
				{
					inv.RemoveItem(inv.FocusedItem);
					new DroppedItem(inv.FocusedItem, inv.Client.Player.Position).AddToRender(inv.Client);
					if (inv.Client.Player.Equipped == this)
						inv.Client.Player.Equipped = null;
				}),
				new("Equip", () =>
				{
					if (inv.Client.Player.Equipped == this)
					{
						inv.Client.Player.Equipped = null;
						OnUnequip(inv.Client);
					}
					else
					{
						inv.Client.Player.Equipped = this;
						OnEquip(inv.Client);
					}
						
				})
			};
		}
	}
	public class Placeable : Equippable
	{
		public Texture2D AvailableTexture;
		public Texture2D BlockedTexture;
		public Entity PlaceableEntity;
		public Placeable(Entity entity) : base(entity.EntityTexture.Default, entity.Name, entity.Description, entity.Anchor)
		{
			PlaceableEntity = entity;
			AvailableTexture =
				Util.ChangeTexture(entity.EntityTexture.Default,
				Util.ColorOpaque(new Color(Color.Green, .5f), entity.EntityTexture.Default.GraphicsDevice));
			BlockedTexture =
				Util.ChangeTexture(entity.EntityTexture.Default,
				Util.ColorOpaque(new Color(Color.Red, .5f), entity.EntityTexture.Default.GraphicsDevice));

			EquippedUpdate = (gc) =>
			{
				Vector2 shift = ItemTexture.Size() / 2;


				var worldpos = gc.ScreenCoords2World(InputHandler.GetMousePos(), ItemTexture.GraphicsDevice);

				//snap to grid if CTRL is held
				//if(InputHandler.GetKeyState(Keys.LeftControl) == KeyStates.Hold)
					//worldpos = new((int)Math.Ceiling(worldpos.X / Tile.TextureSize), (int)Math.Ceiling(worldpos.Y / Tile.TextureSize));

				//Draw texture according to whether the position os obstructed
				if (gc.EntityProcessor.Obstructed(Hitbox.FromSize(worldpos, ItemTexture.Size())))
					gc.Sb.Draw(BlockedTexture, InputHandler.GetMousePos(), Color.White);
				else
				{
					gc.Sb.Draw(AvailableTexture, InputHandler.GetMousePos(), Color.White);
					var state = InputHandler.LMBState();
					if (state == KeyStates.Hold)
					{
						var equipped = gc.Player.Equipped;
						var entity = (equipped as Placeable).PlaceableEntity;
						entity.AddToRender(gc);
						entity.Move(gc, worldpos - entity.Position);
						equipped.OnUnequip(gc);
						gc.Player.Equipped = null;
						gc.Player.Inventory.RemoveItem(equipped);
						
					}
				}
			};
		}
	}
	public class Usable : Item
	{
		public Action<GameClient> OnUse;
		public Usable(Texture2D texture, string name, string desc, Action<GameClient> onuse) : base(texture, name, desc)
		{
			OnUse = onuse;
		}
		public override List<(string name, Action action)> GetActions(Inventory inv)
		{
			return new()
			{
				new("Drop", () =>
				{
					inv.RemoveItem(inv.FocusedItem);
					new DroppedItem(inv.FocusedItem, inv.Client.Player.Position).AddToRender(inv.Client);
				}),
				new("Use", () =>
				{
					OnUse(inv.Client);
				})
			};
		}
		public static Usable NewFood(Texture2D texture, string name, string desc, float satiate)
		{
			return new(texture, name, desc, (gc) =>
			{
				var inv = gc.Player.Inventory;
				gc.Player.Hunger += satiate;
				inv.RemoveItem(inv.FocusedItem);
			});
		}
		public override object Clone()
		{
			return new Usable(ItemTexture, Name, Description, OnUse);
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

		public bool Contains(Item target)
		{
			foreach (var item in InvSpace)
				if (!item.IsCloneOf(target))
					return false;
			return true;
		}
		public Item GetItemByClone(Item clone)
		{
			foreach(var item in InvSpace)
				if(clone.IsCloneOf(item))
					return item;
			return null;
		}
		
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
			InvUIWindow = Window.LabeledWindow(invui_offset, invui_offset.Mult(2), Asliipa.MainGUIColor, Name).WithGUI(gc.ClientGUI).SetPopup() as Window;
			InvUIWindow.AddElement(new ScrollList(new(0), invui_offset));
			InvUIWindow.AddElement(new ColumnList(new(invui_offset.X, 15), new(invui_offset.X, 15)));
			InvUIWindow.AddOnUpdate((ve, gt) => { if (UpdateRequired) { UpdateRequired = false; UpdateList(); }  if (InputHandler.GetKeyState(gc.GameControls.Inv) == KeyStates.JPressed) { ve.Enabled = !ve.Enabled; } });

			//Add empty elements to second column (focused item)
			var focusedcolumn = InvUIWindow[3] as ColumnList;
			focusedcolumn.Padding = 1;
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
			InvUIWindow.Enabled = false;
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
				var button = new Button(desc.Position, new(20), Asliipa.MainGUIColor, pair.name, () => { });
				button.OnClick = pair.action;
				focusedcolumn.AddElement(button);
			}
		}
		public void AddItem(params Item[] items)
		{
			foreach(var item in items) { InvSpace.Add(item); }
			UpdateList();
		}
		/// <summary>
		/// Removes the given item. Use with GetItemByClone() if you don't have direct
		/// item reference
		/// </summary>
		/// <param name="item">Item reference</param>
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
			//define random direction
			var dir = new Vector2((float)Asliipa.Random.NextDouble(), (float)Asliipa.Random.NextDouble());
			dir = dir * 2 - Vector2.One;
			Animator.Add(new Animation(60, (int)Position.Y, (t, coeff) => 
			{
				Position.Y = coeff - Easing.Parabolic(Easing.OutBounce(t)) * dir.Y * 15;
			}
			));
			Animator.Add(new Animation(60, (int)Position.X, (t, coeff) => { Position.X = coeff + 35 * t * dir.X; }));
			base.AddToRender(gc);
		}
		public override void Render(SpriteBatch sb, Vector2 offset, GameTime gt, GameClient gc)
		{
			EntityTexture.Render(sb, Position + offset + AnchorOffset(), gt, Color.Lerp(Color.White, Tint, .3f));
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
		public Equippable Equipped;

		//Player stats
		public float Hunger = 100; // out of 100
		public int Level = 1;
		public int Exp = 0;

		private int _hashed_exp_req = 11;

		public float GetExpRequired()
		{
			if (_hashed_exp_req != 0)
				return (float)Math.Pow(Level, Math.Pow(Level, 0.4f));
			return _hashed_exp_req;
		}

		public override Action<GameClient> OnUpdate
		{
			get => base.OnUpdate; set
			{
				_OnUpdate = Behavior.PlayerController(this) + value;
				_OnUpdate += (gc) => { Hunger -= 0.1f; };
			}
		}
		public Player(Texture2D texture, string name, GameClient gc) : base(texture, name, pos: new(70))
		{
			Description = "Controllable creature";
			GenerateInfobox();
			Speed = 2;
			Inventory = new(gc, Position, "Your Inventory");
		}
		public override void Render(SpriteBatch sb, Vector2 offset, GameTime gt, GameClient gc)
		{
			
			base.Render(sb, offset, gt, gc);
			//render the equipped item
			if (Equipped != null)
				sb.Draw(Equipped.ItemTexture, offset + Position + Position * Anchor, Color.White);
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
