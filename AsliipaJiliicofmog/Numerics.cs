using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog
{
	enum Orientation
	{
		South,
		North,
		West,
		East
	}
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
	static class TextureExtend
	{
		public static Texture2D Copy(this Texture2D texture)
		{
			int w = texture.Width;
			int h = texture.Height;
			Texture2D blanket = new Texture2D(texture.GraphicsDevice, w, h);
			Color[] data = new Color[w * h];
			texture.GetData<Color>(data);
			blanket.SetData(data);
			return blanket;
		}
		public static Texture2D Mirror(this Texture2D texture, bool vertical = false)
		{
			int w = texture.Width;
			int h = texture.Height;

			Texture2D blanket = new Texture2D(texture.GraphicsDevice, w, h);
			Color[] newdata = new Color[w * h];
			Color[] olddata = new Color[w * h];
			texture.GetData<Color>(olddata);

			if(!vertical) //aka horizontal
			{
				NumExtend.XY(w, h, (x, y) =>
				{
					newdata[NumExtend.Flatten(x, y, w)] = olddata[NumExtend.Flatten(w - x, y, w)];
				});
			} else
			{
				NumExtend.XY(w, h, (x, y) =>
				{
					newdata[NumExtend.Flatten(x, y, w)] = olddata[NumExtend.Flatten(x, h - y, w)];
				});
			}
			blanket.SetData(newdata);
			return blanket;
			
		}
		public static Texture2D Rotate90(this Texture2D texture)
		{
			int w = texture.Width;
			int h = texture.Height;

			Texture2D blanket = new Texture2D(texture.GraphicsDevice, w, h);
			Color[] newdata = new Color[w * h];
			Color[] olddata = new Color[w * h];
			texture.GetData<Color>(olddata);

			NumExtend.XY(w, h, (x, y) =>
			{
				newdata[NumExtend.Flatten(x, y, w)] = olddata[NumExtend.Flatten(y,x,w)];
			});
			blanket.SetData(newdata);
			return blanket;
		}
		public static Texture2D Rotate(this Texture2D texture, Orientation o)
		{
			switch(o)
			{
				case Orientation.East:
					return texture.Copy();
				case Orientation.North:
					return texture.Rotate90();
				case Orientation.West:
					var t = texture.Rotate90();
					var r = t.Rotate90();
					t.Dispose();
					return r;
				case Orientation.South:
					var t1 = texture.Rotate90();
					var t2 = t1.Rotate90();
					var t3 = t2.Rotate90();
					t1.Dispose();
					t2.Dispose();
					return t3;
			}
			return texture.Copy();
		}
		public static Texture2D Tint(this Texture2D texture, Color color)
		{
			int w = texture.Width;
			int h = texture.Height;

			Texture2D blanket = new Texture2D(texture.GraphicsDevice, w, h);
			Color[] newdata = new Color[w * h];
			Color[] olddata = new Color[w * h];
			texture.GetData<Color>(olddata);

			NumExtend.XY(w, h, (x, y) =>
			{
				Color c1 = olddata[NumExtend.Flatten(x, y, w)];
				Color c2 = new Color(
					(int)NumExtend.Lerp(c1.R, color.R, .5f),
					(int)NumExtend.Lerp(c1.G, color.G, .5f),
					(int)NumExtend.Lerp(c1.B, color.B, .5f));
				newdata[NumExtend.Flatten(x, y, w)] = c2;
			});
			blanket.SetData(newdata);
			return blanket;
		}
	}
}
