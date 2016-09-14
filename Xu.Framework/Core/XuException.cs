using System;
using System.Runtime.Serialization;

namespace Xu.Core
{
	[Serializable]
	public class XuException : Exception
	{
		public XuException()
		{
		}

		public XuException(string message) : base(message)
		{
		}

		public XuException(string message, Exception inner) : base(message, inner)
		{
		}

		protected XuException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
