using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog
{
	static class Util
	{
        public delegate Texture2D Shader(Texture2D input);
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

        public static Texture2D ChangeTexture(Texture2D input, Shader shader)
        {
            return shader(input);
        }
        public static int Round(float x)
		{
            return (int)Math.Round(x);
		}

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
                EachXY(w, h, (x,y) => 
                {
                    newtexture[Flatten(x + (int)(k * y), y, new_w)] = oldtexture[Flatten(x, y, w)];
                });
                Texture2D texture = new(gd, new_w, h);
                texture.SetData(newtexture);
                return texture;
            };
		}
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
                EachXY(w, h, (x, y) =>
                {
                    var ypos = Round(y / k);
                    newtexture[Flatten(x, ypos, w)] = oldtexture[Flatten(x, y, w)];
                });
                Texture2D texture = new(gd, w, new_h);
                texture.SetData(newtexture);
                return texture;

            };
		}
        public static Shader ColorOpaque(Color color, GraphicsDevice gd)
		{
            return (input) => 
            {
                var w = input.Width;
                var h = input.Height;
                Color[] newtexture = new Color[w*h];
                Color[] oldtexture = new Color[w*h];
                input.GetData(oldtexture);
                EachXY(w, h, (x, y) =>
                {
                    Color currentcolor = oldtexture[Flatten(x, y, w)];
                    if (!(currentcolor.A == 0))
                        currentcolor = color;
                    newtexture[Flatten(x, y, w)] = currentcolor;
                });
                Texture2D texture = new(gd, w, h);
                texture.SetData(newtexture);
                return texture;
            };
            
        }
    }

	public class WeightedList<T>
	{
        private Dictionary<T, int> _list = new();
        private int totalWeight = 0;

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
