using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog
{
	static class GameAudio
	{
		static public Dictionary<string, SoundEffect> SFX = new();
		static public float Volume = 1;
		static public void Play(string name)
		{
			SFX[name].Play(Volume,0,0);
		}
	}
}
