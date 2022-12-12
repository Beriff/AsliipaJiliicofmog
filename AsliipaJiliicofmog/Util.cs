using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace AsliipaJiliicofmog
{
    public static class Shaders
    {
        public delegate Texture2D Shader(Texture2D input);
        public static Texture2D ChangeTexture(Texture2D input, Shader shader)
        {
            return shader(input);
        }
        ///<summary>Shear the image horizontally</summary>
        public static Shader ShearMapX(float k, GraphicsDevice gd)
        {
            return (input) =>
            {
                var w = input.Width;
                var h = input.Height;
                var new_w = (int)Math.Ceiling(h * k + w);
                Color[] newtexture = new Color[h * new_w];
                Color[] oldtexture = new Color[w * h];
                input.GetData(oldtexture);
                Util.EachXY(w, h, (x, y) =>
                {
                    newtexture[Util.Flatten(x + (int)(k * y), y, new_w)] = oldtexture[Util.Flatten(x, y, w)];
                });
                Texture2D texture = new(gd, new_w, h);
                texture.SetData(newtexture);
                return texture;
            };
        }
        ///<summary>Shrink the image, using the average values</summary>
        public static Shader Shrink(int factor, GraphicsDevice gd)
        {
            return (input) =>
            {
                var w = input.Width;
                var h = input.Height;
                int nw = w / factor;
                int nh = h / factor;
                Color[] newtexture = new Color[nw * nh];
                Color[] oldtexture = new Color[w * h];
                input.GetData(oldtexture);
                Util.EachXY(nw, nh, (x, y) =>
                {
                    var oldx = x * factor;
                    var oldy = y * factor;
                    List<int> rs = new();
                    List<int> gs = new();
                    List<int> bs = new();
                    List<int> _as = new();
                    Util.EachXY(factor, factor, (stepx, stepy) =>
                    {
                        var col = oldtexture[Util.Flatten(oldx + stepx, oldy + stepy, w)];

                        rs.Add(col.R);
                        gs.Add(col.G);
                        bs.Add(col.B);
                        _as.Add(col.A);
                    });
                    int irs = (int)rs.Average();
                    int igs = (int)gs.Average();
                    int ibs = (int)bs.Average();
                    int a = (int)(_as.Average());

                    Color targetcolor = new Color(irs, igs, ibs, a);
                    newtexture[Util.Flatten(x, y, nw)] = targetcolor;
                });
                Texture2D texture = new(gd, nw, nh);
                texture.SetData(newtexture);
                return texture;
            };
        }
        /// <summary>Tint the image</summary>
        public static Shader Blend(float k, Color col, GraphicsDevice gd)
        {
            return (input) =>
            {
                var w = input.Width;
                var h = input.Height;
                Color[] newtexture = new Color[h * w];
                Color[] oldtexture = new Color[w * h];
                input.GetData(oldtexture);
                Util.EachXY(w, h, (x, y) =>
                 {
                     var currcol = oldtexture[Util.Flatten(x, y, w)];
                     var tempcol = Color.Lerp(oldtexture[Util.Flatten(x, y, w)], col, k);
                     newtexture[Util.Flatten(x, y, w)] = new Color(tempcol, currcol.A);
                 });
                Texture2D texture = new(gd, w, h);
                texture.SetData(newtexture);
                return texture;
            };

        }
        ///<summary>Compress the image vertically</summary>
        public static Shader ShrinkMapY(float k, GraphicsDevice gd)
        {
            return (input) =>
            {
                var w = input.Width;
                var h = input.Height;
                var new_h = (int)Math.Ceiling(h / k);
                Color[] newtexture = new Color[w * new_h];
                Color[] oldtexture = new Color[w * h];
                input.GetData(oldtexture);
                Util.EachXY(w, h, (x, y) =>
                {
                    var ypos = Util.Round(y / k);
                    newtexture[Util.Flatten(x, ypos, w)] = oldtexture[Util.Flatten(x, y, w)];
                });
                Texture2D texture = new(gd, w, new_h);
                texture.SetData(newtexture);
                return texture;

            };
        }
        ///<summary>Substitute every opaque color with given one</summary>
        public static Shader ColorOpaque(Color color, GraphicsDevice gd)
        {
            return (input) =>
            {
                var w = input.Width;
                var h = input.Height;
                Color[] newtexture = new Color[w * h];
                Color[] oldtexture = new Color[w * h];
                input.GetData(oldtexture);
                Util.EachXY(w, h, (x, y) =>
                {
                    Color currentcolor = oldtexture[Util.Flatten(x, y, w)];
                    if (!(currentcolor.A == 0))
                        currentcolor = color;
                    newtexture[Util.Flatten(x, y, w)] = currentcolor;
                });
                Texture2D texture = new(gd, w, h);
                texture.SetData(newtexture);
                return texture;
            };

        }
        ///<summary>Set pixels with transparency lower than threshold to 0</summary>
        public static Shader ClampOpacity(GraphicsDevice gd, float threshold = 0.5f)
        {
            threshold *= 255;
            return (input) =>
            {
                var w = input.Width;
                var h = input.Height;
                Color[] newtexture = new Color[w * h];
                Color[] oldtexture = new Color[w * h];
                input.GetData(oldtexture);
                Util.EachXY(w, h, (x, y) =>
                {
                    Color target = oldtexture[Util.Flatten(x, y, w)];
                    newtexture[Util.Flatten(x, y, w)] = target.A < threshold ? Color.Transparent : target;
                });
                Texture2D texture = new(gd, w, h);
                texture.SetData(newtexture);
                return texture;
            };
        }
    }
	static class Util
	{
        
		static public void EachXY(int mx, int my, Action<int, int> act, int sx = 0, int sy = 0)
		{
			for (int y = sy; y < my; y++)
			{
				for (int x = sx; x < mx; x++)
				{
					act(x, y);
				}
			}
		}
 

        public struct HslColor
		{
            public short Hue;
            public float Saturation;
            public float Lightness;
            public short H { get => Hue; set => Hue = value; }
            public float S { get => Saturation; set => Saturation = value; }
            public float L { get => Lightness; set => Lightness = value; }
            public HslColor(int h, float s, float l)
			{
                Hue = (short)h;
                Saturation = s;
                Lightness = l;
			}

            public Color RGB()
			{
                float hue2rgb(float p, float q, float t)
                {
                    if (t < 0) { t += 1; }
                    if(t > 1) { t -= 1; }
                    if(t < 1/6) { return p + (q - p) * 6 * t; }
                    if(t < 1/2) { return q; }
                    if(t < 2/3) { return p + (q - p) * (2 / 3 - t) * 6; }
                    return p;
				}
                if (S == 0)
                    return Color.Black;
                var q = L < 0.5f ? L * (1 + S) : L + S - L * S;
                var p = 2 * L - q;
                var r = (int)hue2rgb(p, q, H + 1 / 3);
                var g = (int)hue2rgb(p, q, H);
                var b = (int)hue2rgb(p, q, H - 1 / 3);
                return new Color(r * 255, g * 255, b * 255);
			}
		}
        public static HslColor HSL(this Color col)
		{
            var r = col.R / 255;
            var g = col.G / 255;
            var b = col.B / 255;
            var max = new float[]{ r, g, b }.Max();
            var min = new float[] { r, g, b }.Min();
            float h, s, l;
            h = s = l = (max + min) / 2;

            if(max == min) { h = s = 0; }
            else
			{
                var d = max - min;
                s = l > 0.5f ? d / (2 - max - min) : d / (max + min);
                
                if (max == r) { h = (g - b) / d + (g < b ? 6 : 0); }
                else if (max == g) { h = (b - r) / d + 2; }
                else { h = (r - g) / d + 4; }
                h /= 6;
			}
            return new HslColor((int)h, s, l);
        }

        static public bool ItemIn(List<Item> itemlist, Item target)
		{
            foreach(var item in itemlist)
			{
                if (item.IsCloneOf(target))
                    return true;
			}
            return false;
		}
		static public T[] Enum2List<T>() where T : Enum
		{
			return (T[])Enum.GetValues(typeof(T));
		}

		static public Color ChangeA(Color basecol, int a)
		{
			return new Color(basecol.R, basecol.G, basecol.B, a);
		}

		static public Action<T> Unite<T>(Action<T> a, Action<T> b)
		{
			return (T x) => { a(x); b(x); };
		}

		static public Vector2 CenterOffset(SpriteBatch sb)
		{
			int width = sb.GraphicsDevice.Viewport.Width;
			int height = sb.GraphicsDevice.Viewport.Height;
			return new Vector2(width / 2, height / 2);
		}

        static char[] splitChars = new char[] { ' ', '-', '\t' };
        static public string WordWrap(string str, int width)
        {
            string[] words = Explode(str, splitChars);

            int curLineLength = 0;
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < words.Length; i += 1)
            {
                string word = words[i];
                // If adding the new word to the current line would be too long,
                // then put it on a new line (and split it up if it's too long).
                if (curLineLength + Asliipa.GameFont.MeasureString(word).X > width)
                {
                    // Only move down to a new line if we have text on the current line.
                    // Avoids situation where wrapped whitespace causes emptylines in text.
                    if (curLineLength > 0)
                    {
                        strBuilder.Append(Environment.NewLine);
                        curLineLength = 0;
                    }

                    // If the current word is too long to fit on a line even on it's own then
                    // split the word up.
                    while (word.Length > width)
                    {
                        strBuilder.Append(word.Substring(0, width - 1) + "-");
                        word = word.Substring(width - 1);

                        strBuilder.Append(Environment.NewLine);
                    }

                    // Remove leading whitespace from the word so the new line starts flush to the left.
                    word = word.TrimStart();
                }
                strBuilder.Append(word);
                curLineLength += (int)Asliipa.GameFont.MeasureString(word).X;
            }

            return strBuilder.ToString();
        }

        static public string[] Explode(string str, char[] splitChars)
        {
            List<string> parts = new List<string>();
            int startIndex = 0;
            while (true)
            {
                int index = str.IndexOfAny(splitChars, startIndex);

                if (index == -1)
                {
                    parts.Add(str.Substring(startIndex));
                    return parts.ToArray();
                }

                string word = str.Substring(startIndex, index - startIndex);
                char nextChar = str.Substring(index, 1)[0];
                // Dashes and the likes should stick to the word occuring before it. Whitespace doesn't have to.
                if (char.IsWhiteSpace(nextChar))
                {
                    parts.Add(word);
                    parts.Add(nextChar.ToString());
                }
                else
                {
                    parts.Add(word + nextChar);
                }

                startIndex = index + 1;
            }
        }

        //Im so sick of XNA's Point class utter uselessness
        static public Point Sub(this Point self, Point other)
		{
            return new(self.X - other.X, self.Y - other.Y);
		}
        static public Point Div(this Point self, Point other)
        {
            return new(self.X / other.X, self.Y / other.Y);
        }
        static public Point Div(this Point self, int other)
        {
            return new(self.X / other, self.Y / other);
        }
        static public Point Mult(this Point self, int other)
		{
            return new(self.X * other, self.Y * other);
		}

        static public void DPrint(string text)
		{
            System.Diagnostics.Debug.WriteLine(text);
		}

        static public Vector2 RandomNear(Vector2 center, Vector2? maxdist_n = null)
		{
            Vector2 maxdist = maxdist_n ?? new(Tile.TextureSize * 5);
            var randx = Asliipa.Random.Next((int)(center.X - maxdist.X), (int)(center.X + maxdist.X));
            var randy = Asliipa.Random.Next((int)(center.Y - maxdist.Y), (int)(center.Y + maxdist.Y));
            return new(randx, randy);
        }

        static public Vector2 NormThis(this Vector2 a)
		{
            a.Normalize();
            return a;
		}
        static public Vector2 Abs(this Vector2 a)
		{
            return new(Math.Abs(a.X), Math.Abs(a.Y));
		}
        static public float Deg2Rad(float deg)
		{
            return (float)(deg * Math.PI / 180f);
		}
        static public Vector2 Rotate(this Vector2 a, float angle_degree)
		{
            float angle = Deg2Rad(angle_degree);
            var x = a.X * Math.Cos(angle) - a.Y * Math.Sin(angle);
            var y = a.X * Math.Sin(angle) + a.Y * Math.Cos(angle);
            return new((float)x, (float)y);
            
		}

        //Reduce 2D coordinates to 1D coordinates
        static public int Flatten(int x, int y, int w)
		{
            return y * w + x;
		}

        static public Texture2D Subtexture(this Texture2D self, int width, int height)
		{
            var newtexture = new Texture2D(self.GraphicsDevice, width, height);
            Color[] olddata = new Color[self.Width * self.Height];
            self.GetData(olddata);
            Color[] newdata = new Color[width * height];
            EachXY(width, height, (x, y) => { newdata[Flatten(x, y, width)] = olddata[Flatten(x, y, self.Width)]; });
            newtexture.SetData(newdata);
            return newtexture;
		}
        public static Vector2 Size(this Texture2D self)
		{
            return new(self.Width, self.Height);
		}
        public static int Round(float x)
		{
            return (int)Math.Round(x);
		}


    }

	public class WeightedList<T>
	{
        private Dictionary<T, int> _list = new();
        private int totalWeight = 0;

        public int Count { get => _list.Count; }

        public void Add(T element, int weight)
		{
            _list[element] = weight;
            totalWeight += weight;
		}
        public T Get()
		{
            int r = Asliipa.Random.Next(0, totalWeight);
            T selection = default;
            foreach(var e in _list)
			{
                if(r < e.Value)
				{
                    selection = e.Key;
                    break;
				}
                r -= e.Value;
			}
            return selection;
		}
        
        public static WeightedList<V> CopyFrom<V>(WeightedList<V> other) where V : ICloneable
		{
            var newlist = new WeightedList<V>();
            foreach(var pair in other._list)
			{
                newlist.Add((V)pair.Key.Clone(), pair.Value);
			}
            return newlist;
		}

	}
}
