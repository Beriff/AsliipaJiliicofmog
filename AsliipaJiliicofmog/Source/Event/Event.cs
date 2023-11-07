using System;

namespace AsliipaJiliicofmog.Event
{
	enum EventQueueBehavior
	{
		Ignore,
		Replace,
		Discard,
		Enqueue
	}
	internal class GameEvent
	{
		public float Data;
		public int TickCount = 0;
		public int Lifetime;
		public bool Loop;

		public string Token;
		public EventQueueBehavior QueueBehavior;

		public float Progress { get => TickCount / (float)Lifetime; }

		public Action<GameEvent> Update;
		public Action<GameEvent> OnEnd;

		public GameEvent(float data, int lifetime, string token, EventQueueBehavior behavior, Action<GameEvent> update)
		{
			Data = data;
			Lifetime = lifetime;
			Token = token;
			QueueBehavior = behavior;
			Update = update;
			OnEnd = _ => { };
		}

		public static GameEvent Delay(Action<GameEvent> onend, int delay, string token, EventQueueBehavior behavior)
		{
			return new(0, delay, token, behavior, _ => { }) { OnEnd = onend };
		}
	}
}
