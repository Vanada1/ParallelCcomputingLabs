using System.Drawing;

namespace Task1;

public class ContrastMatrixImage : ContrastMatrixBase<Color>
{
	public ContrastMatrixImage(Color[][] bigImage, Color[][] smallImage) : base(bigImage, smallImage)
	{
		
	}

	private static byte GetByte(int value)
	{
		if (value < 0)
		{
			return 0;
		}

		if (value > 255)
		{
			return 255;
		}

		return (byte)value;
	}

	protected override Color GetDifference(Color first, Color second)
	{
		var r = GetByte(first.R - second.R);
		var g = GetByte(first.G - second.G);
		var b = GetByte(first.B - second.B);
		return Color.FromArgb(r, g, b);
	}
}