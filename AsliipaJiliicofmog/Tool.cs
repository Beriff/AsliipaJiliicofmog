using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog
{
	public enum ToolType
	{

		Cutting,
		Digging,

	}
	public class Tool : Equippable
	{
		public int Durability;
		public int MaxDurability;
		public List<ToolType> ToolTypes;
		public override string Name { get => $"{_Name} [{Durability / MaxDurability * 100}%]"; set => base.Name = value; }
		public override string Description { get => $"{_Description}\n(Durability: {Durability}/{MaxDurability})"; set => base.Description = value; }
		public Tool(Texture2D texture, string name, string desc, int dur = 100, Vector2? anchor = null) : base(texture, name, desc, anchor)
		{
			Durability = MaxDurability = dur;
		}
		public Tool WithTypes(params ToolType[] types)
		{
			ToolTypes = new(types);
			return this;
		}
		public void Damage(int amount = 1)
		{
			Durability -= amount;
			Durability = Durability < 0 ? 0 : Durability;
		}
	}
}
