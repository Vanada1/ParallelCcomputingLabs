using System.Drawing;
using System.Drawing.Imaging;

namespace Task1;

public static class BitmapManager
{
	private const string ResultPath = @"Resources\result.jpg";

	public static Color[][] GetImage(string path)
	{
		var bitmapFilePath = path;
		var b1 = new Bitmap(bitmapFilePath);

		var height = b1.Height;
		var width = b1.Width;

		var colorMatrix = CreateColors(width, height);
		for (var i = 0; i < width; i++)
		{
			for (var j = 0; j < height; j++)
			{
				colorMatrix[i][j] = b1.GetPixel(i, j);
			}
		}

		return colorMatrix;
	}

	public static Color[][] CreateColors(int width, int height)
	{
		var colorMatrix = new Color[width][];
		for (var i = 0; i < width; i++)
		{
			colorMatrix[i] = new Color[height];
		}

		return colorMatrix;
	}

	public static Color[][] CreateColors(Size size)
	{
		return CreateColors(size.Width, size.Height);
	}

	public static void SaveImage(Color[][] colors)
	{
		var width = colors.Length;
		var height = colors[0].Length;
		var bitmap = new Bitmap(width, height);
		for (var i = 0; i < width; i++)
		{
			for (var j = 0; j < height; j++)
			{
				bitmap.SetPixel(i, j, colors[i][j]);
			}
		}

		bitmap.Save(ResultPath, ImageFormat.Jpeg);
	}
}