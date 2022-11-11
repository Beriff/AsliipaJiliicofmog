using AsliipaJiliicofmog.VisualElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AsliipaJiliicofmog
{
	public class Controls
	{
		public Keys Up = Keys.W;
		public Keys Down = Keys.S;
		public Keys Left = Keys.A;
		public Keys Right = Keys.D;
		public Keys Esc = Keys.Escape;
		public Keys Inv = Keys.E;
	}
	//Helps to render entities based on their Y position
	//So if a player walks behind a crate, the crate will be rendered over the player
	//And if the player stands before the crate, the player will be rendered first
	public class OrderedEntityProcessor
	{
		public List<Entity> Entities = new();
		public void Clear()
		{
			Entities = new();
		}
		public void Process(SpriteBatch sb, Vector2 offset, GameClient gc, GameTime gt)
		{
			var orderedEntities = Entities.OrderBy(e => e.Position.Y).ToList();
			foreach(var entity in orderedEntities) { entity.DrawShadow(sb, offset); }
			foreach(var entity in orderedEntities) { entity.OnUpdate(gc); if (entity.RenderEnabled) { entity.Render(sb, offset, gt, gc); } }
		}
	}
	public class GameClient
	{
		public Scene FocusedScene;
		public Vector2 Camera = Vector2.Zero;
		public List<VisualElements.VisualElement> VElements;
		public List<Action<SpriteBatch, GameTime>> OnUpdateEvents;
		public List<Action> OnLClickEvents;
		public SpriteBatch Sb;
		public Controls GameControls = new();
		public OrderedEntityProcessor EntityProcessor = new();

		public bool ExitRequested = false;
		public GUI ClientGUI = new();

		public Player Player;

		public void AddUpdateEvent(Action<SpriteBatch, GameTime> _event)
		{
			OnUpdateEvents.Add(_event);
		}
		public List<Entity> EntityList()
		{
			return EntityProcessor.Entities;
		}

		public Point GetViewport() => new(Sb.GraphicsDevice.Viewport.Width, Sb.GraphicsDevice.Viewport.Height);

		public void Update(SpriteBatch sb, GameTime gt)
		{
			FocusedScene.Update(this);
			Animator.Update();
			FocusedScene.Render(sb, Camera, gt, this);
			foreach (VisualElement ve in VElements)
			{
				ve.Update(ve, gt);
				if(ve.Enabled)
					ve.Render(sb);
			}
			InputHandler.Update(this, Keyboard.GetState(), Mouse.GetState());
		}
		public void AddEntity(Entity e)
		{
			EntityProcessor.Entities.Add(e);
		}

		public Tile? ScreenCoords2Tile(Vector2 cursorpos, GraphicsDevice gd)
		{
			//convert topleft coordinate system to centered
			Viewport vp = gd.Viewport;
			cursorpos -= new Vector2(vp.Width / 2, vp.Height / 2);

			//account for camera shift
			cursorpos += Camera;

			if(cursorpos.X < 0 || cursorpos.Y < 0 || cursorpos.X > Scene.SceneSizePixels - Tile.TextureSize || cursorpos.Y > Scene.SceneSizePixels - Tile.TextureSize)
			{
				return null;
			} else
			{
				return FocusedScene.GetAt((int)Math.Ceiling(cursorpos.X / Tile.TextureSize), (int)Math.Ceiling(cursorpos.Y / Tile.TextureSize));
			}
		}
		protected Window GetSidebar() => VElements[2] as Window;
		public void CreateGUI()
		{
			//Setup main menu & settings GUI
			{
				var viewport = Sb.GraphicsDevice.Viewport;
				var width = viewport.Width;
				var height = viewport.Height;
				Point size = new(width / 2, height / 2);

				var settings = Window.LabeledWindow(new(width / 4, height / 4), size, Asliipa.MainGUIColor, "Settings").WithGUI(ClientGUI).SetPopup() as Window;
				settings.AddOnUpdate((ve, gt) =>
				{
					if (InputHandler.GetKeyState(Keys.Escape) == KeyStates.JPressed)
					{
						settings.Enabled = false;
					}
				});


				var settingsColumn = new ColumnList(new(width / 4, height / 10), new(0));
				var srow1 = new RowList(new(settingsColumn.Position.X, 0), new(width / 2, 0));
				var srow2 = new RowList(new(settingsColumn.Position.X, 0), new(width / 2, 0));
				settings.AddElement(settingsColumn);
				settingsColumn.AddElement(srow1);
				settingsColumn.AddElement(srow2);

				var masterVolumeSlider = new Slider(Asliipa.MainGUIColor, new(0, 10), new(0), (self) => { }).SetAppear() as Slider;
				srow1.AddElement(masterVolumeSlider);
				var masterVolumeText = new Label($"Master Volume: {masterVolumeSlider.Percent}").SetAppear() as Label;
				masterVolumeSlider.OnChange = (self) =>
				{
					GameAudio.Volume = self.Percent;
					GameAudio.Play("click");
					masterVolumeText.Text = $"Master Volume: {Math.Round(masterVolumeSlider.Percent * 100)}/100";
				};
				srow1.AddElement(masterVolumeText);
				var resolutionChoice = new Dropdown(Asliipa.MainGUIColor, "Choose...", new(0), new(0, 10), "1920x1080", "1600x900");
				var resolutionText = new Label("Window Resolution");
				srow2.AddElement(resolutionChoice);
				srow2.AddElement(resolutionText);



				var mainMenu = Window.LabeledWindow(new(width / 4, height / 4), size, Asliipa.MainGUIColor, "Main Menu").WithGUI(ClientGUI).SetPopup() as Window;
				mainMenu.AddOnUpdate((ve, gt) =>
				{
					if (InputHandler.GetKeyState(Keys.Escape) == KeyStates.JPressed)
					{
						mainMenu.Enabled = !mainMenu.Enabled;
					}
				});



				var mainmenuButtons = new ColumnList(new(width / 2 - width / 4, height / 10), new(0));
				mainMenu.AddElement(mainmenuButtons);

				var SettingsButton = new Button(new(width / 2, 0), new(1, 20), Asliipa.MainGUIColor, "Settings", () => { mainMenu.Enabled = false; settings.Enabled = true; }).SetAppear() as Button;
				SettingsButton.Position = new(SettingsButton.Position.X - (int)(SettingsButton.StringDim.X / 2), SettingsButton.Position.Y);
				mainmenuButtons.AddElement(SettingsButton);

				var ExitGameButton = new Button(new(width / 2, 0), new(1, 20), Asliipa.MainGUIColor, "Exit Game", () => { ExitRequested = true; }).SetAppear() as Button;
				ExitGameButton.Position = new(ExitGameButton.Position.X - (int)(ExitGameButton.StringDim.X / 2), ExitGameButton.Position.Y);
				mainmenuButtons.AddElement(ExitGameButton);


				mainMenu.Enabled = false;
				mainMenu.AddToRender(this);
				settings.Enabled = false;
				settings.AddToRender(this);
			}

			//Setup sidebar gui
			{
				//Create sidebar with selected tile/entity info
				var sidebarsize = GetViewport();
				sidebarsize.X /= 5;
				//init the sidebar right behind the window
				var sidebar = Window.LabeledWindow(new(-sidebarsize.X, 0), sidebarsize, Asliipa.MainGUIColor, "");
				//set sidebar button to move it outside the window instead
				(sidebar[0] as Button).OnClick = () => { ToggleSidebar(); };
				//column to hold other elements
				var column = new ColumnList(new(-sidebarsize.X - sidebar.Position.X, 0), new(sidebar.Position.X / 2, 0));
				sidebar.AddElement(column);
				//image to hold targeted tile/entity texture
				var texture = new Image(new(sidebar.Position.X / 2, 0), new Texture2D(Sb.GraphicsDevice, Tile.TextureSize, Tile.TextureSize), Tile.TextureSize);
				column.AddElement(texture);
				var header = new Label("header");
				header.Position = new(column.Position.X, 0);
				var desc = new Label("description");
				desc.Position = new(column.Position.X, 0);
				column.AddElement(header);
				column.AddElement(desc);
				sidebar.AddToRender(this);
				sidebar.AddOnUpdate((ve, gt) => { if (InputHandler.GetKeyState(Keys.F) == KeyStates.JPressed) { ToggleSidebar(); } });
			}
		}
		public void UpdateSidebar(Texture2D texture, string header, string desc)
		{
			var column = GetSidebar()[2] as ColumnList;
			var newImage = new Image(column.Position, texture);
			column[0] = newImage;
			(column[1] as Label).Text = Util.WordWrap(header, GetSidebar().Dimension.X / 2);
			(column[2] as Label).Text = Util.WordWrap(desc, GetSidebar().Dimension.X / 2);
		}

		public void ToggleSidebar()
		{
			var sidebarsize = GetSidebar().Dimension;
			var posx = GetSidebar().Position.X;
			if(posx < 0)
				Animator.Add(new Animation(30, posx, (t, coeff) => { GetSidebar().Position = new((int)(Easing.OutExpo(t) * sidebarsize.X + coeff), 0); }, "sidebar"));
			else
				Animator.Add(new Animation(30, posx, (t, coeff) => { GetSidebar().Position = new((int)(coeff - (Easing.OutExpo(t) * sidebarsize.X)), 0); }, "sidebar"));
		}
		public void RemoveEntity(Entity e)
		{
			EntityProcessor.Entities.Remove(e);
		}

		public GameClient(Scene scene, SpriteBatch sb)
		{
			InputHandler.ActiveGameClient = this;
			FocusedScene = scene;
			VElements = new();
			OnLClickEvents = new();
			OnUpdateEvents = new();
			Sb = sb;
			CreateGUI();
			var player = new Player(Registry.TextureRegistry["dummy"], "Ben Dover", this);
			player.AddToRender(this);
			Player = player;
			player.Inventory.AddItem(new Tool(Registry.TextureRegistry["axe"], "stone axe", "tree cutting tool and a sharp weapon").WithTypes(ToolType.Cutting));
			//Registry.GetCreature("zip jr").AddToRender(this);
			//for(int i = 0; i < 20; i++)
			//player.Inventory.AddItem(new Item(Registry.TextureRegistry["zip"], "yea", "yea"));

			var tree = new Prop(this, Registry.TextureRegistry["tree"], "tree", "ordinary tree", 1, 5, new(.5f, -.7f), new ToolType[] { ToolType.Cutting });
			tree.Drops.Add(new Item(Registry.TextureRegistry["log"], "log", "chopped log of a tree"), 1);
			tree.Move(this, new(64*4));
			tree.AddToRender(this);
		}
	}
}
