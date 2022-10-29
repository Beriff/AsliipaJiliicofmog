using System;

namespace AsliipaJiliicofmog
{
	public static class Program
	{
		[STAThread]
		static void Main()
		{
			using (var game = new Asliipa())
				game.Run();
		}
	}
}
