using System.Diagnostics;
using System.Text;

namespace Task1;

public class Data
{
	private readonly ContrastMatrixNumbers _contrastMatrixNumbers;

	private readonly int[][] _bigMatrix;

	private readonly int[][] _smallMatrix;

	private int[][] _result;

	private readonly int _bigCount;

	private readonly int _smallCount;

	private readonly int _tryNumber;

	private readonly int _threadsCount;

	public Stopwatch TimerWithoutPrepare => _contrastMatrixNumbers.TimerWithoutPrepare;

	public Stopwatch TimerWithPrepare => _contrastMatrixNumbers.TimerWithPrepare;

	public Data(int bigCount, int smallCount, int tryNumber, int threadsCount)
	{
		_bigCount = bigCount;
		_smallCount = smallCount;
		_tryNumber = tryNumber;
		_threadsCount = threadsCount;
		_bigMatrix = CreateMatrix(bigCount, bigCount);
		_smallMatrix = CreateMatrix(smallCount, smallCount);
		_contrastMatrixNumbers = new ContrastMatrixNumbers(_bigMatrix, _smallMatrix);
	}

	public void Start()
	{
		_result = _contrastMatrixNumbers.GetResult(_threadsCount);
		SaveResult();
	}

	private void SaveResult()
	{
		var folderName = $"D:\\Labs\\Results3\\SB_{_bigCount}_SS_{_smallCount}_threads_{_threadsCount}_try_{_tryNumber}\\";
		if (!Directory.Exists(folderName))
		{
			Directory.CreateDirectory(folderName);
		}

		const string bigFileName = "big.txt";
		const string smallFileName = "small.txt";
		const string resultFileName = "result.txt";

		SaveMatrix(_bigMatrix, folderName + bigFileName,
			Array.Empty<Stopwatch>());
		SaveMatrix(_smallMatrix, folderName + smallFileName,
			Array.Empty<Stopwatch>());
		SaveMatrix(_result, folderName + resultFileName,
			new[] {TimerWithPrepare, TimerWithoutPrepare});
	}

	private static void SaveMatrix(int[][] matrix, string filePath, IEnumerable<Stopwatch> stopwatches)
	{
		var fileStream = File.Open(filePath, FileMode.Create);
		using var streamWriter = new StreamWriter(fileStream);

		foreach (var stopwatch in stopwatches)
		{
			streamWriter.WriteLine(stopwatch.Elapsed);
		}

		streamWriter.WriteLine();
		foreach (var i in matrix)
		{
			foreach (var j in i)
			{
				streamWriter.Write($"{j}\t");
			}

			streamWriter.WriteLine();
		}
	}

	private static int[][] CreateMatrix(int width, int height)
	{
		var matrix = BitmapManager.CreateMatrix<int>(width, height);
		var rand = new Random();
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				matrix[i][j] = rand.Next(0, 100);
			}
		}

		return matrix;
	}
}