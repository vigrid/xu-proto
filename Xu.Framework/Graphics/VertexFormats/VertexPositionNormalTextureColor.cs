using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xu.Graphics.VertexFormats
{
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	public struct VertexPositionNormalTextureColor : IVertexType
	{
		private static readonly VertexDeclaration VertexDeclaration;

		[FieldOffset(0)] public Vector3 Position;
		[FieldOffset(12)] public Vector3 Normal;
		[FieldOffset(24)] public Vector2 TextureCoordinate;
		[FieldOffset(32)] public Color Color;

		static VertexPositionNormalTextureColor()
		{
			VertexDeclaration = new VertexDeclaration(new[]
			{
				new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
				new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
				new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
				new VertexElement(32, VertexElementFormat.Color, VertexElementUsage.Color, 0)
			})
			{
				Name = "VertexPositionNormalTextureColor.VertexDeclaration"
			};
		}

		#region IVertexType Members

		VertexDeclaration IVertexType.VertexDeclaration
		{
			get { return VertexDeclaration; }
		}

		#endregion
	}
}
