using AsliipaJiliicofmog.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace AsliipaJiliicofmog.Env
{
	public class Tile : ICloneable
	{
		public const int Width = 16;
		public const int Height = 16;
		public static Vector2 Size { get => new(Width, Height); }

		public Texture2D TileTexture;
		public string Name;

		public Tile(Texture2D TileTexture, string name)
		{
			this.TileTexture = TileTexture;
			Name = name;
		}

		public void Render(SpriteBatch sb, Vector2 position)
		{
			sb.Draw(TileTexture, position, Color.White);
		}

		/// <summary>
		/// Deserialize JSON description of a tile into an object
		/// </summary>
		public static Tile Deserialize(string json)
		{
			dynamic obj = JsonConvert.DeserializeObject(json);
			return new(Registry.Textures[obj.Texture.ToString()], obj.Name.ToString());
		}

		public object Clone() => new Tile(TileTexture, Name);
	}
}
