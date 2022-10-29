using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using AsliipaJiliicofmog.VisualElements;

using System;
using Microsoft.Xna.Framework.Audio;

namespace AsliipaJiliicofmog
{
	public class Asliipa : Game
	{
		public static Random Random = new();
		public const string TEXTUREPATH = @"C:\Users\Maxim\Desktop\assets\textures";

		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		public Scene scene;
		public Vector2 offset;
		public GameClient Client;
		Window MainMenu;
		Window Settings;
		public static Color MainGUIColor = new(50, 50, 50, 137);

		public static SpriteFont GameFont;

		//when UI is focused, dont move camera with mouse drag, apply drag to sliders and other UI elements instead
		public static bool UIFocused = false;

		public static void SetUIFocus(bool val)
		{
			UIFocused = val;
			if (!val)
				InputHandler.MouseDragOffset = new(0);
		}
		public Asliipa()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			UIFocused = false;
		}

		protected override void Initialize()
		{
			

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			GUI.Init(GraphicsDevice);
			Registry.LoadTextureDirectory(TEXTUREPATH, GraphicsDevice);
			GameFont = Content.Load<SpriteFont>("GameFont");
			GameAudio.SFX["click"] = Content.Load<SoundEffect>("click");


			scene = new Scene(new Tile("dirt", Registry.TextureRegistry["dirt"]));
			Client = new GameClient(scene, _spriteBatch);

			ParticleEmitter.Circle(new(25), 300, 10).AddToRender(Client);
			new Creature(new(25), Registry.TextureRegistry["crate"], "crate").AddToRender(Client);
			//Animator.Add(new(500, 0, (a, b) => { (scene.Entities[0] as ParticleEmitter).Radius -= 1; }));

			

			

			//Setup GUI
			{
				var viewport = GraphicsDevice.Viewport;
				var width = viewport.Width;
				var height = viewport.Height;
				Point size = new(width / 2, height / 2);

				Settings = VisualElements.Window.LabeledWindow(new(width / 4, height / 4), size, MainGUIColor, "Settings").SetPopup() as Window;
				Settings.AddOnUpdate((ve, gt) =>
				{
					if (InputHandler.GetKeyState(Keys.Escape) == KeyStates.JPressed)
					{
						Settings.Enabled = false;
					}
				});


				var settingsColumn = new ColumnList(new(width / 4, height / 10), new(0));
				var srow1 = new RowList(new(settingsColumn.Position.X, 0), new(width / 2, 0));
				var srow2 = new RowList(new(settingsColumn.Position.X, 0), new(width / 2, 0));
				Settings.AddElement(settingsColumn);
				settingsColumn.AddElement(srow1);
				settingsColumn.AddElement(srow2);

				var masterVolumeSlider = new Slider(MainGUIColor, new(0, 10), new(0), (self) => { }).SetAppear() as Slider;
				srow1.AddElement(masterVolumeSlider);
				var masterVolumeText = new Label($"Master Volume: {masterVolumeSlider.Percent}").SetAppear() as Label;
				masterVolumeSlider.OnChange = (self) =>
				{
					GameAudio.Volume = self.Percent;
					GameAudio.Play("click");
					masterVolumeText.Text = $"Master Volume: {Math.Round(masterVolumeSlider.Percent * 100)}/100";
				};
				srow1.AddElement(masterVolumeText);
				var resolutionChoice = new Dropdown(MainGUIColor, "Choose...", new(0), new(0, 10), "1920x1080", "1600x900");
				var resolutionText = new Label("Window Resolution");
				srow2.AddElement(resolutionChoice);
				srow2.AddElement(resolutionText);



				MainMenu = VisualElements.Window.LabeledWindow(new(width / 4, height / 4), size, MainGUIColor, "Main Menu").SetPopup() as Window;
				MainMenu.AddOnUpdate((ve, gt) =>
				{
					if (InputHandler.GetKeyState(Keys.Escape) == KeyStates.JPressed)
					{
						MainMenu.Enabled = !MainMenu.Enabled;
					}
				});



				var mainmenuButtons = new ColumnList(new(width / 2 - width / 4, height / 10), new(0));
				MainMenu.AddElement(mainmenuButtons);

				var SettingsButton = new Button(new(width / 2, 0), new(1, 20), MainGUIColor, "Settings", () => { MainMenu.Enabled = false; Settings.Enabled = true; }).SetAppear() as Button;
				SettingsButton.Position = new(SettingsButton.Position.X - (int)(SettingsButton.StringDim.X / 2), SettingsButton.Position.Y);
				mainmenuButtons.AddElement(SettingsButton);

				var ExitGameButton = new Button(new(width / 2, 0), new(1, 20), MainGUIColor, "Exit Game", () => { Exit(); }).SetAppear() as Button;
				ExitGameButton.Position = new(ExitGameButton.Position.X - (int)(ExitGameButton.StringDim.X / 2), ExitGameButton.Position.Y);
				mainmenuButtons.AddElement(ExitGameButton);


				MainMenu.Enabled = false;
				MainMenu.AddToRender(Client);
				Settings.Enabled = false;
				Settings.AddToRender(Client);
			}
			
		}

		protected override void Update(GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.Z))
				GUI.GUIDEBUG = !GUI.GUIDEBUG;

			
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			
			_spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			Client.Update(_spriteBatch, gameTime);
			_spriteBatch.DrawString(GameFont, Math.Ceiling(1d / gameTime.ElapsedGameTime.TotalSeconds).ToString() + " fps", Vector2.Zero, Color.White);
			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
