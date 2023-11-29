using AsliipaJiliicofmog.Data;
using AsliipaJiliicofmog.Rendering.UI;

using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsliipaJiliicofmog.Env.Items
{
	public class Inventory
	{
		protected List<Item> ItemList;

		public bool HasItem(Item item) => ItemList.Contains(item);
		public bool HasItem(string name) => ItemList.Exists(x => x.Name == name);

		public Dictionary<Item,int> Reduced()
		{
			Dictionary<Item, int> dict = new();
			foreach(var item in ItemList)
				_ = dict.ContainsKey(item) ? dict[item]++ : dict[item] = 1;
			return dict;
		}

		/*public ScrollFrame GetUI(GraphicsDevice gd)
		{
			var vp = gd.Viewport.Bounds.Size.ToVector2();
			//Frame f = Frame.Window("Inventory", Registry.DefaultFont, vp / 2, vp / 2);
			//f.Pivot = new(.5f, .5f);

			ScrollFrame sf = new(vp / 2, vp / 2)
			{ Pivot = new(.5f) };
			int i = 0;

			foreach(var pair in Reduced())
			{
				var name = pair.Key.GetDisplayName() + $" x{pair.Value}";
				sf.AddElement(
					new Label(
						null,
						new(0, i * Registry.DefaultFont.MeasureString(name).Y),
						new(1,1)
					).WithText(name, Registry.DefaultFont));
				i++;
			}

			//f.AddElement(sf);
			return sf;
		}*/

		public virtual void Add(Item i) => ItemList.Add(i);

		public Inventory()
		{
			ItemList = new();
		}
	}
}
