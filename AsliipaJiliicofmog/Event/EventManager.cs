using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog.Event
{
	internal class EventManager
	{
		public List<GameEvent> Events;
		private List<GameEvent> Queue;
		public EventManager()
		{
			Events = new();
			Queue = new();
		}
		private GameEvent? FindQueuedEvent(string name)
		{
			foreach (GameEvent e in Queue)
			{
				if (e.Token == name) { return e; }
			}
			return null;
		}
		public GameEvent? FindEvent(string name)
		{
			foreach(GameEvent e in Events)
			{
				if(e.Token == name) { return e; }
			}
			return null;
		}
		public void AddEvent(GameEvent gameEvent)
		{
			var ev = FindEvent(gameEvent.Token);
			if (ev != null)
			{
				switch(gameEvent.QueueBehavior)
				{
					case EventQueueBehavior.Discard:
						return;
					case EventQueueBehavior.Ignore:
						Events.Add(gameEvent); break;
					case EventQueueBehavior.Replace:
						Events.Remove(ev); Events.Add(gameEvent); break;
					case EventQueueBehavior.Enqueue:
						Queue.Add(gameEvent); break;
				}
			} else { Events.Add(gameEvent); }
		}
		public void Update()
		{
			List<GameEvent> toremove = new();
			List<GameEvent> toadd = new();
			foreach(GameEvent e in Events)
			{
				e.TickCount++;
				if(e.TickCount >= e.Lifetime)
				{
					if (e.Loop) { e.OnEnd(e); e.TickCount = 0; continue; }

					var ev = FindQueuedEvent(e.Token);
					if(ev != null) { toadd.Add(ev); }
					toremove.Add(e);

				}
				e.Update(e);
			}
			foreach(var e in toremove) { Events.Remove(e); }
			foreach(var e in toadd) { Events.Add(e); Queue.Remove(e); } //add from queue
		}
	}
}
