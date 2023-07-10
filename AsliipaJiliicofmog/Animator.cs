using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog
{
	enum EnqueuingType
	{
		Override,
		Discard
	}
	static class Easing
	{
		public static Func<float, float> OutBack = (t) =>
		{
			float c1 = 1.70158f;
			float c3 = c1 + 1;

			return 1 + c3 * MathF.Pow(t - 1, 3) + c1 * MathF.Pow(t - 1, 2);
		};
		public static Func<float, float> InOutBack = (t) =>
		{
			float c1 = 1.70158f;
			float c2 = c1 * 1.525f;

			return t < 0.5 ? (MathF.Pow(2 * t, 2) * ((c2 + 1) * 2 * t - c2)) / 2
			: (MathF.Pow(2 * t - 2, 2) * ((c2 + 1) * (t * 2 - 2) + c2) + 2) / 2;
		};
	}
	class Animation
	{
		public string? Token;
		public float Data;
		public EnqueuingType Enqueuing;
		public Action<float, float> OnUpdate;
		public Action? OnFinish;
		public bool Repeat;
		public int Lifetime;
		public int Counter = 0;
		public Animation(int lifetime, Action<float, float> onupdate, EnqueuingType enqueuing = EnqueuingType.Override, 
			float data = 0, string token = null, bool repeat = true, Action onfinish = null)
		{
			Lifetime = lifetime;
			OnUpdate = onupdate;
			Enqueuing = enqueuing;
			Data = data;
			Token = token;
			Repeat = repeat;
			OnFinish = onfinish;
		}
	}
	class Animator
	{
		public List<Animation> Animations;
		public void Update()
		{
			List<Animation> toremove = new();
			foreach(var anim in Animations)
			{
				anim.Counter++;
				if (anim.Counter >= anim.Lifetime)
				{
					if (anim.Repeat)
						anim.Counter = 0;
					else
						toremove.Add(anim);
				}
				anim.OnUpdate(anim.Counter / (float)anim.Lifetime, anim.Data);
			}
			foreach(var anim in toremove)
			{
				if (anim.OnFinish != null)
					anim?.OnFinish();
				Animations.Remove(anim);
			}
		}
		public void Add(Animation anim)
		{
			if(anim.Token != null)
			{
				List<Animation> toremove = new();
				bool found = false;
				foreach(var e in Animations)
				{
					if(e.Token == anim.Token)
					{
						found = true;
						if(anim.Enqueuing == EnqueuingType.Override)
						{
							toremove.Add(e);
							Animations.Add(anim);
						}
					}
					if (found) { break; } else { Animations.Add(anim); }
				}
				foreach(var e in toremove) { Animations.Remove(e); }
				
			} else { Animations.Add(anim); }
		}
		public Animator()
		{
			Animations = new();
		}
	}
}
