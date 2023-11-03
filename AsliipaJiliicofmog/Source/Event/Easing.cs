using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog.Source.Event
{
	internal static class Easing
	{
		public delegate float Ease(float t);

		public static Ease QuadIn =>
			t => 2 * t * t;
		public static Ease QuadOut =>
			t => 1 - MathF.Pow(-2 * t + 2, 2) / 2;
		public static Ease QuadInOut => 
			t => t < 0.5 ? 2f * t * t : 1 - MathF.Pow(-2 * t + 2, 2) / 2f;

		public static Ease BackIn => 
			t => 2.70158f * t * t * t - 1.70158f * t * t;

		public static Ease BackOut =>
			t => 1 + 2.70158f * MathF.Pow(t - 1, 3) + 2.70158f * MathF.Pow(t - 1, 2);

		public static Ease BackInOut =>
			t => t < 0.5
				? (MathF.Pow(2 * t, 2) * ((2.5949095f + 1) * 2 * t - 2.5949095f)) / 2f
				: (MathF.Pow(2 * t - 2, 2) * ((2.5949095f + 1) * (t * 2 - 2) + 2.5949095f) + 2) / 2f;

		public static Ease BounceInOut => t =>
		{
			float n1 = 7.5625f;
			float d1 = 2.75f;

			if (t < 1 / d1)
			{
				return n1 * t * t;
			}
			else if (t < 2 / d1)
			{
				return n1 * (t -= 1.5f / d1) * t + 0.75f;
			}
			else if (t < 2.5f / d1)
			{
				return n1 * (t -= 2.25f / d1) * t + 0.9375f;
			}
			else
			{
				return n1 * (t -= 2.625f / d1) * t + 0.984375f;
			}
		};
	}
}
