using AsliipaJiliicofmog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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
