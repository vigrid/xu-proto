using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Xu.Graphics.Deferred
{
	public class DeferredClearEffect : Effect
	{
		public static DeferredClearEffect Create(ContentManager contentManager)
		{
			var effect = contentManager.Load<Effect>("Core/Effects/Clear");
			return new DeferredClearEffect(effect);
		}

		private DeferredClearEffect(Effect cloneSource) : base(cloneSource)
		{
		}
	}
}