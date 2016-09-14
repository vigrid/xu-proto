using Microsoft.Xna.Framework;

namespace Xu.Extensions
{
	public static class GameServiceContainerExtensions
	{
		public static TService Resolve<TService>(this GameServiceContainer container)
		{
			return (TService) container.GetService(typeof (TService));
		}
	}
}
