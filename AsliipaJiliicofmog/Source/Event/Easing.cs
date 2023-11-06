using System;

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
			t => {
				float c1 = 1.70158f;
				float c3 = c1 + 1;

				return 1 + c3 * MathF.Pow(t - 1, 3) + c1 * MathF.Pow(t - 1, 2);
			};

		public static Ease BackInOut =>
			t => t < 0.5
				? (MathF.Pow(2 * t, 2) * ((2.5949095f + 1) * 2 * t - 2.5949095f)) / 2f
				: (MathF.Pow(2 * t - 2, 2) * ((2.5949095f + 1) * (t * 2 - 2) + 2.5949095f) + 2) / 2f;

		public static Ease BounceInOut => t =>
		{
			float n = 7.5625f;
			float d = 2.75f;

			if (t < 1 / d)
				return n * t * t;
			else if (t < 2 / d)
				return n * (t -= 1.5f / d) * t + 0.75f;
			else if (t < 2.5f / d)
				return n * (t -= 2.25f / d) * t + 0.9375f;

			return n * (t -= 2.625f / d) * t + 0.984375f;
		};

		public static Ease SmoothStep => t =>
			t * t * (3.0f - 2.0f * t);
	}
}
