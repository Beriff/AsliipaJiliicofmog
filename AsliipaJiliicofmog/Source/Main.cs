using AsliipaJiliicofmog;
using System;

namespace Asliipa
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			using var game = new Client();
				game.Run();

		}
	}
}
