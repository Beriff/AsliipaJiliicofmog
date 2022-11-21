using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog
{
	static class Easing
	{
		public static float OutBack(float x)
		{
			const float c1 = 1.70158f;
			const float c3 = c1 + 1;

			return (float)(1 + c3 * Math.Pow(x - 1, 3) + c1 * Math.Pow(x - 1, 2));
		}
		public static float Linear(float x) => x;
		public static float Smoothstep(float x)
		{
			return (float)(x < 0.5 ? 4 * x * x * x : 1 - Math.Pow(-2 * x + 2, 3) / 2);
		}
		public static float Invert(float x) => 1 - x;

		public static float Lerp(int a, int b, float t) => a * (1 - t) + b * t;

		public static float OutExpo(float x) => x == 1 ? 1 : 1 - (float)Math.Pow(2, -10 * x);

		public static float Signaling(float x) => (float)((Math.Sin((x * Math.PI % Math.PI) + Math.PI / 2f) * 2 + 1) / 2f);
		public static float Parabolic(float x) => (1 - (2 * x - 1) * (2 * x - 1));
		public static Func<float,float> FadeOscillate(int fade, int scale)
		{
			return x => (float)(-Math.Pow(scale, fade) * Math.Sin(x) / Math.Pow(x, fade));
		}
		public static float OutBounce(float x)
		{
			float n1 = 7.5625f;
			float d1 = 2.75f;

			if (x < 1 / d1)
			{
				return n1 * x * x;
			}
			else if (x < 2 / d1)
			{
				return n1 * (x -= 1.5f / d1) * x + 0.75f;
			}
			else if (x < 2.5 / d1)
			{
				return n1 * (x -= 2.25f / d1) * x + 0.9375f;
			}
			else
			{
				return n1 * (x -= 2.625f / d1) * x + 0.984375f;
			}
		}

	}

	public class Animation
	{
		public int FramesSpan;
		int FrameProgress;
		public Action<float, float> OnProgress;
		public bool Finished;
		public float Coefficient;
		public string? Signature;

		public Animation(int span, float coeff, Action<float, float> progress, string? signature = null)
		{
			FramesSpan = span;
			FrameProgress = 0;
			OnProgress = progress;
			Coefficient = coeff;
			Finished = false;
			Signature = signature;
		}
		public void AddProgress()
		{
			if (!Finished)
			{
				FrameProgress += 1;
				if (FrameProgress == FramesSpan)
					Finished = true;
			}
		}
		public void Update()
		{
			OnProgress(FrameProgress / (float)FramesSpan, Coefficient);
		}
	}
	static class Animator
	{
		public static List<Animation> AnimationQueue = new();
		public static void Add(Animation animation) 
		{ 
			foreach(var anim in AnimationQueue)
			{
				if (anim.Signature != null && anim.Signature == animation.Signature)
					return;	
			}
			AnimationQueue.Add(animation);
		}
		public static void Update()
		{
			List<Animation> ToRemove = new();
			foreach(var anim in AnimationQueue)
			{
				anim.AddProgress();
				anim.Update();
				if(anim.Finished) { ToRemove.Add(anim); }
			}
			foreach(var anim in ToRemove) { AnimationQueue.Remove(anim); }
		}
	}
}
