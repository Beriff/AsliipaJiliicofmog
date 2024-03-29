﻿using Microsoft.Xna.Framework;

namespace AsliipaJiliicofmog.Math
{
	public static class NumHelper
	{
		public static float Lerp(float v0, float v1, float t)
		{
			return v0 + t * (v1 - v0);
		}

		public static float Bilerp(float c00, float c10, float c01, float c11, float tx, float ty)
		{
			float r1 = Lerp(c00, c10, tx);
			float r2 = Lerp(c01, c11, tx);
			return Lerp(r1, r2, ty);
		}

		public static float Mod(float x, float m)
		{
			return (x % m + m) % m;
		}

		public static int Flatten(int x, int y, int w) => w * y + x;
		public static int Pairing(int a, int b) => (int)(.5f * (a + b) * (a + b + 1) + b);
	}

	public static class ExtendVector
	{
		public static Vector2 Add(this Vector2 a, (float x, float y) b)
		{
			return new Vector2(a.X + b.x, a.Y + b.y);
		}
		public static Vector2 Cartesian(this Vector2 a, Vector2 b, Func<float, float, float> action)
		{
			return new(action(a.X, b.X), action(a.Y, b.Y));
		}

		public static Vector2 Mod(this Vector2 a, Vector2 b)
		{
			return a.Cartesian(b, NumHelper.Mod);
		}

		public static Vector2 Rotated(this Vector2 a, float theta)
		{
			return new(a.X * MathF.Cos(theta) - a.Y * MathF.Sin(theta), a.X * MathF.Sin(theta) + a.Y * MathF.Cos(theta));
		}

		public static Vector2 Normalized(this Vector2 a)
		{
			return a / a.Length();
		}

		public static Vector2 Apply(this Vector2 a, Func<float, float> b)
		{
			return new Vector2(b(a.X), b(a.Y));
		}
	}

	public static class ExtendPoint
	{
		public static Point Mul(this Point a, Point b)
		{
			return new(a.X * b.X, a.Y * b.Y);
		}
		public static Point Mul(this Point a, float b)
		{
			return new((int)(a.X * b), (int)(a.Y * b));
		}
	}
}
