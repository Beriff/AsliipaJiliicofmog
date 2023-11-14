using Microsoft.Xna.Framework.Graphics;


namespace AsliipaJiliicofmog.Env.Item
{
	public class Item : ICloneable
	{
		public string Name { get; set; }
		public string Description { get; set; }

		/*
		 * If the item is marked as composite,
		 * it doesn't define its material, but rather
		 * its parent items (ex. sword has "iron edge" and "wooden handle" parent items)
		 */
		public bool Composite { get; set; }
		public List<Item> ParentItems { get; set; } = new List<Item>();
		public Material Material { get; set; }
		public Texture2D Texture { get; set; }
		public Item(string name, Texture2D texture, Material m, string desc = "no description") 
		{
			Name = name;
			Texture = texture;
			Description = desc;
			Composite = false;
			Material = m;
		}
		public Item(string name, string desc, Texture2D texture, params Item[] parentitems)
			: this(name, desc, texture, parentitems.ToList()) { }
		public Item(string name, string desc, Texture2D texture, List<Item> parentitems)
		{
			Composite = true;
			ParentItems = parentitems;
			Name = name;
			Description = desc;
			Texture = texture;
		}

		public object Clone()
		{
			if(Composite)
				return new Item(Name, Description, Texture, ParentItems);
			return new Item(Name, Texture, Material, Description);
		}

		public virtual string GetDisplayName() => Name;
	}
}
