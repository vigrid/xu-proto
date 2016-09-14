using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xu.Graphics.VertexFormats
{
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	public struct VertexPositionNormalColor : IVertexType
	{
		private static readonly VertexDeclaration VertexDeclaration;

		[FieldOffset(0)] public Vector3 Position;
		[FieldOffset(12)] public Vector3 Normal;
		[FieldOffset(24)] public Color Color;

		static VertexPositionNormalColor()
		{
			VertexDeclaration = new VertexDeclaration(new[]
			{
				new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
				new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
				new VertexElement(24, VertexElementFormat.Color, VertexElementUsage.Color, 0)
			})
			{
				Name = "VertexPositionNormalColor.VertexPositionNormalColor"
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
