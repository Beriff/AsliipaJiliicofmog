using AsliipaJiliicofmog.Data;
using AsliipaJiliicofmog.Rendering.UI;

namespace AsliipaJiliicofmog.Env.Items
{
	public class Inventory
	{
		protected List<Item> ItemList;
		public string Owner { get; set; }

		public bool HasItem(Item item) => ItemList.Contains(item);
		public bool HasItem(string name) => ItemList.Exists(x => x.Name == name);

		public Dictionary<Item,int> Reduced()
		{
			Dictionary<Item, int> dict = new();
			foreach(var item in ItemList)
				_ = dict.ContainsKey(item) ? dict[item]++ : dict[item] = 1;
			return dict;
		}

		/// <summary>
		/// Construct a new UI with basic inventory functionality
		/// (displaying items, and being able to remove and equip items)
		/// </summary>
		public Frame GetUI()
		{
			var sf = new Scrollframe(
				new (new(0), new(0,15), new(0,-15), new(1,1) ));

			var sf_list = new ListLayout(DimUI.Full, name: Owner + "-inventory-llayout");

			var w = Frame.Window(Owner + " (inv)", DimUI.Global(new(.5f), new(.5f)));
			w.Pivot = new(.5f);
			w.Name = Owner + "-inventory";

			w.Add(sf);
			sf.Add(sf_list);

			foreach( (Item item, int count) in Reduced() )
			{
				sf_list.Add(
					new Label(
						$"{item.Name} x{count}", 
						DimUI.Global(new(0), new(1, .2f)),
						$"inv-label-{item.Name}"
						));
			}

			return w;
		}

		public virtual void Add(Item i) => ItemList.Add(i);

		public Inventory(string owner)
		{
			ItemList = new();
			Owner = owner;
		}
	}
}
