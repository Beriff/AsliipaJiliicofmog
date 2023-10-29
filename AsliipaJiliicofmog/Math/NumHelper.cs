using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog.Math
{
	internal static class NumHelper
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

	static class ExtendVector
	{
		public static Vector2 Add(this Vector2 a, (float x, float y) b)
		{
			return new Vector2(a.X + b.x, a.Y + b.y);
		}
	}
}
