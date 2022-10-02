using Task1;

const string Img256 = @"Resources\Nature1024.jpg";
const string Img64 = @"Resources\Nature64.jpg";
const int Times = 3;

async Task StartTest(int bigPow, int smallPow, int times)
{
	Console.WriteLine($"Start test: Big size - 2^{bigPow}, 2^{bigPow}; Small size - 2^{smallPow}, 2^{smallPow}");
	var threads = new[] {1, 2, 4, 8, 16};
	foreach (var thread in threads)
	{
		for (var i = 0; i < times; i++)
		{
			Console.Write($"Treads count - {thread}; Times - {i}");
			var bigCount = (int)Math.Pow(2, bigPow);
			var smallCount = (int)Math.Pow(2, smallPow);
			var data = new Data(bigCount, smallCount, i, thread);
			await data.Start();
			Console.Write($"; Without Prepare - {data.TimerWithoutPrepare.Elapsed}; With Prepare - {data.TimerWithPrepare.Elapsed}");
			Console.WriteLine();
		}

		Console.WriteLine();
	}

	Console.WriteLine($"End test: Big size - 2^{bigPow}, 2^{bigPow}; Small size - 2^{smallPow}, 2^{smallPow}");
}

var img256 = BitmapManager.GetImage(Img256);
var img64 = BitmapManager.GetImage(Img64);

var contrastMatrixImage = new ContrastMatrixImage(img256, img64);

var result = await contrastMatrixImage.GetResult();

BitmapManager.SaveImage(result);

await StartTest(12,10,Times);
await StartTest(12,9,Times);
await StartTest(13,9,Times);