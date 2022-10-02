using Task1;

const string Img256 = @"Resources\Nature1024.jpg";
const string Img64 = @"Resources\Nature64.jpg";
const int Times = 10;

void WriteLog(string text, StreamWriter streamWriter)
{
	Console.WriteLine(text);
	streamWriter.WriteLine(text);
}

long GetMiddle(List<long> values)
{
	var sum = values.Sum();
	return sum / values.Count;
}

async Task StartTest(int bigPow, int smallPow, int times, StreamWriter streamWriter)
{
	WriteLog($"Start test: Big size - 2^{bigPow}, 2^{bigPow}; Small size - 2^{smallPow}, 2^{smallPow}", streamWriter);
	var timeWithoutPrepare = new List<long>();
	var timeWithPrepare = new List<long>();
	var threads = new[] {1, 2, 4, 8, 16};
	foreach (var thread in threads)
	{
		for (var i = 0; i < times; i++)
		{
			var bigCount = (int)Math.Pow(2, bigPow);
			var smallCount = (int)Math.Pow(2, smallPow);
			var data = new Data(bigCount, smallCount, i, thread);
			await data.Start();

			WriteLog($"Treads count - {thread}; Times - {i}; Without Prepare - {data.TimerWithoutPrepare.ElapsedMilliseconds}; With Prepare - {data.TimerWithPrepare.ElapsedMilliseconds}", streamWriter);

			timeWithoutPrepare.Add(data.TimerWithoutPrepare.ElapsedMilliseconds);
			timeWithPrepare.Add(data.TimerWithPrepare.ElapsedMilliseconds);
		}

		WriteLog($"Middle Without Prepare - {GetMiddle(timeWithoutPrepare)}; With Prepare - {GetMiddle(timeWithPrepare)}", streamWriter);
		WriteLog(string.Empty, streamWriter);
	}

	WriteLog($"End test: Big size - 2^{bigPow}, 2^{bigPow}; Small size - 2^{smallPow}, 2^{smallPow}", streamWriter);
}

var img256 = BitmapManager.GetImage(Img256);
var img64 = BitmapManager.GetImage(Img64);

var contrastMatrixImage = new ContrastMatrixImage(img256, img64);

var result = await contrastMatrixImage.GetResult();

BitmapManager.SaveImage(result);

var fileStream = File.Open("log.txt", FileMode.Create);
await using var streamWriter = new StreamWriter(fileStream) { AutoFlush = true };
await StartTest(12,10,Times, streamWriter);
await StartTest(12, 9, Times, streamWriter);
await StartTest(13,9,Times, streamWriter);