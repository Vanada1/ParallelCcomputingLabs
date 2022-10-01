using System.Drawing;

namespace Task1;

public class ContrastMatrixImage
{
	private readonly Color[][] _bigImage;
	private readonly Size _bigImageSize;

	private readonly Color[][] _smallImage;
	private readonly Size _smallImageSize;

	private bool _isWidthDevided;

	public ContrastMatrixImage(Color[][] bigImage, Color[][] smallImage)
	{
		_bigImage = bigImage;
		_bigImageSize = new Size(_bigImage.Length, _bigImage[0].Length);
		if (_bigImageSize.Width != _bigImageSize.Height)
		{
			throw new ArgumentException($"Big Image must be square Size: {_bigImageSize}");
		}

		_smallImage = smallImage;
		_smallImageSize = new Size(_smallImage.Length, _smallImage[0].Length);
		if (_smallImageSize.Width != _smallImageSize.Height)
		{
			throw new ArgumentException($"Small Image must be square Size: {_smallImageSize}");
		}
	}

	public async Task<Color[][]> GetResult(int threadsCount = 1)
	{
		if (threadsCount <= 0)
		{
			throw new ArgumentException("Threads count cannot be less than 1");
		}

		if (_bigImageSize.Height % threadsCount != 0 
		    || _bigImageSize.Width % threadsCount != 0)
		{
			throw new ArgumentException("Cannot to divide on threads Big Image");
		}

		if (_smallImageSize.Height % threadsCount != 0
		    || _smallImageSize.Width % threadsCount != 0)
		{
			throw new ArgumentException("Cannot to divide on threads Small Image");
		}

		var difference = _bigImageSize.Width / _smallImageSize.Width;
		if (threadsCount == 1)
		{
			return await GetPart(_bigImage, _smallImage, difference);
		}
		
		var currentThread = 0;
		var big = new List<Color[][]> {_bigImage};
		var small = new List<Color[][]> {_smallImage};
		while (currentThread < threadsCount)
		{
			var newBig = new List<Color[][]>();
			var newSmall = new List<Color[][]>();
			for (var i = 0; i < big.Count; i++)
			{
				newBig.AddRange(GetHalf(big[i]));
				newSmall.AddRange(GetHalf(small[i]));
			}

			big = newBig;
			small = newSmall;
			currentThread += 2;
		}

		var countTasks = big.Select((t, i) => GetPart(t, small[i], difference));
		Task.WaitAll(countTasks.ToArray());
		var partsResult = new List<Color[][]>(countTasks.Select(t=>t.Result)).ToList();
		while (partsResult.Count != 1)
		{
			var newPartsResult = new List<Color[][]>();
			for (var i = 0; i < partsResult.Count - 1; i += 2)
			{
				var color1 = partsResult[i];
				var color2 = partsResult[i + 1];
				newPartsResult.Add(MergeColors(color1, color2));
			}

			_isWidthDevided = !_isWidthDevided;
			partsResult = newPartsResult;
		}
		
		return partsResult.First();
	}

	private async Task<Color[][]> GetPart(Color[][] bigColors, Color[][] smallColors, int difference)
	{
		var result = BitmapManager.CreateColors(bigColors.Length, bigColors[0].Length);
		for (var i = 0; i < smallColors.Length; i++)
		{
			for (var j = 0; j < smallColors[0].Length; j++)
			{
				for (var k = 0; k < difference; k++)
				{
					for (var l = 0; l < difference; l++)
					{
						var x = i * difference + k;
						var y = j * difference + l;
						var bigColor = bigColors[x][y];
						var smallColor = smallColors[i][j];
						var r = GetByte(bigColor.R - smallColor.R);
						var g = GetByte(bigColor.G - smallColor.G);
						var b = GetByte(bigColor.B - smallColor.B);
						result[x][y] = Color.FromArgb(r, g, b);
					}
				}
			}
		}

		return result;
	}

	private Color[][] MergeColors(Color[][] firstColors, Color[][] secondColors)
	{
		var width = firstColors.Length;
		var height = firstColors[0].Length;
		var newWidth = width;
		var newHeight = height;
		if (_isWidthDevided)
		{
			newWidth += width;
		}
		else
		{
			newHeight += height;
		}

		var result = BitmapManager.CreateColors(newWidth, newHeight);
		for (var i = 0; i < width; i++)
		{
			for (var j = 0; j < height; j++)
			{
				result[i][j] = firstColors[i][j];
				var x = newWidth != width ? width : 0;
				var y = newHeight != height ? height : 0;
				result[i + x][j + y] = secondColors[i][j];
			}
		}

		return result;
	}

	private IEnumerable<Color[][]> GetHalf(Color[][] colors)
	{
		var width = colors.Length;
		var height = colors[0].Length;
		var widthMultiplier = 0;
		var heightMultiplier = 0;
		if (width > height)
		{
			width /= 2;
			widthMultiplier = 1;
			_isWidthDevided = true;
		}
		else
		{
			height /= 2;
			heightMultiplier = 1;
			_isWidthDevided = false;
		}

		var colors1 = BitmapManager.CreateColors(width, height);
		var colors2 = BitmapManager.CreateColors(width, height);

		for (var i = 0; i < width; i++)
		{
			for (var j = 0; j < height; j++)
			{
				colors1[i][j] = colors[i][j];
				var x = i + width * widthMultiplier;
				var y = j + height * heightMultiplier;
				colors2[i][j] = colors[x][y];
			}
		}

		return new List<Color[][]> {colors1, colors2};
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
}