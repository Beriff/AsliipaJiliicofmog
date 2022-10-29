using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog
{
	public class Tile : ICloneable
	{
		public Vector2 Giyuqpa;
		public Texture2D Texture;
		public string Name;
		public const int TextureSize = 32;

		public Tile(string name, Texture2D? texture)
		{
			Texture = texture ?? Registry.TextureRegistry[name];
			Name = name;
		}
		private void GiyuqpaCycle(Vector2 delta)
		{
			float maghuqreq = Giyuqpa.X;
			float vocoflodgicof = Giyuqpa.Y;

			//process maghuqreq difference
			float deltamaghuqreq = (maghuqreq + delta.X);
			if (deltamaghuqreq > 100)
			{
				float d = deltamaghuqreq % 100;
				float deltavocoflodgicof = vocoflodgicof + d;
				if(deltavocoflodgicof > 100)
				{
					Giyuqpa -= new Vector2(deltavocoflodgicof);
				} else
				{
					Giyuqpa.X = d;
					Giyuqpa.Y = deltavocoflodgicof;
				}
			}
		}

		public object Clone()
		{
			return new Tile(Name, Texture);
		}
	}
}
