using AsliipaJiliicofmog.VisualElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog
{
	public class Recipe
	{
		public List<Item> RequiredItems;
		public bool Unlocked = true;
		public List<Item> Results;
		public Action<Inventory> OnCreate;

		public static Action<Inventory> RemoveIngredients(Recipe recipe) { return (inv) =>
		{
			foreach (var item in recipe.RequiredItems)
			{
				inv.RemoveItem(inv.GetItemByClone(item));
			}
		}; }

		public bool CanCreate(Inventory inv)
		{
			foreach (var item in RequiredItems)
				if (!inv.Contains(item))
					return false;
			return true;
		}

		public void Create(Inventory inv)
		{
			if (!Unlocked)
				return;
			OnCreate(inv);
			foreach (var item in Results)
				inv.AddItem(item.Clone() as Item);
		}
		public Recipe(List<Item> ingredients, List<Item> results)
		{
			RequiredItems = ingredients;
			Results = results;
		}
		public static Recipe operator + (Recipe self, Action<Inventory> oncreate)
		{
			self.OnCreate += oncreate;
			return self;
		}
	}
	public abstract class Station : Creature
	{
		public List<Recipe> Recipes;

		//Station UI
		public Window UIWindow;

		public Station(GameClient gc, Texture2D texture, string name, string desc) : base(texture, name, desc: desc)
		{
			Recipes = new();

			//Set up UI
			var viewport = gc.Sb.GraphicsDevice.Viewport;
			UIWindow = Window.LabeledWindow(new(viewport.Width / 4, viewport.Height / 2), new(viewport.Width / 2, viewport.Height / 2), Asliipa.MainGUIColor, name)
				.WithGUI(gc.ClientGUI).SetPopup() as Window;

			_OnClick += () =>
			{
				UIWindow.Enabled = !UIWindow.Enabled;
			};

		}
	}
	public abstract class StationContainer : Station
	{
		public Item[] Slot;
		public Item this[int i] { get => Slot[i]; set => Slot[i] = value; }

		public StationContainer(GameClient gc, Texture2D texture, string name, string desc) : base(gc, texture, name, desc)
		{
		}
	}
}
