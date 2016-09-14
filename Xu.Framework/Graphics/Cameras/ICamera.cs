using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xu.Graphics.Cameras
{
	public interface ICamera
	{
		Matrix View { get; }
		Matrix Projection { get; }

		BoundingFrustum Frustum { get; }

		Vector3 Position { get; }
		Vector3 Direction { get; }

		Ray Pick(Vector2 nearPlanePosition, Viewport viewport);
	}
}
