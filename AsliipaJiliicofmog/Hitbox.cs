using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog
{
	public class Hitbox
	{
		public Vector2 Start;
		public Vector2 End;
		public bool Test(Hitbox b)
		{
				return (
				  Start.X <= b.End.X &&
				  End.X >= b.Start.X &&
				  Start.Y <= b.End.Y &&
				  End.Y >= b.Start.Y
				);
		
		}
		public Vector2 Middle()
		{
			return new(Start.X + (End.X - Start.X) / 2 , Start.Y + (End.Y - Start.Y) / 2 );
		}
		public bool Test(Vector2 b)
		{
			return b.X > Start.X && b.X < End.X && b.Y > Start.Y && b.Y < End.Y;
		}
		public Hitbox(Vector2 a, Vector2 b)
		{
			Start = a;
			End = b;
		}
		public Hitbox(Rectangle a)
		{
			Start = a.Location.ToVector2();
			End = a.Location.ToVector2() + a.Size.ToVector2();
		}
		public static Hitbox FromSize(Vector2 a, Vector2 b)
		{
			return new Hitbox(a, a + b);
		}
		public static Hitbox FromSize(Point a, Point b)
		{
			return new Hitbox(a.ToVector2(), a.ToVector2() + b.ToVector2());
		}
		public static Hitbox operator * (Hitbox self, int other)
		{
			return new(self.Start * new Vector2(other), self.End * new Vector2(other));
		}
		public static Hitbox operator + (Hitbox a, Vector2 b)
		{
			return new(a.Start + b, a.End + b);
		}
		public override string ToString()
		{
			return $"[{Start} :: {End}]";
		}

		public Rectangle ToRect()
		{
			return new Rectangle((int)Start.X, (int)Start.Y, (int)(End.X - Start.X), (int)(End.Y - Start.Y));
		}

		public Hitbox Clone()
		{
			return new Hitbox(Start, End);
		}

		public Vector2 RandomInside()
		{
			return new Vector2(Asliipa.Random.Next((int)Start.X, (int)End.X), Asliipa.Random.Next((int)Start.Y, (int)End.Y));
		}
	}
}
