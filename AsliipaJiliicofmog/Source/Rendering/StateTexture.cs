using AsliipaJiliicofmog.Event;
using AsliipaJiliicofmog.Rendering;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;

namespace AsliipaJiliicofmog.Rendering
{
	public class StateTexture : IGameTexture
	{
		public Dictionary<string, IGameTexture> States;
		public IGameTexture SelectedState;
		public IGameTexture? InterfaceState;

		public void SwitchState(string state) { SelectedState = States[state]; }
		public Point Size { get => (InterfaceState ?? SelectedState).Size; }
		public void PlayAnimation(EventManager em, IGameTexture animation, int length)
		{
			InterfaceState = animation;
			em.AddEvent(GameEvent.Delay(
				(ge) =>
				{
					InterfaceState = null;
				}, length, "", EventQueueBehavior.Replace
			));
		}

		public void Render(SpriteBatch sb, Vector2 position, Color color)
		{
			(InterfaceState ?? SelectedState).Render(sb, position, color);
		}

		public StateTexture(params (string, IGameTexture t)[] states)
		{
			States = new();
			foreach (var (name, texture) in states)
				States[name] = texture;
			SelectedState = states[0].t;
		}
	}
}
