using AsliipaJiliicofmog.VisualElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace AsliipaJiliicofmog
{
	/// <summary>
	/// Recipe is an abstract class, representing how different items can transform into
	/// other ones, bundled with additional meta-info
	/// </summary>
	public class Recipe
	{
		public List<Item> Required;
		public List<Item> Yields;
		public ListDictionary Metadata;

		public Recipe()
		{
			Metadata = new();
		}
		public Recipe WithReq(params Item[] required)
		{
			Required = new(required);
			return this;
		}
		public Recipe WithYield(params Item[] yields)
		{
			Yields = new(yields);
			return this;
		}
		public Recipe WithMetadata(params (string, string)[] meta)
		{
			foreach(var entry in meta)
			{
				Metadata.Add(entry.Item1, entry.Item2);
			}
			return this;
		}

		public bool Satisfies(List<Item> required)
		{
			foreach(var req in Required)
			{
				if (!Util.ItemIn(required, req))
					return false;
			}
			return true;
		}

		public void Craft(Inventory inv)
		{
			foreach(var item in Required)
				inv.RemoveItem(inv.GetItemByClone(item));
			foreach (var item in Yields)
				inv.AddItem(item);
		}
	}
	public abstract class Station : Prop
	{

		//Station UI
		public Window UIWindow;
		public List<Recipe> Recipes;

		public Station(GameClient gc, Texture2D texture, string name, string desc) : 
			base(gc, texture, name, desc, 1, 5, null, new ToolType[] { ToolType.Cutting })
		{

			CustomDrop = false;
			Recipes = new();
			Drops.Add(GetItem(), 1);
			//Set up UI
			var viewport = gc.Sb.GraphicsDevice.Viewport;
			UIWindow = Window.LabeledWindow(new(viewport.Width / 4, viewport.Height / 2), new(viewport.Width / 2, viewport.Height / 2), Asliipa.MainGUIColor, name)
				.WithGUI(gc.ClientGUI).SetPopup() as Window;

			_OnClick += (gc) =>
			{
				UIWindow.Enabled = !UIWindow.Enabled;
			};

		}
	}
	public abstract class StationContainer : Station
	{
		public List<Item> Slot;
		public Item this[int i] { get => Slot[i]; set => Slot[i] = value; }

		public StationContainer(GameClient gc, Texture2D texture, string name, string desc) : base(gc, texture, name, desc)
		{
			Slot = new();
			Slot.Add(null);
		}
	}
}
