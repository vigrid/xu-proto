using Sample02;
using Sample03;
using Sample04;


namespace Sample01
{
#if WINDOWS || XBOX
	internal static class Program
	{
		private static void Main()
		{
			using (var game = new SampleGame04())
			{
				game.Run();
			}
		}
	}
#endif
}
