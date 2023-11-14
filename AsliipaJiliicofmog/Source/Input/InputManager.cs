using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog.Input
{
	public static class InputManager
	{
		public static readonly List<InputConsumer> Consumers = new();

		static InputManager()
		{
			Consumers.Add(new("UI") { Active = false });
			Consumers.Add(new("Gameplay", false));
		}

		public static InputConsumer GetConsumer(string name)
		{
			foreach (var consumer in Consumers)
			{
				if (consumer.Name == name) { return consumer; }
			}

			return null;
		}

		public static void Update()
		{
			bool blockedflag = false;
			foreach(var consumer in Consumers)
			{
				if(blockedflag) { consumer.IsBlocked = true; }
				consumer.Update();
				if(consumer.BlockInputStream && consumer.Active) { blockedflag = true; }
			}
		}
	}
}
