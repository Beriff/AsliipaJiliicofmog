using System;
using System.Collections.Generic;
using System.Text;

namespace AsliipaJiliicofmog.Input
{
	internal static class InputManager
	{
		public static readonly List<InputConsumer> Consumers = new();

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
				if(consumer.BlockInputStream) { blockedflag = true; }
			}
		}
	}
}
