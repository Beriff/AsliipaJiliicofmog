using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog
{
	public class Hitbox
	{
		public Point Start;
		public Point End;
		public bool Test(Hitbox b)
		{
				return (
				  Start.X <= b.End.X &&
				  End.X >= b.Start.X &&
				  Start.Y <= b.End.Y &&
				  End.Y >= b.Start.Y
				);
		
		}
		public Point Middle()
		{
			return new((Start.X + (End.X - Start.X)) / 2, (Start.Y + (End.Y - Start.Y)) / 2);
		}
		public bool Test(Point b)
		{
			return b.X > Start.X && b.X < End.X && b.Y > Start.Y && b.Y < End.Y;
		}
		public Hitbox(Point a, Point b)
		{
			Start = a;
			End = b;
		}
		public Hitbox(Rectangle a)
		{
			Start = a.Location;
			End = a.Location + a.Size;
		}
		public static Hitbox FromSize(Point a, Point b)
		{
			return new Hitbox(a, a + b);
		}
		public static Hitbox operator * (Hitbox self, int other)
		{
			return new(self.Start * new Point(other), self.End * new Point(other));
		}
		public static Hitbox operator + (Hitbox a, Point b)
		{
			return new(a.Start + b, a.End + b);
		}
		public override string ToString()
		{
			return $"[{Start} :: {End}]";
		}

		public Rectangle ToRect()
		{
			return new Rectangle(Start.X, Start.Y, End.X - Start.X, End.Y - Start.Y);
		}
	}
}
