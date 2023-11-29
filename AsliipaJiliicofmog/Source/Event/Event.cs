namespace AsliipaJiliicofmog.Event
{
	/// <summary>
	/// Indicates how EventManager should treat the event when added
	/// </summary>
	public enum EventQueueBehavior
	{
		/// <summary>
		/// Add a copy of the event to the currently updated events
		/// </summary>
		Ignore,
		/// <summary>
		/// Replace an event with the same token with proposed event
		/// </summary>
		Replace,
		/// <summary>
		/// Discard the propsed event if one with same token is found
		/// </summary>
		Discard,
		/// <summary>
		/// Add the proposed event after the one with same token is finished
		/// </summary>
		Enqueue
	}
	/// <summary>
	/// A wrapper for a function that is executed every Update() cycle by an event manager
	/// </summary>
	public class GameEvent
	{
		/// <summary>
		/// Internal data if the event requires it
		/// </summary>
		public float Data;
		/// <summary>
		/// Amount of times the event has ticked
		/// </summary>
		public int TickCount = 0;
		/// <summary>
		/// Amount of times the event is allowed to be executed
		/// </summary>
		public int Lifetime;
		/// <summary>
		/// Reset the TickCount instead of removing the event if set to true
		/// </summary>
		public bool Loop;

		/// <summary>
		/// Unique identifier
		/// </summary>
		public string Token;
		/// <summary>
		/// How the event should be treated when proposed to the event manager
		/// </summary>
		public EventQueueBehavior QueueBehavior;

		/// <summary>
		/// How close the event is to being finished
		/// </summary>
		public float Progress { get => TickCount / (float)Lifetime; }

		/// <summary>
		/// Triggered every tick
		/// </summary>
		public Action<GameEvent> Update;
		/// <summary>
		/// Triggered when the event dies out
		/// </summary>
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

		/// <summary>
		/// Construct an empty event with a provided function that triggers after the event dies out
		/// </summary>
		public static GameEvent Delay(Action<GameEvent> onend, int delay, string token, EventQueueBehavior behavior)
		{
			return new(0, delay, token, behavior, _ => { }) { OnEnd = onend };
		}
	}
}
