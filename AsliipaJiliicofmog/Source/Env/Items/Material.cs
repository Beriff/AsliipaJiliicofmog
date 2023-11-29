using Microsoft.Xna.Framework;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;

namespace AsliipaJiliicofmog.Env.Items
{
	public enum MaterialType
	{
		Organic,
		Metal
	}

	public class GameWorldException : Exception
	{
		public GameWorldException() { }
		public GameWorldException(string message) : base(message) { }
		public GameWorldException(string message, Exception innerException) : base(message, innerException) { }
	}

	public class Material
	{
		public MaterialType Type { get; set; }
		public string Name { get; set; }
		public float Density { get; set; }
		public Color Color { get; set; }
		public float Strength { get; set; }
		public float MeltingPoint { get; set; }
		public bool Composite { get; set; }
		public (Material m1, Material m2)? ParentMaterials { get; set; }

		public Material(MaterialType mt, string name, float density, Color c, float strength, float mpoint = 0)
		{
			Type = mt;
			Name = name;
			Density = density;
			Color = c;
			Strength = strength;
			MeltingPoint = mpoint;
			Composite = false;
		}

		public static Material operator +(Material a, Material b)
		{
			if (a.Type != b.Type)
				throw new GameWorldException("Cannot combine materials of different type");
			return new(a.Type, "", a.Density * b.Density, Color.Lerp(a.Color, b.Color, .5f),
				a.Strength * b.Strength, (a.MeltingPoint * b.MeltingPoint) / (a.MeltingPoint + b.MeltingPoint));
		}

		public static Material Deserialize(string json)
		{
			dynamic obj = JsonConvert.DeserializeObject(json);
			List<int> l = obj.Color.ToObject<List<int>>();
			return new(
				Enum.Parse(typeof(MaterialType), obj.Element.ToString()),
				obj.Name.ToString(),
				float.Parse(obj.Density.ToString()),
				new Color(l[0], l[1], l[2]),
				float.Parse(obj.Strength.ToString()),
				float.Parse(obj.MeltingPoint.ToString())
			)
			{ Composite = false };
		}
	}
}
