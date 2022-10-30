using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace AsliipaJiliicofmog
{
	class Registry
	{
		public static Dictionary<string, Texture2D> TextureRegistry = new();
		public static Dictionary<string, (Creature creature, List<string> behaviors)> CreatureRegistry = new();
		public static void LoadTextureDirectory(string path, GraphicsDevice gd)
		{
			foreach(var fullpath in Directory.GetFiles(path))
			{
				var filename = fullpath.Replace(path + "\\", "").Replace(".png", "");
				TextureRegistry[filename] = Texture2D.FromStream(gd, new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.None));
			}
		}
		public static void LoadCreatureDirectory(string path)
		{
			foreach(var fullpath in Directory.GetFiles(path))
			{
				var json = File.ReadAllText(fullpath);
				dynamic decoded = JObject.Parse(json);
				CreatureRegistry[(string)decoded.name] = (new Creature(TextureRegistry[(string)decoded.texture], (string)decoded.name, desc: (string)decoded.desc), new());
				var creaturepair = CreatureRegistry[(string)decoded.name];
				creaturepair.creature.Speed = (float)decoded.speed;

				foreach(string behavior in decoded.behavior)
				{
					//creaturepair.creature.OnUpdate += (Action<GameClient>)typeof(Behavior).GetMethod(behavior).Invoke(null, new object[1] { creaturepair.creature });
					creaturepair.behaviors.Add(behavior);
					
				}
			}
		}
		public static Creature GetCreature(string name)
		{
			var creature = CreatureRegistry[name].creature.Clone() as Creature;
			foreach(var beh in CreatureRegistry[name].behaviors)
			{
				creature.OnUpdate += (Action<GameClient>)typeof(Behavior).GetMethod(beh).Invoke(null, new Creature[1] { creature });
			}
			return creature;
		}
	}
}
