using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog
{
	static class NumExtend
	{
		public static Vector2 Normalized(this Vector2 a)
		{
			var b = a;
			b.Normalize();
			return b;
		}
		public static Rectangle Vec2Rect(Vector2 b, Vector2 c) => new Rectangle(b.ToPoint(), c.ToPoint());
		public static Vector2 Rotated(this Vector2 a, double theta)
		{
			return new((float)(a.X * Math.Cos(theta) - a.Y * Math.Sin(theta)), (float)(a.X * Math.Sin(theta) + a.Y * Math.Cos(theta)));
		}
		public static Vector2 PointTo(this Vector2 a, Vector2 b)
		{
			return b - a;
		}
		public static float Distance(this Vector2 a, Vector2 b) => a.PointTo(b).Length();
		public static float Lerp(float v0, float v1, float t)
		{
			return v0 + t * (v1 - v0);
		}
		public static Vector2 Lerp(Vector2 v0, Vector2 v1, float t)
		{
			return new(Lerp(v0.X, v1.X, t), Lerp(v0.Y, v1.Y, t));
		}
		public static float FloatRange(this Random r, float min, float max)
		{
			return Lerp(min, max, (float)r.NextDouble());
		}
		public static int Flatten(int x, int y, int w) => w * y + x;
		public static void XY(int endx, int endy, int startx, int starty, Action<int,int> act)
		{
			for(int x = startx; x < endx; x++)
			{
				for (int y = starty; y < endy; y++)
					act(x, y);
			}
		}
		public static void XY(int endx, int endy, Action<int,int> act) => XY(endx, endy, 0, 0, act);
	}
}
