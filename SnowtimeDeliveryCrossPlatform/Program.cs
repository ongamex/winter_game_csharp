using System;
using System.Text.Json;

namespace Game1
{

	public class Demo
	{
		public float x = 5;
		public float bahur { get; set; }
	}

	public static class Program
	{
		[STAThread]
		static void Main() {
			using (var game = new Game1())
				game.Run();
		}
	}
}
