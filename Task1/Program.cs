using Task1;

const string Img256 = @"Resources\Nature1024.jpg";
const string Img64 = @"Resources\Nature64.jpg";

var img256 = BitmapManager.GetImage(Img256);
var img64 = BitmapManager.GetImage(Img64);

var contrastMatrixImage = new ContrastMatrixImage(img256, img64);

var trueResult = await contrastMatrixImage.GetResult();
var result = await contrastMatrixImage.GetResult();

for (var i = 0; i < trueResult.Length; i++)
{
	for (var j = 0; j < trueResult[0].Length; j++)
	{
		var trueColor = trueResult[i][j];
		var testColor = result[i][j];
		if (trueColor != testColor)
		{
			Console.WriteLine($"True: {trueColor}; Test {testColor}\ti={i}; j={j}");
		}
	}
}

BitmapManager.SaveImage(result);