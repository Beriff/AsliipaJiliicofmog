using AsliipaJiliicofmog.Event;

namespace AsliipaJiliicofmog.Tests
{
	[TestClass]
	public class GameEventTests
	{
		[TestMethod]
		public void EventManager_EventDiscard()
		{
			EventManager em = new();
			GameEvent parentevent = new(0, 1, "test", default, _ => { });
			em.AddEvent(parentevent);
			GameEvent e = new(0, 1, "test", EventQueueBehavior.Discard, _ => { });
			em.AddEvent(e);

			Assert.IsFalse(em.Events.Contains(e));

		}
		[TestMethod]
		public void EventManager_EventEnqueue()
		{
			EventManager em = new();
			GameEvent parentevent = new(0, 1, "test", default, _ => { });
			em.AddEvent(parentevent);
			GameEvent e = new(0, 1, "test", EventQueueBehavior.Enqueue, _ => { });
			em.AddEvent(e);
			Assert.IsFalse(em.Events.Contains(e));
			Assert.IsTrue(em.Queue.Contains(e));
		}
		[TestMethod]
		public void EventManager_EventIgnore()
		{
			EventManager em = new();
			GameEvent parentevent = new(0, 1, "test", default, _ => { });
			em.AddEvent(parentevent);
			GameEvent e = new(0, 1, "test", EventQueueBehavior.Ignore, _ => { });
			em.AddEvent(e);
			Assert.AreEqual(2, em.Events.Count);
		}
		[TestMethod]
		public void EventManager_EventReplace()
		{
			EventManager em = new();
			GameEvent parentevent = new(0, 1, "test", default, _ => { });
			em.AddEvent(parentevent);
			GameEvent e = new(0, 1, "test", EventQueueBehavior.Replace, _ => { });
			em.AddEvent(e);
			Assert.IsTrue(em.Events.Contains(e));
			Assert.IsFalse(em.Events.Contains(parentevent));
		}
	}
}