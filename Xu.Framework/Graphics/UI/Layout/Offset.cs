namespace Xu.Graphics.UI.Layout
{
	public class Offset
	{
		public Offset(float fraction, int value)
		{
			Fraction = fraction;
			Value = value;
		}

		public float Fraction { get; private set; }
		public int Value { get; private set; }

		public Offset Translated(float fraction)
		{
			return new Offset(Fraction + fraction, Value);
		}

		public Offset Translated(int value)
		{
			return new Offset(Fraction, Value + value);
		}
	}
}
