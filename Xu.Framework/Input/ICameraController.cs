using Microsoft.Xna.Framework;

namespace Xu.Input
{
	public interface ICameraController
	{
		void Translate(Vector3 deltaTranslation);
		void Translate(float deltaX, float deltaY, float deltaZ);

		void Rotate(Vector3 deltaRotation);
		void Rotate(float deltaYaw, float deltaPitch, float deltaRoll);

		void Accelerate(float multiplier);

		bool InvertX { get; set; }
		bool InvertY { get; set; }
		bool InvertZ { get; set; }
	}
}
