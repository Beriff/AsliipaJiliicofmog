using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog.Env.Item
{
	public class Item
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Composite { get; set; }
		public List<Item> ParentItems { get; set; }
		public Material Material { get; set; }


	}
}
