using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AsliipaJiliicofmog
{
	class Registry
	{
		public static Dictionary<string, Texture2D> TextureRegistry = new Dictionary<string, Texture2D>();
		public static void LoadTextureDirectory(string path, GraphicsDevice gd)
		{
			foreach(var fullpath in Directory.GetFiles(path))
			{
				var filename = fullpath.Replace(path + "\\", "").Replace(".png", "");
				TextureRegistry[filename] = Texture2D.FromStream(gd, new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.None));
			}
		}
	}
}
