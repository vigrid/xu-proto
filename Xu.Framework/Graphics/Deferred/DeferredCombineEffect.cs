using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Xu.Graphics.Deferred
{
	public class DeferredCombineEffect : Effect
	{
		public static DeferredCombineEffect Create(ContentManager contentManager)
		{
			var effect = contentManager.Load<Effect>("Core/Effects/Render-Combine");
			return new DeferredCombineEffect(effect);
		}

		private DeferredCombineEffect(Effect cloneSource) : base(cloneSource)
		{
		}
	}
}